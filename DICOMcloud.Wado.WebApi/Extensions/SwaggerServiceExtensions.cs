using DICOMcloud.Wado.WebApi.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DICOMcloud.Wado.WebApi.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(o =>
            {
                o.CustomSchemaIds(x => x.FullName);

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "In the value text box type \"Bearer \" and then paste the token.",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    o.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, configuration));
                }

                // add a custom operation filter which sets default values
                o.OperationFilter<SwaggerDefaultValues>();
            });

            return services;
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, IConfiguration config)
        {
            var title = config["Service:Title"];
            var info = new OpenApiInfo()
            {
                Title = $"{title} {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = config["Service:Description"],
                Contact = new OpenApiContact()
                {
                    Name = config["Service:Contact:Name"],
                    Email = config["Service:Contact:Email"]
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}