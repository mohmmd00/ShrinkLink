using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.Contracts.DataTransferObjects.QrCodeService;

namespace ShrinkLink.WebApp.Models.DashBoardBindingModels
{
    public class QRCodeModel
    {
        public AllLinksFetcherResponseTransferObject LinksViewModel { get; set; }
        public QrCodeGenerateResponseTransferObject QrCode { get; set; }
        public QrCodeGenerateRequestTransferObject QrCodeGenerateRequest { get; set; }

    }
}
