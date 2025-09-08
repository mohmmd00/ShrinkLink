using ShrinkLink.Contracts.DataTransferObjects.AnalyzeService;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;

namespace ShrinkLink.Analyze.Service.Models.Interfaces
{
    public interface IAnalyzeService
    {
        Task<VisitAnalysisReportResponseTransferObject> AnalyzeVisitsAsync(List<VisitAsData> visitors,
            CancellationToken ct = default);
    }
}
