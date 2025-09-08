using ShrinkLink.Analyze.Service.Models.Interfaces;
using ShrinkLink.Contracts.DataTransferObjects.AnalyzeService;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;

namespace ShrinkLink.Analyze.Service.Services
{
    public class AnalyzeService : IAnalyzeService
    {
        public async Task<VisitAnalysisReportResponseTransferObject> AnalyzeVisitsAsync(
            List<VisitAsData> visitors, CancellationToken ct = default)
        {
            if (visitors == null)
            {
                return new VisitAnalysisReportResponseTransferObject()
                {
                    Status = VisitAnalysisReportStatus.Failed,
                    Message = "Failed to Receive Visits"
                };
            }

            if (!visitors.Any())
            {
                return new VisitAnalysisReportResponseTransferObject()
                {
                    Status = VisitAnalysisReportStatus.VisitorNotFound,
                    Message = "No visits found for the provided code.",
                    AnalysisData = new AnalysisData()
                    
                };
            }

            try
            {
                int totalVisits = visitors.Count();
                var uniqueCountries = visitors.Select(v => v.Country).Distinct().Count();
                var uniqueCities = visitors.Select(v => v.City).Distinct().Count();
                var uniqueDeviceTypes = visitors.Select(v => v.DeviceType).Distinct().Count();
                var uniqueBrowsers = visitors.Select(v => v.Browser).Distinct().Count();
                var uniqueOperatingSystems = visitors.Select(v => v.OperatingSystem).Distinct().Count();

                var visitsByCountry = visitors.GroupBy(v => v.Country).ToDictionary(g => g.Key, g => g.Count());
                var visitsByCity = visitors.GroupBy(v => v.City).ToDictionary(g => g.Key, g => g.Count());
                var visitsByDay = visitors.GroupBy(v => DateTime.Parse(v.ClickedAt).ToString("yyyy-MM-dd")).ToDictionary(g => g.Key, g => g.Count());
                var visitsByWeekday = visitors.GroupBy(v => DateTime.Parse(v.ClickedAt).DayOfWeek.ToString()).ToDictionary(g => g.Key, g => g.Count());
                var visitsByDeviceType = visitors.GroupBy(v => v.DeviceType).ToDictionary(g => g.Key, g => g.Count());
                var visitsByBrowser = visitors.GroupBy(v => v.Browser).ToDictionary(g => g.Key, g => g.Count());
                var visitsByOperatingSystem = visitors.GroupBy(v => v.OperatingSystem).ToDictionary(g => g.Key, g => g.Count());

                // Group by hour (0–23) using string keys like "0", "13", "23"
                var visitsByHour = visitors
                    .GroupBy(v => DateTime.Parse(v.ClickedAt).Hour)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count());

                // Ensure all 24 hours exist
                foreach (int hour in Enumerable.Range(0, 24))
                {
                    string hourKey = hour.ToString();
                    if (!visitsByHour.ContainsKey(hourKey))
                        visitsByHour[hourKey] = 0;
                }

                // Sort by hour as integer
                var trafficByHour = visitsByHour
                    .OrderBy(kv => int.Parse(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                string peakHour = trafficByHour.OrderByDescending(kv => kv.Value).FirstOrDefault().Key ?? "None";
                double averageVisitsPerHour = trafficByHour.Any() ? (double)totalVisits / 24 : 0;

                var topCountries = visitsByCountry.OrderByDescending(kv => kv.Value).Take(3).ToDictionary(kv => kv.Key, kv => kv.Value);
                var topCities = visitsByCity.OrderByDescending(kv => kv.Value).Take(3).ToDictionary(kv => kv.Key, kv => kv.Value);
                var topDeviceTypes = visitsByDeviceType.OrderByDescending(kv => kv.Value).Take(3).ToDictionary(kv => kv.Key, kv => kv.Value);
                var topBrowsers = visitsByBrowser.OrderByDescending(kv => kv.Value).Take(3).ToDictionary(kv => kv.Key, kv => kv.Value);
                var topOperatingSystems = visitsByOperatingSystem.OrderByDescending(kv => kv.Value).Take(3).ToDictionary(kv => kv.Key, kv => kv.Value);

                var mostActiveCountry = topCountries.Keys.FirstOrDefault() ?? "None";
                var mostActiveCity = topCities.Keys.FirstOrDefault() ?? "None";
                var mostActiveDeviceType = topDeviceTypes.Keys.FirstOrDefault() ?? "None";
                var mostActiveBrowser = topBrowsers.Keys.FirstOrDefault() ?? "None";
                var mostActiveOperatingSystem = topOperatingSystems.Keys.FirstOrDefault() ?? "None";

                var countryVisitPercentages = visitsByCountry.ToDictionary(
                    kv => kv.Key,
                    kv => Math.Round(kv.Value * 100.0 / totalVisits, 2)
                );

                var analysisData = new AnalysisData()
                {
                    TotalVisits = totalVisits,
                    UniqueCountries = uniqueCountries,
                    UniqueCities = uniqueCities,
                    UniqueDeviceTypes = uniqueDeviceTypes,
                    UniqueBrowsers = uniqueBrowsers,
                    UniqueOperatingSystems = uniqueOperatingSystems,
                    VisitsByCountry = visitsByCountry,
                    VisitsByCity = visitsByCity,
                    VisitsByHour = visitsByHour,
                    TrafficByHour = trafficByHour,
                    VisitsByDay = visitsByDay,
                    VisitsByWeekday = visitsByWeekday,
                    VisitsByDeviceType = visitsByDeviceType,
                    VisitsByBrowser = visitsByBrowser,
                    VisitsByOperatingSystem = visitsByOperatingSystem,
                    TopCountries = topCountries,
                    TopCities = topCities,
                    TopDeviceTypes = topDeviceTypes,
                    TopBrowsers = topBrowsers,
                    TopOperatingSystems = topOperatingSystems,
                    PeakHour = peakHour,
                    AverageVisitsPerHour = averageVisitsPerHour,
                    CountryVisitPercentages = countryVisitPercentages,
                    MostActiveCountry = mostActiveCountry,
                    MostActiveCity = mostActiveCity,
                    MostActiveDeviceType = mostActiveDeviceType,
                    MostActiveBrowser = mostActiveBrowser,
                    MostActiveOperatingSystem = mostActiveOperatingSystem
                };

                return new VisitAnalysisReportResponseTransferObject
                {
                    Status = VisitAnalysisReportStatus.Success,
                    Message = "Visit analysis generated successfully.",
                    AnalysisData = analysisData
                };
            }
            catch (Exception)
            {
                return new VisitAnalysisReportResponseTransferObject
                {
                    Status = VisitAnalysisReportStatus.Failed,
                    Message = "An unexpected error occurred while analyzing visits."
                };
            }
        }
    }
}