using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAccountGenerator.Services
{
    public interface IAzureStorageService
    {
        Task<string> GetAuthorizationHeader();
        Task<IPage<StorageAccount>> GetAllStorageAccount(StorageManagementClient storageMgmtClient);
        Task<List<StorageAccount>> CreateStorageAccount(StorageManagementClient storageMgmtClient);
        Task DeleteStorageAccount(string acctName, StorageManagementClient storageMgmtClient);

        Task<StorageAccount> UpdateStorageAccountSku(string updatedName, string accountName, StorageManagementClient storageMgmtClient);

        Task RegisterStorageResourceProvider(ResourceManagementClient resourcesClient);

        Task<ResourceGroup?> CreateResourceGroup(string rgname, ResourceManagementClient resourcesClient);
        Task<StorageAccount> GetProperties(string accountName, StorageManagementClient storageMgmtClient);
        IList<StorageAccountKey> GetKeysAccess(string accountName, StorageManagementClient storageMgmtClient);

        string GetConnectionString(string key, string accountName);
    }
}
