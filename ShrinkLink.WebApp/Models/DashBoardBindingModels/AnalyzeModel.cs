using ShrinkLink.Contracts.DataTransferObjects.AnalyzeService;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.WebApp.Models.Analyze;

namespace ShrinkLink.WebApp.Models.DashBoardBindingModels
{
    public class AnalyzeModel
    {

        public VisitAnalysisReportResponseTransferObject AnalyzeViewModel { get; set; }
        public AllLinksFetcherResponseTransferObject Links { get; set; }
    }
}
