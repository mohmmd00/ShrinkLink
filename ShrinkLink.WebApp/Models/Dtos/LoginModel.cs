using AuthService.Webapp.Contracts.Dtos;

namespace ShrinkLink.WebApp.Models.Dtos
{
    public class LoginModel
    {
        public LoginTransferObject TransferObject { get; set; } 
        public LoginResponseTransferObject ResponseTransferObject { get; set; }
    }
}
