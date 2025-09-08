namespace ShrinkLink.Contracts.DataTransferObjects.AuthorizationService
{
    public enum UserInfoResultStatus
    {
        Success,
        InvalidToken,
        MissingToken,
        Failed
    }

    public class UserInformation
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }

    public class UserBasicInformationTransferObject
    {
        public UserInformation UserInformation { get; set; }
        public UserInfoResultStatus Status { get; set; }
        public string Message { get; set; }

    }
}
