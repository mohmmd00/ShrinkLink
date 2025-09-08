namespace ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService
{


    public enum LinkRemoverResultStatus
    {
        Success,
        InvalidCode,
        Unauthorized,
        InvalidInformation,
        Failed
    }
    public class LinkRemoverResponseTransferObject
    {
        public LinkRemoverResultStatus Status { get; set; }
        public string Message { get; set; }
    }
}
