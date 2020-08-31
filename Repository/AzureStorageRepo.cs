using AzureStorageAccountGenerator.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAccountGenerator.Repository
{
    public class AzureStorageRepo : IAzureStorageRepo
    {
        protected readonly ApplicationDbContext Context;
        public AzureStorageRepo(ApplicationDbContext context)
        {
            Context = context;
        }
        public async Task<IList<DMSServiceInfo>> GetDMSServiceInfo()
        {
           return await Context.DMSServiceInfo.AsNoTracking()
                 .Where(x => x.AzStorageConnectionString==null && !x.IsDeleted)
                 .ToListAsync();
        }

        public async Task<DMSServiceInfo> GetDMSServiceInfoByAccountName(string accountName)
        {
            return await Context.DMSServiceInfo.AsNoTracking().FirstOrDefaultAsync(x => x.AzStorageContainer == accountName);
        }

        public async Task<int> UpdateDMSServiceInfo(DMSServiceInfo serviceInfo)
        {
            Context.DMSServiceInfo.Attach(serviceInfo);
            var entry = Context.Entry(serviceInfo);
            entry.State = EntityState.Modified;
            return await Context.SaveChangesAsync();
        }

        public async Task<int> UpdateStorageAccountDetails(StorageAccountModel storageModel)
        {
            Context.StorageAccounts.Add(storageModel);
            return await Context.SaveChangesAsync();
        }

        public async Task<int> AddExceptionLogs(ExceptionLog log)
        {
            Context.ExceptionLogs.Add(log);
            return await Context.SaveChangesAsync();
        }
    }
}
