using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;

namespace ShrinkLink.WebApp.Models.DashBoardBindingModels
{
    public class LinkModel
    {
        public LinkShortenerRequestTransferObject LonglinkModel { get; set; } 
        public AllLinksFetcherResponseTransferObject LinksViewModel { get; set; }

    }
}
