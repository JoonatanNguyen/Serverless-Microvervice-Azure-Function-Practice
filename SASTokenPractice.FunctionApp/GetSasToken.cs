using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using SASTokenPractice.FunctionApp.OidcApiAuthorization.Abstractions;
using SASTokenPractice.FunctionApp.OidcApiAuthorization.Models;

namespace SASTokenPractice.FunctionApp
{
    public class GetSasToken
    {
        private readonly IApiAuthorization _apiAuthorization;
        public GetSasToken(IApiAuthorization apiAuthorization)
        {
            _apiAuthorization = apiAuthorization;
        }
        
        [FunctionName("GetSasToken")]
        public async Task<IActionResult> GenerateSasToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogWarning($"HTTP trigger function {nameof(GetSasToken)} received a request.");

            ApiAuthorizationResult authorizationResult = await _apiAuthorization.AuthorizeAsync(req.Headers);
            if (authorizationResult.Failed)
            {
                log.LogWarning(authorizationResult.FailureReason);
                return new UnauthorizedResult();
            }
            log.LogWarning($"HTTP trigger function {nameof(GetSasToken)} request is authorized.");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ImageData imageData = JsonConvert.DeserializeObject<ImageData>(requestBody);
            
            CloudStorageAccount storageAccount = GetCloudStorageAccount(context);
            var credentials = storageAccount.Credentials;
            var accountName = credentials.AccountName;
            var accountKey = credentials.ExportBase64EncodedKey();
            
            var fullFileName = GetFullFileName(imageData.SasAccessType);
            var blobNamePath = GetBlobNamePath(imageData.SasAccessType);
            var blobName = $"{blobNamePath}/{fullFileName}";
            
            // Create an instance of the CloudBlobClient
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            
            // Retrieve an instance of the container using hard-code container name
            CloudBlobContainer container = client.GetContainerReference("cdn");
            
            // Create a SAS token that's valid for one hour 
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                ExpiresOn = DateTime.UtcNow.AddHours(1),
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b"
            };
            
            // Specify permission for the SAS
            sasBuilder.SetPermissions(BlobSasPermissions.Create);
            
            // Use the key to get the SAS token 
            string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, accountKey))
                .ToString();
            
            var sasUri = $"{container.GetBlobReference(blobName).Uri}?{sasToken}";
            
            return new OkObjectResult(sasUri);
        }

        private static CloudStorageAccount GetCloudStorageAccount(ExecutionContext context)
        {
            var config = new ConfigurationBuilder()  
                .SetBasePath(context.FunctionAppDirectory)  
                .AddJsonFile("local.settings.json", true, true)  
                .AddEnvironmentVariables().Build();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["AzureWebJobsStorage"]);  
            return storageAccount;
        }

        private static string GetFullFileName(SasAccessType sasAccessType)
        {
            var fileName = Guid.NewGuid();
            string fileExtension;
            switch (sasAccessType)
            {
                case SasAccessType.Design:
                case SasAccessType.ProfileImage:
                    fileExtension = ".jpg";
                    break;
                case SasAccessType.Model:
                    fileExtension = ".glb";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sasAccessType), sasAccessType, null);
            }

            return $"{fileName}{fileExtension}";
        }
        
        private static string GetBlobNamePath(SasAccessType sasAccessType)
        {
            var blobNamePath = "";
            switch (sasAccessType)
            {
                case SasAccessType.Design:
                    blobNamePath = "images";
                    break;
                case SasAccessType.Model:
                    blobNamePath = "models";
                    break;
                case SasAccessType.ProfileImage:
                    blobNamePath =  "profile_images";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sasAccessType), sasAccessType, null);
            }

            return blobNamePath;
        }

        public bool ValidateToken(HttpRequest request)
        {
            
            return true;
        }

        public enum SasAccessType
        {
            Design, 
            Model, 
            ProfileImage
        }
    }
}