using System;
using System.IO;
using System.Linq;
using System.Text;
using SBPWebApi.Configuration;
using SBPWebApi.Middleware;
using SBPWebApi.Services;
using SBPWebApi.SwaggerExamples;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace SBPWebApi
{
    /// <summary>
    /// OWIN configuration and setup.
    /// </summary>
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// Initializes new instance of <see cref="Startup"/>
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .SetBasePath(_env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));

            services.AddLogging();
            services.AddTransient<ISBPService, SBPService>();
            void build(CorsPolicyBuilder b) { b.WithOrigins("*").WithMethods("*").WithHeaders("*").AllowCredentials().Build(); };
            services.AddCors(options => { options.AddPolicy("AllowAllPolicy", build); });

            services
                .AddMvc(
                    options =>
                    {
                        options.CacheProfiles.Add(
                            "default",
                            new CacheProfile
                            {
                                Duration = 600,
                                Location = ResponseCacheLocation.None
                            });

                        //var jsonInputFormatter = options.InputFormatters.OfType<JsonInputFormatter>().First();
                        //jsonInputFormatter.SupportedMediaTypes.Add("multipart/form-data");
                    })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            services.AddResponseCaching();

            services.AddSwaggerExamplesFromAssemblyOf<PlanningExample>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "SPB-WebApi",
                    Description = "API pour l'application SPB",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Damian DINIC", Email = "damian.dinic@gmail.com", Url = "" }
                });

                c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{PlatformServices.Default.Application.ApplicationName}.xml"));
                c.DescribeAllEnumsAsStrings();

                c.ExampleFilters();
                c.OperationFilter<AddFileParamTypesOperationFilter>();
            });

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(
                    async context =>
                    {
                        var loggerFactory = app.ApplicationServices.GetService<ILogger>();

                        context.Request.EnableRewind();
                        context.Request.Body.Position = 0;

                        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                        {
                            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
                            var exceptionHandler = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

                            loggerFactory.LogError(exceptionHandler.Error, "Error: {0}. Request: {1}", exceptionHandler.Error.Message, body);
                        }
                    });
            });

            app.UseMiddleware<RequestValidationMiddleware>();
            app.UseCors("AllowAllPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
