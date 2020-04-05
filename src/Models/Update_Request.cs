namespace CheckDenFakt.TrustedPublisher.Models 
{
    public class Update_Request 
    {
        public string Url { get; set; }
        public double TrustScore { get; set; }  
        public string Reason { get; set; }

          }
}