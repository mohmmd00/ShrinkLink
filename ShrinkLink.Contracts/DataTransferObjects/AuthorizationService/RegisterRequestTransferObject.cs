namespace ShrinkLink.Contracts.DataTransferObjects.AuthorizationService
{
    public class RegisterRequestTransferObject
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
