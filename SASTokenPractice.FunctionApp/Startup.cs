// using System;
// using Microsoft.Azure.Functions.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection;
// using SASTokenPractice.FunctionApp;
// using SASTokenPractice.FunctionApp.AccessTokens;
//
// [assembly: FunctionsStartup(typeof(Startup))]
// namespace SASTokenPractice.FunctionApp
// {
//     /// <summary>
//     /// Runs when the Azure Functions host starts.
//     /// </summary>
//     public class Startup : FunctionsStartup
//     {
//         public override void Configure(IFunctionsHostBuilder builder)
//         {
//             // Get the configuration files for the OAuth token issuer
//             var issuerToken = Environment.GetEnvironmentVariable("IssuerToken");
//             var audience = Environment.GetEnvironmentVariable("Audience");
//             var issuer = Environment.GetEnvironmentVariable("Issuer");
//
//             // Register the access token provider as a singleton
//             builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>(s =>
//                 new AccessTokenProvider(issuerToken, audience, issuer));
//         }
//     }
// }

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using SASTokenPractice.FunctionApp;
using SASTokenPractice.FunctionApp.OidcApiAuthorization;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SASTokenPractice.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOidcApiAuthorization();
        }
    }
}