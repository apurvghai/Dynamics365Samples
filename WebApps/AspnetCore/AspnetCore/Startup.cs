using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspnetCore.ObjectModel.Class;
using AspnetCore.ObjectModel.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AspnetCore
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie().AddOpenIdConnect("oidc", options => SetOpenIdConnection(options));


            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true);
            var config = configBuilder.Build();
            services.Configure<ProjectConfiguration>(config);

            //Enable Dependency
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ProjectConfiguration>();
            services.AddSingleton<IConnector, Connector>();
            services.AddSingleton<IThirdPartyConnector, ThirdPartyConnector>();
        }

        public void SetOpenIdConnection(OpenIdConnectOptions options)
        {
            options.SaveTokens = true;
            options.RequireHttpsMetadata = false;
            options.ClientId = Configuration["OpenIdConnect:ClientId"];
            options.ClientSecret = Configuration["OpenIdConnect:ClientSecret"];
            options.Authority = Configuration["OpenIdConnect:Authority"];
            options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
            options.CallbackPath = "/signin-oidc";
            options.UseTokenLifetime = true;
            options.MetadataAddress = $"{Configuration["OpenIdConnect:Authority"]}/{Configuration["OpenIdConnect:TenantId"]}/.well-known/openid-configuration";
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SignInScheme = "Cookies";
            options.ResponseType = OpenIdConnectResponseType.IdToken;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                AuthenticationType = "Cookies",
                ValidateIssuer = false
            };

            // Scopes needed by application
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("roles");

            options.Events = new OpenIdConnectEvents()
            {
                OnAuthorizationCodeReceived = AuthorizationCodeReceived
            };
        }

        private Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            var code = context;
            return Task.FromResult(0);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
