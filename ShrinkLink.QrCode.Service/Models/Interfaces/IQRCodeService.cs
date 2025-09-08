using ShrinkLink.Contracts.DataTransferObjects.QrCodeService;

namespace ShrinkLink.QrCode.Service.Models.Interfaces
{
    public interface IQRCodeService
    {
        Task<QrCodeGenerateResponseTransferObject> GenerateQrCode(string link);
    }
}
