using Microsoft.Azure.Management.ResourceManager;
using RM= Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure;
using AzureStorageAccountGenerator.Repository;
using AzureStorageAccountGenerator.Models;

namespace AzureStorageAccountGenerator.Services
{
    public class AzureStorageService: IAzureStorageService
    {
        string clientId = string.Empty;
        string clientSecretKey = string.Empty;
        string tenantId = string.Empty;
        string resourceGroupName = string.Empty;
        string DefaultLocation = string.Empty;
        
        //public static Dictionary<string, string> DefaultTags = new Dictionary<string, string>
        //{
        //    {"key123","value123"},
        //    {"key234","value234"}
        //};
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageRepo _azureStorageRepo;

        public AzureStorageService(IConfiguration configuration, IAzureStorageRepo azureStorageRepo)
        {
            _configuration = configuration;
            _azureStorageRepo = azureStorageRepo;
            clientId = _configuration.GetSection("AppSettings:clientId").Value;
            clientSecretKey = _configuration.GetSection("AppSettings:clientSecretKey").Value;
            tenantId = _configuration.GetSection("AppSettings:tenantId").Value;
            resourceGroupName= _configuration.GetSection("AppSettings:resourceGroupName").Value;
            DefaultLocation = _configuration.GetSection("AzureStorageSettings:DefaultLocation").Value;
        }

        public async Task<string> GetAuthorizationHeader()
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

        public async Task<IPage<StorageAccount>> GetAllStorageAccount(StorageManagementClient storageMgmtClient)
        {
               return await storageMgmtClient.StorageAccounts.ListAsync();
            
        }

        public async Task<List<StorageAccount>> CreateStorageAccount(StorageManagementClient storageMgmtClient)
        {
            IList<DMSServiceInfo> dmsServiceList = await _azureStorageRepo.GetDMSServiceInfo().ConfigureAwait(false);
            StorageAccountCreateParameters parameters = GetDefaultStorageAccountParameters();
            List<StorageAccount> strList = new List<StorageAccount>();
            foreach (var service in dmsServiceList)
            {
                var uri = new Uri(service.DMSTenantURL);
                var sURL = uri.Host;
                string[] names = sURL.Split('.');
                string accountName = service.DatabaseName + names[0];
                if (service.IsReplication)
                {
                    parameters.Sku = new Sku(SkuName.StandardGRS);
                }
                else
                {
                    parameters.Sku = new Sku(SkuName.StandardLRS);
                }

                if(string.IsNullOrEmpty(service.Location))
                {
                    parameters.Location = DefaultLocation;
                }
                else
                {
                    parameters.Location = service.Location;
                }

                IDictionary<string, string> tags = new Dictionary<string, string>();
                tags.Add(accountName, accountName);
                parameters.Tags = tags;

                var storageAccount = await storageMgmtClient.StorageAccounts.CreateAsync(resourceGroupName, accountName, parameters);
                await storageMgmtClient.BlobContainers.CreateAsync(resourceGroupName, accountName, accountName, GetBlobContainer());

                BlobServiceProperties blobProperties = new BlobServiceProperties
                {
                    DeleteRetentionPolicy = new DeleteRetentionPolicy
                    {
                        Enabled = true,
                        Days = 7
                    }
                };

                storageMgmtClient.BlobServices.SetServiceProperties(resourceGroupName, accountName, blobProperties);
                IList<StorageAccountKey> keys= GetKeysAccess(accountName, storageMgmtClient);
                string connectionString = GetConnectionString(keys[0].Value, accountName);

                service.AzStorageConnectionString = connectionString;
                service.AzStorageContainer = accountName;
                
                int record= await _azureStorageRepo.UpdateAZConnectionStringandContainer(service).ConfigureAwait(false);
                strList.Add(storageAccount);
            }
            return strList;
        }

        public async Task<StorageAccount> GetProperties(string accountName, StorageManagementClient storageMgmtClient)
        {
            var storAcct = await storageMgmtClient.StorageAccounts.GetPropertiesAsync(resourceGroupName, accountName);
            return storAcct;
        }

        public IList<StorageAccountKey> GetKeysAccess(string accountName, StorageManagementClient storageMgmtClient)
        {
            IList<StorageAccountKey> acctKeys = storageMgmtClient.StorageAccounts.ListKeys(resourceGroupName, accountName).Keys;
            return acctKeys;
        }

        public string GetConnectionString(string key, string accountName)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName = myAccountName;AccountKey = myAccountKey;EndpointSuffix = mySuffix;";
            connectionString= connectionString.Replace("myAccountName", accountName).Replace("myAccountKey", key).Replace("mySuffix", "core.windows.net");
            return connectionString;
        }

        /// <summary>
        /// Deletes a storage account for the specified account name
        /// </summary>
        /// <param name="rgname"></param>
        /// <param name="acctName"></param>
        /// <param name="storageMgmtClient"></param>
        public async Task DeleteStorageAccount(string accountName, StorageManagementClient storageMgmtClient)
        {
           await storageMgmtClient.StorageAccounts.DeleteAsync(resourceGroupName, accountName);
         
        }

        /// <summary>
        /// Updates the storage account
        /// </summary>
        /// <param name="rgname">Resource Group Name</param>
        /// <param name="acctName">Account Name</param>
        /// <param name="storageMgmtClient"></param>
        public async Task<StorageAccount> UpdateStorageAccountSku(string updatedName, string accountName, StorageManagementClient storageMgmtClient)
        {
            var parameters = new StorageAccountUpdateParameters
            {
                Sku = new Sku(updatedName)
            };
            var storageAccount = await storageMgmtClient.StorageAccounts.UpdateAsync(resourceGroupName, accountName, parameters);

            return storageAccount;
        }

        /// <summary>
        /// Returns default values to create a storage account
        /// </summary>
        /// <returns>The parameters to provide for the account</returns>
        private static StorageAccountCreateParameters GetDefaultStorageAccountParameters()
        {
            StorageAccountCreateParameters account = new StorageAccountCreateParameters
            {
                Kind = Kind.StorageV2,
                AccessTier = AccessTier.Hot,
                MinimumTlsVersion = MinimumTlsVersion.TLS12,
                AllowBlobPublicAccess = true,
                EnableHttpsTrafficOnly = true,
                LargeFileSharesState = LargeFileSharesState.Disabled
            };

            return account;
        }


        public async Task RegisterStorageResourceProvider(ResourceManagementClient resourcesClient)
        {
            await resourcesClient.Providers.RegisterAsync("Microsoft.Storage");
        }

        /// <summary>
        /// Creates a new resource group with the specified name
        /// If one already exists then it gets updated
        /// </summary>
        /// <param name="resourcesClient"></param>
        public async Task<RM.ResourceGroup> CreateResourceGroup(string rgname, ResourceManagementClient resourcesClient)
        {
            var resourceGroup = await resourcesClient.ResourceGroups.CreateOrUpdateAsync(
                    rgname,
                    new RM.ResourceGroup
                    {
                        Location = DefaultLocation
                    });
            return resourceGroup;
        }

        /// <summary>
        /// Create a new Storage Account. If one already exists then the request still succeeds
        /// </summary>
        /// <param name="rgname">Resource Group Name</param>
        /// <param name="acctName">Account Name</param>
        /// <param name="useCoolStorage">Use Cool Storage</param>
        /// <param name="useEncryption">Use Encryption</param>
        /// <param name="storageMgmtClient">Storage Management Client</param>

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
