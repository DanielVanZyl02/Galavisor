namespace GalavisorApi.Middleware;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddDefaultAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Require authenticated user by default
            options.FallbackPolicy = options.DefaultPolicy;
        });

        return services;
    }
}
