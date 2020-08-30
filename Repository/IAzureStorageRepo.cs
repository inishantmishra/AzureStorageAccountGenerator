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
        Task<int> UpdateAZConnectionStringandContainer(DMSServiceInfo serviceInfo);

        Task<DMSServiceInfo> GetDMSServiceInfoById(int id);
    }
}
