namespace AuthService.Webapp.Contracts.Dtos;

public class LoginResponseTransferObject
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}