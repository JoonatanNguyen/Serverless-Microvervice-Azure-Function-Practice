using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SASTokenPractice.FunctionApp.OidcApiAuthorization.Models;

namespace SASTokenPractice.FunctionApp.OidcApiAuthorization.Abstractions
{
    public interface IApiAuthorization
    {
        Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders);

        Task<HealthCheckResult> HealthCheckAsync();
    }
}