using Microsoft.Azure.Management.ResourceManager;
using RM = Microsoft.Azure.Management.ResourceManager.Models;
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
    public class AzureStorageService : IAzureStorageService
    {
        string clientId = string.Empty;
        string clientSecretKey = string.Empty;
        string tenantId = string.Empty;
        string resourceGroupName = string.Empty;
        string DefaultLocation = string.Empty;
        string authURL = string.Empty;
        string azureManagementURL = string.Empty;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageRepo _azureStorageRepo;

        public AzureStorageService(IConfiguration configuration, IAzureStorageRepo azureStorageRepo)
        {
            _configuration = configuration;
            _azureStorageRepo = azureStorageRepo;
            clientId = _configuration.GetSection("AppSettings:clientId").Value;
            clientSecretKey = _configuration.GetSection("AppSettings:clientSecretKey").Value;
            tenantId = _configuration.GetSection("AppSettings:tenantId").Value;
            resourceGroupName = _configuration.GetSection("AppSettings:resourceGroupName").Value;
            authURL= _configuration.GetSection("AppSettings:authURL").Value;
            azureManagementURL= _configuration.GetSection("AppSettings:azureManagementURL").Value;
            DefaultLocation = _configuration.GetSection("AzureStorageSettings:DefaultLocation").Value;
        }

        public async Task<string> GetAuthorizationHeader()
        {
            ClientCredential cc = new ClientCredential(clientId, clientSecretKey);
            var context = new AuthenticationContext(authURL + tenantId);
            var result = await context.AcquireTokenAsync(azureManagementURL, cc);

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
            IList<DMSServiceInfo> dmsServiceList = new List<DMSServiceInfo>();
            try
            {
                dmsServiceList = await _azureStorageRepo.GetDMSServiceInfo().ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                ExceptionLog log = new ExceptionLog();
                log.DMSServiceInfoId = 0;
                log.InnerException = ex.InnerException.ToString();
                log.StackTrace = ex.StackTrace.ToString();
                log.ExceptionMessage = ex.Message.ToString();
                await _azureStorageRepo.AddExceptionLogs(log);
                throw ex;
            }
            StorageAccountCreateParameters parameters = GetDefaultStorageAccountParameters(_configuration);
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

                if (string.IsNullOrEmpty(service.Location))
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

                try
                {
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
                    IList<StorageAccountKey> accessKeys = GetKeysAccess(accountName, storageMgmtClient);
                    string connectionString = GetConnectionString(accessKeys[0].Value, accountName);

                    service.AzStorageConnectionString = connectionString;
                    service.AzStorageContainer = accountName;

                    int record = await _azureStorageRepo.UpdateDMSServiceInfo(service).ConfigureAwait(false);

                    StorageAccountModel storageModel = MapStorageAccountDetails(storageAccount);
                    storageModel.KeyAccessName = accessKeys[0].KeyName;
                    storageModel.KeyAccessValue = accessKeys[0].Value;
                    storageModel.DMSServiceInfoId = service.Id;
                    await _azureStorageRepo.UpdateStorageAccountDetails(storageModel);
                    strList.Add(storageAccount);
                }
                catch(Exception ex)
                {
                    ExceptionLog log = new ExceptionLog();
                    log.DMSServiceInfoId = service.Id;
                    log.InnerException = ex.InnerException.ToString();
                    log.StackTrace = ex.StackTrace.ToString();
                    log.ExceptionMessage = ex.Message.ToString();
                    await _azureStorageRepo.AddExceptionLogs(log);
                    throw ex;
                }
            }
            return strList;
        }

        private StorageAccountModel MapStorageAccountDetails(StorageAccount storageAccount)
        {
            StorageAccountModel model = new StorageAccountModel();
            model.AccessTier = storageAccount.AccessTier.ToString();
            model.AccountId = storageAccount.Id.ToString();
            model.MinimumTlsVersion = storageAccount.MinimumTlsVersion.ToString();
            model.AccountName = storageAccount.Name;
            model.AccountType = storageAccount.Type;
            model.AllowBlobPublicAccess = storageAccount.AllowBlobPublicAccess;
            model.CreationTime = storageAccount.CreationTime;
            model.EnableHttpsTrafficOnly = storageAccount.EnableHttpsTrafficOnly;
            model.IsHnsEnabled = storageAccount.IsHnsEnabled;
            model.KeySource = storageAccount.Encryption.KeySource.ToString();
            model.SkuName = storageAccount.Sku.Name.ToString();
            model.LargeFileSharesState = storageAccount.LargeFileSharesState;
            model.Kind = storageAccount.Kind;
            model.BlobEndPoints = storageAccount.PrimaryEndpoints.Blob.ToString();
            model.TableEndPoints = storageAccount.PrimaryEndpoints.Table.ToString();
            model.FileEndPoints = storageAccount.PrimaryEndpoints.File.ToString();
            model.QueueEndPoints = storageAccount.PrimaryEndpoints.Queue.ToString();
            model.PrimaryLocation = storageAccount.PrimaryLocation.ToString();
            model.Location = storageAccount.Location.ToString();
            foreach (var tag in storageAccount.Tags)
            {
                model.TagKey = tag.Key;
                model.TagValue = tag.Value;
            }

            return model;

        }

        public IList<StorageAccountKey> GetKeysAccess(string accountName, StorageManagementClient storageMgmtClient)
        {
            IList<StorageAccountKey> acctKeys = storageMgmtClient.StorageAccounts.ListKeys(resourceGroupName, accountName).Keys;
            return acctKeys;
        }

        public string GetConnectionString(string key, string accountName)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=myAccountName;AccountKey=myAccountKey;EndpointSuffix=mySuffix;";
            connectionString = connectionString.Replace("myAccountName", accountName).Replace("myAccountKey", key).Replace("mySuffix", "core.windows.net").Replace(" ","");
            return connectionString;
        }

        public async Task DeleteStorageAccount(string accountName, StorageManagementClient storageMgmtClient)
        {
            DMSServiceInfo serviceInfo= await _azureStorageRepo.GetDMSServiceInfoByAccountName(accountName).ConfigureAwait(false);
            await storageMgmtClient.StorageAccounts.DeleteAsync(resourceGroupName, accountName);
            serviceInfo.IsDeleted = true;
            await _azureStorageRepo.UpdateDMSServiceInfo(serviceInfo);

        }
        private static StorageAccountCreateParameters GetDefaultStorageAccountParameters(IConfiguration config)
        {
            StorageAccountCreateParameters account = new StorageAccountCreateParameters
            {
                Kind = config.GetSection("AzureStorageSettings:Kind").Value,
                AccessTier = config.GetSection("AzureStorageSettings:AccessTier").Value == "0" ? AccessTier.Hot : AccessTier.Cool,
                MinimumTlsVersion = config.GetSection("AzureStorageSettings:MinimumTlsVersion").Value,
                AllowBlobPublicAccess = Boolean.Parse(config.GetSection("AzureStorageSettings:AllowBlobPublicAccess").Value),
                EnableHttpsTrafficOnly = Boolean.Parse(config.GetSection("AzureStorageSettings:EnableHttpsTrafficOnly").Value),
                LargeFileSharesState = config.GetSection("AzureStorageSettings:LargeFileSharesState").Value
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
