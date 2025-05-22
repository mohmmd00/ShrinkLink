using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ShrinkLink.UrlShortener.Service.Models.Entities
{
    public class Visitor
    {
        public Guid PrimaryId { get;private set; }
        public string ProcessedUrlCode { get;private set; }
        public string IpAddress { get;private set; }
        public string Country { get;private set; }
        public string OperatingSystem { get;private set; }
        public string Browser { get;private set; }
        public string DeviceType { get;private set; }
        public string UserAgent { get;private set; }
        public string ClickedAt { get;private set; }


        public ProcessedUrl ProcessedUrl { get; set; } //navigation prop    
        public Guid ShortenGuidId { get; set; } //key to navigate

        public Visitor(string processedUrlCode, string ipAddress, string country, string operatingSystem, string browser, string deviceType, string userAgent,Guid shortenGuidId)
        {
            PrimaryId = Guid.NewGuid();
            ProcessedUrlCode = processedUrlCode;
            IpAddress = ipAddress;
            Country = country;
            OperatingSystem = operatingSystem;
            Browser = browser;
            DeviceType = deviceType;
            UserAgent = userAgent;
            ClickedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ShortenGuidId = shortenGuidId;
        }
    }
}
