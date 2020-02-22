using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using CeloPracticalChallenge.API.Models;
using CeloPracticalChallenge.API.Repositories;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CeloPracticalChallenge.API
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
            services.AddControllers().AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                   new CamelCasePropertyNamesContractResolver();
            })
             // .AddXmlDataContractSerializerFormatters()
             .ConfigureApiBehaviorOptions(setupAction =>
             {
                 setupAction.InvalidModelStateResponseFactory = context =>
                 {
                     var problemDetails = new ValidationProblemDetails(context.ModelState) {
                         Type = "https://celopracticalchallenge.com/modelvalidationproblem",
                         Title = "One or more model validation errors occurred.",
                         Status = StatusCodes.Status422UnprocessableEntity,
                         Detail = "See the errors property for details.",
                         Instance = context.HttpContext.Request.Path
                     };

                     problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                     return new UnprocessableEntityObjectResult(problemDetails) {
                         ContentTypes = { "application/problem+json" }
                     };
                 };
             });

            services.Configure<MvcOptions>(config => {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                      .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null) {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }
            });

            services.AddDbContext<CeloPracticalChallengeDBContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("CeloPracticalChallengeContext"))
            );
                    
            services.AddScoped<IRandomUserRepository, RandomUserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
