using System;

namespace ShrinkLink.Contracts.DataTransferObjects.QrCodeService
{
    public enum QrCodeGenerateStatus
    {
        Success,
        UnAuthorized,
        InvalidInput,
        CodeCheckingFailed,
        Failed
    }

    public class QrCodeGenerateResponseTransferObject
    {
        public QrCodeGenerateStatus Status { get; set; }
        public string Message { get; set; }
        public string QrCodeBase64 { get; set; } // QR Code image as Base64 string
    }
}