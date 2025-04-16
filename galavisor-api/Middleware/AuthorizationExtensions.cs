namespace GalavisorApi.Middleware;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddDefaultAuthorization(this IServiceCollection Services)
    {
        Services.AddAuthorization(Options =>
        {
            // Require authenticated user by default
            Options.FallbackPolicy = Options.DefaultPolicy;
        });

        return Services;
    }
}
