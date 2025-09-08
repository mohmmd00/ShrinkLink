namespace ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService
{
    public enum LinkCheckerResultStatus
    {
        Success,
        InvalidCode,
        Unauthorized, 
        Failed
    }

    public class LinkCheckerResponseTransferObject
    {
        public LinkCheckerResultStatus Status { get; set; }
        public string Link { get; set; }
        public string Message { get; set; }
    }
}
