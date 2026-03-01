using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;
using ISession = NHibernate.ISession;

namespace DataAccess
{
    public interface ISessionFactoryHelper
    {
        ISession GetSession();
        IStatelessSession GetStateLessSession();
    }
}
