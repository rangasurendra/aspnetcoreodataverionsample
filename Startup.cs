using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using static Microsoft.AspNetCore.Mvc.CompatibilityVersion;
using static Microsoft.OData.ODataUrlKeyDelimiter;

namespace WebODataDataService
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
            // services.AddControllers();

            services
            .AddMvc(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(j =>
                {
                    j.JsonSerializerOptions.PropertyNamingPolicy = null;
                    j.JsonSerializerOptions.DictionaryKeyPolicy = null;
                })
            .SetCompatibilityVersion(Latest);


            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);

            });



            services.AddOData().EnableApiVersioning();
            services.AddODataApiExplorer(
            options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;

                // configure query options (which cannot otherwise be configured by OData conventions)

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, VersionedODataModelBuilder modelBuilder)
        {
            app.UseMvc(
                routeBuilder =>
                {
                    routeBuilder.EnableDependencyInjection();

                    // the following will not work as expected
                    // BUG: https://github.com/OData/WebApi/issues/1837
                    // routeBuilder.SetDefaultODataOptions( new ODataOptions() { UrlKeyDelimiter = Parentheses } );
                    routeBuilder.ServiceProvider.GetRequiredService<ODataOptions>().UrlKeyDelimiter = Parentheses;

                    // global odata query options
                    routeBuilder.Count();

                    routeBuilder.MapVersionedODataRoutes("odata", "api", modelBuilder.GetEdmModels());
                });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();



            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers();

            //     endpoints.EnableDependencyInjection();

            // });
        }
    }
}
