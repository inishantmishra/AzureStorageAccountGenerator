using AzureStorageAccountGenerator.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAccountGenerator.Repository
{
    public partial class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DMSServiceInfo> DMSServiceInfo { get; set; }
        public virtual DbSet<StorageAccountModel> StorageAccounts { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }


    }
}
