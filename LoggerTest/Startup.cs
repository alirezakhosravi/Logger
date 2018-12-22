using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using LoggerTest.Models;
using Raveshmand.Logger.Extentions;

namespace LoggerTest
{
    public class Startup
    {
        private static readonly LoggerFactory LoggerFactory
            = new LoggerFactory(new[]
            {
                new DebugLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Information)
            });

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<Context>((provider, builder) =>
            {
                builder.UseSqlServer("Data Source=localhost,1433;user id=sa; password=123qweRt;Initial Catalog=TestDb;MultipleActiveResultSets=true",
                        optionsBuilder =>
                        {
                            optionsBuilder.CommandTimeout(180);
                            optionsBuilder.UseRowNumberForPaging();
                        })
                    .ConfigureWarnings(warnings => warnings.Throw(CoreEventId.IncludeIgnoredWarning))
                    .UseLoggerFactory(LoggerFactory);
            });

            services.AddLogger<Context>(options =>
            {
                options.LoggerName = "Logger";
                options.Filter = (category, level) => true;// category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            loggerFactory.AddEntityFramework<Context>(app.ApplicationServices, LogLevel.Warning);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
