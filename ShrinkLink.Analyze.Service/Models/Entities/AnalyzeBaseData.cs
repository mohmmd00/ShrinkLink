namespace ShrinkLink.Analyze.Service.Models.Entities
{
    public class AnalyzeBaseData
    {
        public Guid PrimaryId { get; set; }
        public string ProcessedUrlCode { get; set; }
        public string IpAddress { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string OperatingSystem { get; set; }
        public string Browser { get; set; }
        public string DeviceType { get; set; }
        public string UserAgent { get; set; }
        public string ClickedAt { get; set; }
    }
}
