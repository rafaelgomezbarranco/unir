using Microsoft.OpenApi.Models;

namespace NotificationWebApi.Modules.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerGenWithDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(_ =>
        {
            // Created the Swagger document
            _.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "Version 1.0.0",
                Title = "Notification Api",
                Description = " This API offers you the ability to send WhatsApp or SMS messages",
                TermsOfService = new Uri("https://mexico.unir.net/estudios/condiciones.htm"),
                License = new OpenApiLicense
                {
                    Name = "Professional licence.",
                    Url = new Uri("https://example.com/license")
                }
            });


            // Generate the swagger documentation
            foreach (var name in Directory.GetFiles(AppContext.BaseDirectory, "*.XML", SearchOption.TopDirectoryOnly))
                _.IncludeXmlComments(name);
        });

        return services;
    }
}