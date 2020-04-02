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

            string domain = uri.DnsSafeHost.ToLowerInvariant();

            if (domain.StartsWith("www."))
            {
                domain = domain.Replace("www.", "");
            }

            string uriWithoutScheme = domain + uri.PathAndQuery.Replace("/", "").ToLowerInvariant().TrimEnd('/');

            double trustScore = req.TrustScore;
            string reason = req.Reason;

            return new Publisher()
            {
                PartitionKey = domain,
                TrustScore = trustScore,
                RowKey = uriWithoutScheme,
                Url = url,
                Reason = reason
            };
        }
    }
}
