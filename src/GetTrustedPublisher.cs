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
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using CheckDenFakt.TrustedPublisher.Models;
using System.Linq;

namespace CheckDenFakt.TrustedPublisher
{
    public static class GetTrustedPublisher
    {
        [FunctionName("GetAllPublisher")]
        public static async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] dynamic req, [Table("Publisher")] CloudTable cloudTable,
            ILogger log)
        {
            TableQuery<Publisher> rangeQuery = new TableQuery<Publisher>();

            List<Publisher> publishers = new List<Publisher>();

            // Execute the query and loop through the results
            foreach (var entity in await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null).ConfigureAwait(false))
            {
                publishers.Add(entity);
            }

            return new OkObjectResult(publishers);
        }

        [FunctionName("GetTrustedPublisher")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] Request req, [Table("Publisher")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            #region Null Checks
            if (cloudTable == null)
            {
                throw new ArgumentNullException(nameof(cloudTable));
            }
            #endregion

            try
            {
                if (!Uri.IsWellFormedUriString(req.Url, UriKind.Absolute))
                {
                    throw new FormatException("Url isn't well formed!");
                }

                string url = req.Url;

                Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
                string domain = uri.DnsSafeHost.ToLowerInvariant();

                if (domain.StartsWith("www."))
                {
                    domain = domain.Replace("www.", "");
                }

                string uriWithoutScheme = domain + uri.PathAndQuery.Replace("/", "").ToLowerInvariant().TrimEnd('/');

                TableQuery<Publisher> rangeQuery = new TableQuery<Publisher>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, domain) );

                List<Publisher> list = new List<Publisher>();

                // Execute the query and loop through the results
                foreach (var entity in await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null).ConfigureAwait(false))
                {
                    list.Add(entity);
                }

                foreach (var item in list.OrderByDescending(x => x.RowKey.Length))
                {
                    if (uriWithoutScheme.StartsWith(item.RowKey))
                    {
                        return new OkObjectResult(item);
                    }
                }

                return new NotFoundObjectResult(null);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogDebug(ex.StackTrace);
                throw;
            }
        }
    }
}
