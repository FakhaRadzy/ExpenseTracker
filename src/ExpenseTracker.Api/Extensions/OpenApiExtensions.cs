using Microsoft.OpenApi;

namespace ExpenseTracker.Api.Extensions
{
    public static class OpenApiExtensions
    {
        // Adds a "Bearer" security scheme to the OpenAPI document so Scalar shows a token field
        public static IServiceCollection AddOpenApiWithBearerAuth(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Paste the accessToken from /api//auth/login",
                    };

                    document.Security =[ new OpenApiSecurityRequirement {
                            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                        }
                    ];

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}
