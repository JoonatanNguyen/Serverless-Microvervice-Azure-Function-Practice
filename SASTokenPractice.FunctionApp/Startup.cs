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