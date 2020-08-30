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
                 .Where(x => x.AzStorageConnectionString==null)
                 .ToListAsync();
        }

        public async Task<DMSServiceInfo> GetDMSServiceInfoById(int id)
        {
            return await Context.DMSServiceInfo.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> UpdateAZConnectionStringandContainer(DMSServiceInfo serviceInfo)
        {
            Context.DMSServiceInfo.Attach(serviceInfo);
            var entry = Context.Entry(serviceInfo);
            entry.State = EntityState.Modified;
            return await Context.SaveChangesAsync();
        }
    }
}
