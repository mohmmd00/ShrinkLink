using ShrinkLink.Contracts.DataTransferObjects.AnalyzeService;

namespace ShrinkLink.WebApp.Models.Analyze
{
    public class AnalyzeLinkViewModel
    {
        public int TotalVisits { get; set; }
        public int UniqueCountries { get; set; }
        public int UniqueCities { get; set; }
        public int UniqueDeviceTypes { get; set; }
        public int UniqueBrowsers { get; set; }
        public int UniqueOperatingSystems { get; set; }
        public Dictionary<string, int> VisitsByCountry { get; set; }
        public Dictionary<string, int> VisitsByCity { get; set; }
        public Dictionary<string, int> VisitsByHour { get; set; }
        public Dictionary<string, int> VisitsByDeviceType { get; set; }
        public Dictionary<string, int> VisitsByBrowser { get; set; }
        public Dictionary<string, int> VisitsByOperatingSystem { get; set; }
        public string MostActiveCountry { get; set; }
        public string MostActiveCity { get; set; }
        public string MostActiveDeviceType { get; set; }
        public string MostActiveBrowser { get; set; }
        public string MostActiveOperatingSystem { get; set; }
        public VisitAnalysisReportStatus Status { get; set; }
        public string Message { get; set; }
    }


}

