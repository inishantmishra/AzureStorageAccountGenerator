using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AzureStorageAccountGenerator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure;

namespace AzureStorageAccountGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureStorageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageService _azureStorageService;
        string subscriptionId = string.Empty;

        public AzureStorageController(IConfiguration configuration, IAzureStorageService azureStorageService)
        {
            _configuration = configuration;
            _azureStorageService = azureStorageService;
            subscriptionId = _configuration.GetSection("AppSettings:subscriptionId").Value;
        }

        [HttpGet("GetAllStorageAccounts")]
        public async Task<IActionResult> GetAllStorageAccounts()
        {
            string token = _azureStorageService.GetAuthorizationHeader().Result;
            TokenCredentials credential = new TokenCredentials(token);

            StorageManagementClient storageMgmtClient = new StorageManagementClient(credential) { SubscriptionId = subscriptionId };
            IPage<StorageAccount> storageAccounts = await _azureStorageService.GetAllStorageAccount(storageMgmtClient);

            StorageAccountListResult resultList = new StorageAccountListResult();
            resultList.Count = storageAccounts.ToList().Count;
            resultList.value = storageAccounts.ToList();
            return Ok(resultList);
        }
       
        [HttpPost("CreateStorageAccount")]
        public async Task<IActionResult> GenerateAccount()
        {
            List<StorageAccount> storageAccounts = new List<StorageAccount>();
            try
            {
                string token = _azureStorageService.GetAuthorizationHeader().Result;
                TokenCredentials credential = new TokenCredentials(token);

                StorageManagementClient storageMgmtClient = new StorageManagementClient(credential) { SubscriptionId = subscriptionId };
                ResourceManagementClient resourcesClient = new ResourceManagementClient(credential) { SubscriptionId = subscriptionId };

                //await _azureStorageService.RegisterStorageResourceProvider(resourcesClient);
                storageAccounts = await _azureStorageService.CreateStorageAccount(storageMgmtClient);
            }
            catch(Exception ex)
            {
                
                throw ex;
            }
            return Ok(storageAccounts);
        }


        [HttpDelete("DeleteStorageAccount")]
        public async Task<IActionResult> DeleteStorageAccount(string accountName)
        {
            string token = _azureStorageService.GetAuthorizationHeader().Result;
            TokenCredentials credential = new TokenCredentials(token);
            StorageManagementClient storageMgmtClient = new StorageManagementClient(credential) { SubscriptionId = subscriptionId };
            await _azureStorageService.DeleteStorageAccount(accountName, storageMgmtClient);

            return Ok();
        }

    }
}