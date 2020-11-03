using Microsoft.IdentityModel.Tokens;

namespace SASTokenPractice.FunctionApp.OidcApiAuthorization.Abstractions
{
    public interface IJwtSecurityTokenHandlerWrapper
    {
        void ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }
}