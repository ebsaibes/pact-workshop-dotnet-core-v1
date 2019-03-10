using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tests.Middleware;
using Microsoft.AspNetCore.Hosting;



namespace tests
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration){
            Configuration = configuration;
        }

        public IConfiguration Configuration{get;}

        /// <summary>
        /// Called by runtime env.!-- Use this method to configure the HTTP Request Pipeline
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services){
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder application, IHostingEnvironment env){
            application.UseMiddleware<ProviderStateMiddleware>();
            application.UseMvc();
        }

    }
}