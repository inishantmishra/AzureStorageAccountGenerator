using AzureStorageAccountGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAccountGenerator.Repository
{
   public interface IAzureStorageRepo
    {
        Task<IList<DMSServiceInfo>> GetDMSServiceInfo();
        Task<int> UpdateDMSServiceInfo(DMSServiceInfo serviceInfo);
        Task<DMSServiceInfo> GetDMSServiceInfoByAccountName(string accountName);
        Task<int> UpdateStorageAccountDetails(StorageAccountModel storageModel);
        Task<int> AddExceptionLogs(ExceptionLog log);
    }
}
