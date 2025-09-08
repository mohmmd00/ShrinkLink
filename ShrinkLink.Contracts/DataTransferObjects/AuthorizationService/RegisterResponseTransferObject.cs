namespace ShrinkLink.Contracts.DataTransferObjects.AuthorizationService
{

    public enum RegisterResultStatus
    {
        Success,
        UsernameExists,
        EmailAddressExists,
        InvalidUsername,
        InvalidPassword,
        InvalidEmailAddress,
        Failed
    }
    public class RegisterResponseTransferObject
    {
        public string Message { get; set; }
        public RegisterResultStatus Status { get; set; }
    }
}
