namespace ShrinkLink.Contracts.DataTransferObjects.AuthorizationService
{
    public enum LoginResultStatus
    {
        Success,
        InvalidUsernameOrPassword,
        InvalidInput,
        Failed
    }
    public class LoginResponseTransferObject
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; }
        public LoginResultStatus Status { get; set; }
    }
}
