using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace AzureStorageAccountGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateStorageAccount : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string clientId = string.Empty;
        string clientSecretKey = string.Empty;
        string tenantId = string.Empty;
        string subscriptionId = string.Empty;
        string resourceGroupName = "TestResource";
        string accountName = "10010maresemmc2";
        const string DefaultLocation = "westus";
        public static Sku DefaultSku = new Sku(SkuName.StandardLRS);
        public static Dictionary<string, string> DefaultTags = new Dictionary<string, string>
        {
            {"key123","value123"},
            {"key234","value234"}
        };
        public GenerateStorageAccount(IConfiguration configuration)
        {
            _configuration = configuration;
            clientId = _configuration.GetSection("AppSettings:clientId").Value;
            clientSecretKey = _configuration.GetSection("AppSettings:clientSecretKey").Value;
            tenantId = _configuration.GetSection("AppSettings:tenantId").Value;
            subscriptionId = _configuration.GetSection("AppSettings:subscriptionId").Value;
        }

        private async Task<string> GetAuthorizationHeader()
        {
            ClientCredential cc = new ClientCredential(clientId, clientSecretKey);
            var context = new AuthenticationContext("https://login.windows.net/" + tenantId);
            var result = await context.AcquireTokenAsync("https://management.azure.com/", cc);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;

            return token;
        }

        [HttpGet("GetStatus")]
        //[EnableQuery()]
        public async Task<IActionResult> GetStatus()
        {
            string token = GetAuthorizationHeader().Result;
            TokenCredentials credential = new TokenCredentials(token);
            ResourceManagementClient resourcesClient = new ResourceManagementClient(credential) { SubscriptionId = subscriptionId };
            StorageManagementClient storageMgmtClient = new StorageManagementClient(credential) { SubscriptionId = subscriptionId };
            
            StorageAccount sa = new StorageAccount();
            try
            {
                //Register the Storage Resource Provider with the Subscription
                RegisterStorageResourceProvider(resourcesClient);

                //Create a new resource group
                //CreateResourceGroup(rgName, resourcesClient);

                //Create a new account in a specific resource group with the specified account name                     
                sa= CreateStorageAccount(resourceGroupName, accountName, storageMgmtClient);

                await storageMgmtClient.BlobContainers.CreateAsync(resourceGroupName, accountName, accountName, GetBlobContainer());

                BlobServiceProperties blobProperties = new BlobServiceProperties
                {
                    DeleteRetentionPolicy= new DeleteRetentionPolicy
                    {
                        Enabled= true,
                        Days= 7
                    }
                };
                
                storageMgmtClient.BlobServices.SetServiceProperties(resourceGroupName, accountName, blobProperties);

                StorageAccount storAcct = storageMgmtClient.StorageAccounts.GetProperties(resourceGroupName, accountName);

                IList<StorageAccountKey> acctKeys = storageMgmtClient.StorageAccounts.ListKeys(resourceGroupName, accountName).Keys;

            }
            catch (Exception e)
            {
                throw e;
            }
            return Ok(sa);
        }

        /// <summary>
        /// Registers the Storage Resource Provider in the subscription.
        /// </summary>
        /// <param name="resourcesClient"></param>
        public static void RegisterStorageResourceProvider(ResourceManagementClient resourcesClient)
        {
            //Console.WriteLine("Registering Storage Resource Provider with subscription...");
            resourcesClient.Providers.Register("Microsoft.Storage");
            //Console.WriteLine("Storage Resource Provider registered.");
        }

        /// <summary>
        /// Creates a new resource group with the specified name
        /// If one already exists then it gets updated
        /// </summary>
        /// <param name="resourcesClient"></param>
        public static void CreateResourceGroup(string rgname, ResourceManagementClient resourcesClient)
        {
            Console.WriteLine("Creating a resource group...");
            var resourceGroup = resourcesClient.ResourceGroups.CreateOrUpdate(
                    rgname,
                    new Microsoft.Azure.Management.ResourceManager.Models.ResourceGroup
                    {
                        Location = DefaultLocation
                    });
            Console.WriteLine("Resource group created with name " + resourceGroup.Name);

        }

        /// <summary>
        /// Create a new Storage Account. If one already exists then the request still succeeds
        /// </summary>
        /// <param name="rgname">Resource Group Name</param>
        /// <param name="acctName">Account Name</param>
        /// <param name="useCoolStorage">Use Cool Storage</param>
        /// <param name="useEncryption">Use Encryption</param>
        /// <param name="storageMgmtClient">Storage Management Client</param>
        private static StorageAccount CreateStorageAccount(string rgname, string acctName, StorageManagementClient storageMgmtClient)
        {
            StorageAccountCreateParameters parameters = GetDefaultStorageAccountParameters();

            //Console.WriteLine("Creating a storage account...");
            var storageAccount = storageMgmtClient.StorageAccounts.Create(rgname, acctName, parameters);
            //Console.WriteLine("Storage account created with name " + storageAccount.Name);
            return storageAccount;
        }

        /// <summary>
        /// Deletes a storage account for the specified account name
        /// </summary>
        /// <param name="rgname"></param>
        /// <param name="acctName"></param>
        /// <param name="storageMgmtClient"></param>
        private static void DeleteStorageAccount(string rgname, string acctName, StorageManagementClient storageMgmtClient)
        {
            Console.WriteLine("Deleting a storage account...");
            storageMgmtClient.StorageAccounts.Delete(rgname, acctName);
            Console.WriteLine("Storage account " + acctName + " deleted");
        }

        /// <summary>
        /// Updates the storage account
        /// </summary>
        /// <param name="rgname">Resource Group Name</param>
        /// <param name="acctName">Account Name</param>
        /// <param name="storageMgmtClient"></param>
        private static void UpdateStorageAccountSku(string rgname, string acctName, string skuName, StorageManagementClient storageMgmtClient)
        {
            Console.WriteLine("Updating storage account...");
            // Update storage account sku
            var parameters = new StorageAccountUpdateParameters
            {
                Sku = new Microsoft.Azure.Management.Storage.Models.Sku(skuName)
            };
            var storageAccount = storageMgmtClient.StorageAccounts.Update(rgname, acctName, parameters);
            Console.WriteLine("Sku on storage account updated to " + storageAccount.Sku.Name);
        }

        /// <summary>
        /// Returns default values to create a storage account
        /// </summary>
        /// <returns>The parameters to provide for the account</returns>
        private static StorageAccountCreateParameters GetDefaultStorageAccountParameters()
        {
            StorageAccountCreateParameters account = new StorageAccountCreateParameters
            {
                Location = DefaultLocation,
                Kind = Kind.StorageV2,
                Tags = DefaultTags,
                Sku = DefaultSku,
                AccessTier= AccessTier.Hot,
                MinimumTlsVersion= MinimumTlsVersion.TLS12,
                AllowBlobPublicAccess=true,
                EnableHttpsTrafficOnly=true,
                LargeFileSharesState= LargeFileSharesState.Disabled
            };

            return account;
        }

        private static BlobContainer GetBlobContainer()
        {
            BlobContainer container = new BlobContainer
            {
                PublicAccess = PublicAccess.Blob
            };

            return container;
        }
    }
}