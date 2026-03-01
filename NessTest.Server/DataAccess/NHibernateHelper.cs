using Entities.Dbo;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ISession = NHibernate.ISession;

namespace DataAccess
{
    public class NHibernateHelper : ISessionFactoryHelper
    {

        private static ISessionFactory _sessionFactory;
        [ThreadStatic]
        private static ISession _currentSession;
        private IConfiguration _configuration;

        public NHibernateHelper(IConfiguration Configuration)
        {
            _configuration = Configuration;
            getSessionFactory();
        }
        private ISessionFactory getSessionFactory()
        {
            lock (this)
            {
                if (_sessionFactory == null)
                {
                    try
                    {

                        FluentConfiguration configuration = GetConfiguration();
                        _sessionFactory = configuration.BuildSessionFactory();
                    }
                    catch (Exception e)
                    {
                        var s = e.Message;
                    }
                }

                return _sessionFactory;
            }
        }


        private FluentConfiguration GetConfiguration()
        {
            var connStr = _configuration.GetConnectionString("ConnStr");
            // כעת, AppSettings:MapNamespace מכיל את שם ה-Namespace הרצוי
            var targetNamespace = _configuration["AppSettings:MapNamespace"];

            // 1. קבלת ה-Assembly לפי ה-Namespace
            var mappingAssembly = GetAssemblyByNamespace(targetNamespace);

            if (mappingAssembly == null)
            {
                throw new InvalidOperationException($"Assembly containing namespace '{targetNamespace}' was not found among loaded assemblies.");
            }

            // 2. בניית הקונפיגורציה של Fluent NHibernate
            var config = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    // **שינוי קריטי:** הגדרת הדרייבר והדיאלקט לגרסאות המודרניות
                    .Driver<NHibernate.Driver.MicrosoftDataSqlClientDriver>() // הדרייבר החדש
                    .Dialect<NHibernate.Dialect.MsSql2012Dialect>()         // הדיאלקט המתאים (אם זו הגרסה שלך)
                    .DefaultSchema("dbo").AdoNetBatchSize(0)
                    .ConnectionString(connStr))
                .Mappings(m =>
                    m.FluentMappings.AddFromAssembly(mappingAssembly))
                // ... שאר ההגדרות
                .ExposeConfiguration(cfg => new SchemaExport(cfg)
                                            .Create(true, false));
            return config;
        }
        /// <summary>
        /// מחפש Assembly נטען שמכיל מחלקה כלשהי השייכת ל-Namespace הנתון.
        /// </summary>
        /// <param name="targetNamespace">ה-Namespace שאותו יש לחפש.</param>
        /// <returns>ה-Assembly המתאים, או null אם לא נמצא.</returns>
        private Assembly GetAssemblyByNamespace(string targetNamespace)
        {
            // סורקים את כל ה-Assemblies שנטענו כבר ע"י ה-AppDomain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // נסו למצוא מחלקה כלשהי (Type) בתוך ה-Assembly הזה
                    // ששם ה-Namespace שלה תואם ל-targetNamespace.
                    var foundType = assembly.GetTypes()
                        .FirstOrDefault(t =>
                            t.IsClass &&
                            !t.IsAbstract &&
                            t.Namespace != null &&
                            t.Namespace.Contains(targetNamespace, StringComparison.Ordinal));

                    if (foundType != null)
                    {
                        // מצאנו את ה-Assembly שמכיל את ה-Namespace!
                        return assembly;
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // מתעלמים מ-Assemblies שאי אפשר לטעון את כל ה-Types שלהם (נפוץ ב-Reflection)
                }
            }

            // אם ה-Assembly לא נטען, ננסה לטעון אותו באופן גנרי.
            // אזהרה: טעינה זו עלולה להיות פחות מדויקת.
            try
            {
                // אם שם ה-Namespace זהה לשם ה-Assembly, אפשר לנסות לטעון אותו.
                return Assembly.Load(targetNamespace);
            }
            catch (FileNotFoundException)
            {
                // לא נמצא Assembly עם שם ה-Namespace
                return null;
            }
            catch (Exception)
            {
                // שגיאה אחרת בטעינה
                return null;
            }

            return null;
        }
        private FluentConfiguration GetConfiguration1()
        {

            var connStr = _configuration.GetConnectionString("ConnStr");
            string targetNamespace = "Entities";

            // קבל הפניה ל-Assembly הנוכחי (או ל-Assembly ספציפי אחר)
            Assembly assemblyToScan = Assembly.GetExecutingAssembly(); // או Assembly.GetAssembly(typeof(SomeTypeInAssembly))


            //AppDomain.CurrentDomain.Load(_configuration["AppSettings:MapAssembly"]);
            //var config = Fluently.Configure()
            //                     .Database(MsSqlConfiguration.MsSql2012.DefaultSchema("dbo").AdoNetBatchSize(0)
            //                               .ConnectionString(connStr))
            //    .Mappings(m =>
            //              m.FluentMappings.AddFromAssembly(AppDomain.CurrentDomain.GetAssemblies().Single(x => x.GetName().Name == _configuration["AppSettings:MapAssembly"])))


            //    .ExposeConfiguration(cfg => new SchemaExport(cfg)
            //                                    .Create(true, false));
            //var config = Fluently.Configure()
            //    .Database(MsSqlConfiguration.MsSql2012.DefaultSchema("dbo").AdoNetBatchSize(0).ConnectionString(connStr))
            //    .Mappings(m =>
            //    {
            //        m.AutoMappings.Add(AutoMap.Assembly(assemblyToScan).Where(type => type.Namespace.Contains(targetNamespace)));
            //    })
            //    .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, false));

            var config = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.DefaultSchema("dbo").AdoNetBatchSize(0).ConnectionString(connStr))
                .Mappings(m =>
                {
                    m.AutoMappings.Add(
                        AutoMap.Assembly(assemblyToScan)
                            .Where(type => type.Namespace != null && type.Namespace.Contains(targetNamespace))
                    );
                })
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, false));


            return config;
        }

        private ISession getExistingOrNewSession(ISessionFactory factory)
        {
            {
                if (_currentSession == null)
                {
                    _currentSession = factory.OpenSession();
                }
                else if (!_currentSession.IsOpen)
                {
                    _currentSession = factory.OpenSession();
                }
            }

            return _currentSession;
        }

        public ISession GetSession()
        {
            return getExistingOrNewSession(_sessionFactory);
        }

        public IStatelessSession GetStateLessSession()
        {
            return _sessionFactory.OpenStatelessSession();
        }

        private ISession openSessionAndAddToContext(ISessionFactory factory)
        {
            ISession session = factory.OpenSession();
            return session;
        }
    }
}
