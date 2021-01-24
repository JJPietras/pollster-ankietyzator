using System;
using System.Security.Claims;
using Ankietyzator.Services.Implementations;
using Ankietyzator.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Ankietyzator.Models
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IKeyService, KeyService>();
            services.AddScoped<IPollingService, PollingService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IStatService, StatService>();

            services.AddDbContext<AnkietyzatorDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("AnkietyzatorDBContextAzure"),
                        sqlOptions => sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null));
                }
            );

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/google/google-login";
                options.LogoutPath = "/google/google-logout";
                options.ReturnUrlParameter = "https://cc-2020-group-one-ankietyzator.azurewebsites.net/";
            }).AddGoogle(options =>
            {
                //IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");
                options.ClientId = "239751999427-njuioj9lb28dimdq02pn3gv1itqcd8be.apps.googleusercontent.com";//googleAuthNSection["ClientId"];
                options.ClientSecret = "jmwj5AIGrZNkia7-WTHmpd5E";//googleAuthNSection["ClientSecret"];
                options.SaveTokens = true;
                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.ReturnUrlParameter = "https://cc-2020-group-one-ankietyzator.azurewebsites.net/";
            });

            /*services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });*/

            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto
                });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}