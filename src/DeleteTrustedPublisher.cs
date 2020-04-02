using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using CheckDenFakt.TrustedPublisher.Models;
using Microsoft.WindowsAzure.Storage;

namespace CheckDenFaktTrustedPublisher
{
    public static class DeleteTrustedPublisher
    {
        [FunctionName("DeleteTrustedPublisher")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] dynamic req, [Table("Publisher")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            #region Null Checks
            if (req == null)
            {
                throw new ArgumentNullException(nameof(Publisher));
            }

            if (cloudTable == null)
            {
                throw new ArgumentNullException(nameof(cloudTable));
            }
            #endregion

            var entity = new DynamicTableEntity(req.PartitionKey, req.RowKey)
            {
                ETag = "*"
            };

            var deleteOperation = TableOperation.Delete(entity);

            try
            {
                await cloudTable.ExecuteAsync(deleteOperation).ConfigureAwait(false);

                return new OkResult();
            }
            catch (StorageException ex)
            {
                log.LogError(ex, $"There was an storage exception {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Unknown error occured : {ex.Message}");
                throw;
            }
        }
    }
}
