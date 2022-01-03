using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hamwi.Shared;
using Hamwi.Shared.Extensions.Startup;
using Hamwi.Shared.Services.Client;
using Hamwi.Shared.Services.Client.Interfaces;

namespace Hamwi.MVC
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(IHamwiServices), typeof(HamwiServices));
            services.AddRequiredServices(_configuration);
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<HamwiContext>();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/Error/Index");
            app.UseStatusCodePagesWithReExecute("/Error/PageNotFound");
            app.UseRouting();
            app.UseFileServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}