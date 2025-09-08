namespace ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService
{

    public enum LinkShortenerResultStatus
    {
        Success,
        InvalidUrl,
        Unauthorized,
        ShortLinkCreationFailed,
        Failed
    }

    public class LinkShortenerResponseTransferObject
    {
        public LinkShortenerResultStatus Status { get; set; }
        public string Message { get; set; }
        public string ShortUrl { get; set; }

    }
}

