namespace AuthService.Webapp.Contracts.Dtos;

public class AuthResponseTransferObject
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}