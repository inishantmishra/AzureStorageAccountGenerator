using Npgsql.TypeHandlers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAccountGenerator.Models
{
    public class DMSServiceInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public System.Guid TenantId { get; set; }
        public string DatabaseServer { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabaseName { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabasePort { get; set; }
        public string DMSTenantURL { get; set; }
        public string AzStorageConnectionString { get; set; }
        public string AzStorageContainer { get; set; }
        public Boolean IsDbProvisioned { get; set; }
        public string Location { get; set; }
        public Boolean IsReplication { get; set; }
    }
}
