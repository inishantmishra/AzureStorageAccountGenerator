using Microsoft.Azure.Management.Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAccountGenerator.Services
{
    public class StorageAccountListResult
    {
        public int Count { get; set; }

        public List<StorageAccount> value { get; set; }
    }
}
