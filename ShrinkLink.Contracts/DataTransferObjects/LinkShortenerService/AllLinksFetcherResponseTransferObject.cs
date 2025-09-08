namespace ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService
{

    public enum AllLinksFetcherResultStatus
    {
        Success,
        Unauthorized,
        NoLinksFound,
        Failed
    }

    public class LinksViewModel
    {
        public string OriginalUrl { get; set; }
        public string Code { get; set; }
        public string CreatedAt { get; set; }
    }

    public class AllLinksFetcherResponseTransferObject
    {
        public AllLinksFetcherResultStatus Status { get; set; }
        public string Message { get; set; }
        public List<LinksViewModel> LinksToViewModel { get; set; }

    }
}
