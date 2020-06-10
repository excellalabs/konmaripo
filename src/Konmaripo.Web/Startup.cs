using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.Internal;

namespace Konmaripo.Web
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
            ConfigureHackyHttpsEnforcement(services);

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options =>
                {
                    Configuration.Bind("AzureAd", options);
                });

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddRazorPages();

            services.AddOptions();
            
            services.Configure<GitHubSettings>(Configuration.GetSection("GitHubSettings"));
            services.AddTransient(serviceProvider =>
            {
                var settings = serviceProvider.GetService<IOptions<GitHubSettings>>();
                var credentials = new Credentials(token: settings.Value.AccessToken);

                return new GitHubClient(new ProductHeaderValue("Konmaripo"),
                    new InMemoryCredentialStore(credentials));
            });

            services.AddSingleton<IGitHubService>(provider =>
            {
                var gitHubClient = provider.GetRequiredService<GitHubClient>();
                var githubSettings = provider.GetService<IOptions<GitHubSettings>>();

                var service = new GitHubService(gitHubClient, githubSettings);

                var cachedService = new CachedGitHubService(service, new MemoryCache(new MemoryCacheOptions()));

                return cachedService;
            });
        }

        /// <summary>
        /// This is a workaround due to issues running in a Linux container with no reverse proxy in front of it.
        /// In this situation, Azure AD authentication attempts to use a redirect URI that is http instead of https.
        /// For more information: https://github.com/excellalabs/konmaripo/issues/10
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureHackyHttpsEnforcement(IServiceCollection services)
        {
            // HACK
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                           ForwardedHeaders.XForwardedProto;
                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
