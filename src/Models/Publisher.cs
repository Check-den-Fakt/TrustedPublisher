namespace CheckDenFakt.TrustedPublisher.Models 
{
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Table;

    public class Publisher : TableEntity
    {
        public Publisher()
        {
        }

        public Publisher(string name, string url)
        {
            this.PartitionKey = name;
            this.RowKey = url;
        }

        public string Url { get; set; }

        public double TrustScore { get; set; }

        public string Reason { get; set; }
    }
}