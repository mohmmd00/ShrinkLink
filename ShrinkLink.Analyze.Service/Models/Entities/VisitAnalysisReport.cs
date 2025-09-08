namespace ShrinkLink.Analyze.Service.Models.Entities
{
    public class VisitAnalysisReport
    {
        public int TotalVisits { get; set; }
        public int UniqueCountries { get; set; }
        public int UniqueCities { get; set; }
        public Dictionary<string, int> VisitsByCountry { get; set; }
        public Dictionary<string, int> VisitsByCity { get; set; }
        public Dictionary<string, int> VisitsByHour { get; set; }
        public string MostActiveCountry { get; set; }
        public string MostActiveCity { get; set; }
    }
}
