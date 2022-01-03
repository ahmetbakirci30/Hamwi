using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hamwi.Shared.Extensions.Startup;

namespace Hamwi.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRequiredServices(_configuration);
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.SeedDataAsync(_configuration).Wait();
            app.UseRouting();
            app.UseFileServer();
            app.UseCors(policy => policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}