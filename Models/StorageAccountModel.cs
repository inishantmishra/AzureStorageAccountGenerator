using Microsoft.Azure.Management.Storage.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAccountGenerator.Models
{
    public class StorageAccountModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DMSServiceInfo DMSServiceInfo { get; set; }
        public int DMSServiceInfoId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountId { get; set; }
        public string Kind { get; set; }
        public string SkuName { get; set; }
        public string BlobEndPoints { get; set; }
        public string QueueEndPoints { get; set; }
        public string FileEndPoints { get; set; }
        public string TableEndPoints { get; set; }
        public string PrimaryLocation { get; set; }
        public DateTime? CreationTime { get; set; }
        public string LargeFileSharesState { get; set; }
        public string AccessTier { get; set; }
        public string KeySource { get; set; }
        public bool? IsHnsEnabled { get; set; }
        public bool? EnableHttpsTrafficOnly { get; set; }
        public bool? AllowBlobPublicAccess { get; set; }
        public string MinimumTlsVersion { get; set; }
        public string TagKey { get; set; }
        public string TagValue { get; set; }
        public string Location { get; set; }
        public string KeyAccessName { get; set; }
        public string KeyAccessValue { get; set; }
    }
}
