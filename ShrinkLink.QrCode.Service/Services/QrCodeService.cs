using QRCoder;
using ShrinkLink.Contracts.DataTransferObjects.QrCodeService;
using ShrinkLink.QrCode.Service.Models.Interfaces;

namespace QRCodeApi.Services
{
    public class QRCodeService : IQRCodeService
    {

        private readonly IConfiguration _configuration;

        public QRCodeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<QrCodeGenerateResponseTransferObject> GenerateQrCode(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return new QrCodeGenerateResponseTransferObject
                {
                    Status = QrCodeGenerateStatus.InvalidInput,
                    Message = "Text is required to generate a QR link.",
                    QrCodeBase64 = null
                };
            }

            try
            {
                // 1. Generate QR link
                using var qrGenerator = new QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrData);

                // 2. Render to PNG byte[]
                byte[] pngBytes = qrCode.GetGraphic(20);

                // 3. Convert to Base64 string
                string base64 = Convert.ToBase64String(pngBytes);

                return new QrCodeGenerateResponseTransferObject
                {
                    Status = QrCodeGenerateStatus.Success,
                    Message = "QR link generated successfully.",
                    QrCodeBase64 = base64
                };
            }
            catch (Exception ex)
            {
                return new QrCodeGenerateResponseTransferObject
                {
                    Status = QrCodeGenerateStatus.Failed,
                    Message = $"Error generating QR link: {ex.Message}",
                    QrCodeBase64 = null
                };
            }
        }
    }
}