using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using CityInfo.API.Services;
using Microsoft.Extensions.Configuration;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        // Startup Constructor
        public Startup(IHostingEnvironment env)
        {
            // Set up mapping to appSettings.json and appSettings.Production.json files
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional:true, reloadOnChange:true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()))
                ;

            // .AddJsonOptions(o => 
            // {
            //     if (o.SerializerSettings.ContractResolver != null)
            //     {
            //         var castedResolver = o.SerializerSettings.ContractResolver
            //              as DefaultContractResolver;

            //         // JSON will simply take property names as they are defined on our class
            //         castedResolver.NamingStrategy = null;
            //     }
            // });


            // Set up Mailing Service
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            // Add Db Context
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));
        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            CityInfoContext cityInfoContext)
        {
            // Logs through console window and in Debug Output window
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();

            // Will only fire in Development environment
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            // Seed Data
            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();
            app.UseMvc();

            //app.Run((context) =>
            //{
            //    throw new Exception("Example exception");
            //});
        }
    }
}
