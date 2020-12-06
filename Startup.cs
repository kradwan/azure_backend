using System.Text;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder =>
                {
                    builder
                    .AllowAnyOrigin() 
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowAll");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("users/", async context =>
                {
                    int userId = 0;
                    if (!Int32.TryParse(context.Request.Query["Id"], out userId))
                    {
                        userId = -1;
                    }

                    DbConnectionString DomainConnectionString = new DbConnectionString();
                    DomainConnectionString.DomainConnectionString = Configuration.GetSection("kr-azurebackend_db_mssql").Value;
                    var usersRepository = new UsersRepository(DomainConnectionString);
                    IList<User> users = new List<User>();
                    if(userId != -1)
                        users.Add(usersRepository.GetUserById(userId));
                    else
                      users = usersRepository.GetAllUsers();
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(users), Encoding.UTF8);
                });
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello in a test environment - Azure web app", Encoding.UTF8);
                });
            });
        }
    }
}
