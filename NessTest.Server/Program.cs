using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Services.mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<ISessionFactoryHelper, NHibernateHelper>();
var corsOrigins = builder.Configuration["CorsOrigins"]?.Split(",");
if (corsOrigins != null)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder
          .WithOrigins(corsOrigins)
          .AllowAnyHeader()
          .WithExposedHeaders("*")
          .AllowAnyMethod()
          .AllowCredentials();
        });
    });
}


builder.Services.AddControllers();
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new EntityToModel());

});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller}/{action=Index}/{id?}");
});


app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();