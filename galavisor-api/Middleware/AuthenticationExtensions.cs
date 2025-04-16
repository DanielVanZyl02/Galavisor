using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GalavisorApi.Middleware;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddGoogleJwtAuthentication(this IServiceCollection Services)
    {
        Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Options =>
            {
                Options.Authority = "https://accounts.google.com";

                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
                    ValidateAudience = false
                };
                Options.MapInboundClaims = false;
            });

        return Services;
    }
}
