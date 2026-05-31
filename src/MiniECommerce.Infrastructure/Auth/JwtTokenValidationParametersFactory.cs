using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MiniECommerce.Infrastructure.Auth;

public static class JwtTokenValidationParametersFactory
{
    public static TokenValidationParameters Create(JwtOptions options, bool validateLifetime = true)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = validateLifetime,
            ValidIssuer = options.Issuer,
            ValidAudience = options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }
}
