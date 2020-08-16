using System.Text;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace AzureBackend
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Environment = environment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.Configure<DbConnectionString>(Configuration.GetSection("krazurebackend_db_mssql"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    int userId = 0;
                    if (!Int32.TryParse(context.Request.Query["Id"], out userId))
                    {
                        userId = -1;
                    }

                    DbConnectionString DomainConnectionString = new DbConnectionString();
                    DomainConnectionString.DomainConnectionString = Configuration.GetSection("krazurebackend_db_mssql:domainConnectionString").Value;
                    var usersRepository = new UsersRepository(DomainConnectionString);
                    User User = usersRepository.GetUserById(userId);
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(User), Encoding.UTF8);
                });
            });
        }
    }
}
