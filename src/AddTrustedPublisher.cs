using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CheckDenFakt.TrustedPublisher.Models;

namespace CheckDenFaktTrustedPublisher
{
    public static class AddTrustedPublisher
    {
        [FunctionName("AddTrustedPublisher")]
        [return: Table("Publisher")]
        public static Publisher Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string url = req.Url;
            Uri.TryCreate(url, UriKind.Absolute, out Uri uri);

            string domain = uri.DnsSafeHost;
            string uriWithoutScheme = uri.Host + uri.PathAndQuery;

            decimal trustScore = req.TrustScore;

            return new Publisher()
            {
                PartitionKey = domain,
                trustScore = trustScore,
                RowKey = uriWithoutScheme.Replace("/", ""),
                Url = uriWithoutScheme
            };
        }
    }
}
