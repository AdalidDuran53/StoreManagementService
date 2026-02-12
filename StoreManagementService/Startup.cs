using ExceptionsManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StoreManagementService.BusinessLogic;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StoreManagementService.FilterProvider.ExamplesProvider;

namespace StoreManagementService
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
            services.AddControllers();
            // register swagger generator
            services.AddSwaggerGen();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "StoreManagementService",
                    Version = "v1",
                    Description = "API para gestion de inventarios"
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.ExampleFilters();
            });

            services.AddSwaggerExamplesFromAssemblyOf<CustomResponseCreatedExample>();
            services.AddSwaggerExamplesFromAssemblyOf<CustomResponseBadRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<CustomResponseOKExample>();

            // Configure API versioning
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.WithOrigins("*")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });



            services.AddScoped<ServiceBaseFunctionality>();
            services.AddScoped<ClientFunctionality>();
            services.AddScoped<ErrorServiceModel>();
            services.AddScoped<StoreFunctionality>();
            services.AddScoped<ItemFunctionality>();
            services.AddScoped<ItemClientFunctionality>();
            services.AddScoped<ItemStoreFunctionality>();
            services.AddScoped<EmailVerifyFunctionality>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // redirect root to swagger
            app.Use(async (context, next) => {
                if (context.Request.Path == "/")
                {
                    // permanent redirect to swagger
                    context.Response.Redirect("/swagger/index.html", permanent: false);
                    return;
                }
                await next();
            });

            if (env.IsDevelopment())
            {
                // for secure implementation, disable swagger in production
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");
            app.UseAuthorization();

            // Enable API versioning
            app.UseApiVersioning();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
