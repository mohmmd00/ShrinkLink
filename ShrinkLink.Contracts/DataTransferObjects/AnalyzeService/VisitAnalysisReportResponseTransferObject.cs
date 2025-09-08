namespace ShrinkLink.Contracts.DataTransferObjects.AnalyzeService
{
    public enum VisitAnalysisReportStatus
    {
        Success,
        InvalidData,
        Unauthorized,
        VisitorNotFound,
        VisitorFetchingFailed,
        Failed
    }
    public class AnalysisData
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
        public Dictionary<string, int> TrafficByHour { get; set; } 
        public Dictionary<string, int> VisitsByDay { get; set; }
        public Dictionary<string, int> VisitsByWeekday { get; set; }
        public Dictionary<string, int> VisitsByDeviceType { get; set; }
        public Dictionary<string, int> VisitsByBrowser { get; set; }
        public Dictionary<string, int> VisitsByOperatingSystem { get; set; }

        public Dictionary<string, int> TopCountries { get; set; }
        public Dictionary<string, int> TopCities { get; set; }
        public Dictionary<string, int> TopDeviceTypes { get; set; }
        public Dictionary<string, int> TopBrowsers { get; set; }
        public Dictionary<string, int> TopOperatingSystems { get; set; }

        public string PeakHour { get; set; }
        public double AverageVisitsPerHour { get; set; }
        public Dictionary<string, double> CountryVisitPercentages { get; set; }

        public string MostActiveCountry { get; set; }
        public string MostActiveCity { get; set; }
        public string MostActiveDeviceType { get; set; }
        public string MostActiveBrowser { get; set; }
        public string MostActiveOperatingSystem { get; set; }
    }

    public class VisitAnalysisReportResponseTransferObject
    {
        public VisitAnalysisReportStatus Status { get; set; }
        public string Message { get; set; }
        public AnalysisData AnalysisData { get; set; }
    }
}