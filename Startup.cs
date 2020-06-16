using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sors.Authentication;
using sors.Data;
using sors.Helpers;
using System;
using System.Net;
using System.Net.Mail;

namespace sors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }

        public static IConfiguration StaticConfig { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x
                .UseSqlServer(Configuration.GetConnectionString("LostZoneLocal"))
                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning)));

            services.Configure<IISOptions>(options => { options.AutomaticAuthentication = true; });
            services.Configure<ShaderOptions>(Configuration);
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUser", policy => policy.RequireRole("admin", "riskCoordinator", "riskManager", "user"));
                options.AddPolicy("RequireCoordinator", policy => policy.RequireRole("admin", "riskCoordinator", "riskManager"));
                options.AddPolicy("RequireRM", policy => policy.RequireRole("admin", "riskManager"));
                options.AddPolicy("RequireSecurity", policy => policy.RequireRole("admin", "security"));
                options.AddPolicy("RequireAdmin", policy => policy.RequireRole("admin"));
            });
            services.AddMvc().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // надо бы разобраться в этом: =>
                opt.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            });
            services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
            //services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddCors();
            services.AddScoped<SmtpClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                NetworkCredential nc = CredentialCache.DefaultNetworkCredentials;
                return new SmtpClient()
                {
                    Host = config.GetValue<String>("Email:Smtp:Host"),
                    Port = config.GetValue<int>("Email:Smtp:Port"),
                    UseDefaultCredentials = true,
                    Credentials = (System.Net.ICredentialsByHost)nc.GetCredential(
                        config.GetValue<String>("Email:Smtp:Host"),
                        config.GetValue<int>("Email:Smtp:Port"),
                        "Basic")
                };
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<Seed>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Seed seeder)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseDeveloperExceptionPage();
            //app.UseAuthentication();
            //app.UseCors(x => x.WithOrigins("http://localhost:4200")
            //    .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            //app.UseMvc();
            seeder.SeedRoles();

            app.UseCors(x => x.WithOrigins("http://localhost:4200")
                .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseMvc();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Fallback}/{action=Index}/{id?}");

                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
