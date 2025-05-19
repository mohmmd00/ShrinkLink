using AuthService.Webapp.Contracts.Dtos;

namespace ShrinkLink.WebApp.Models.Dtos
{
    public class RegisterModel
    {
        public RegisterTransferObject TransferObject { get; set; }
        public RegisterResponseTransferObject ResponseTransferObject { get; set; }
    }
}
