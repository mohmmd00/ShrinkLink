using UrlShortener.Webapp.Contracts.Dtos;

namespace ShrinkLink.WebApp.Models.Dtos
{
    public class ShortenerModel
    {
        public ShortLinkTransferObject Response { get; set; } = new();
        public LongLinkTransferObject Request { get; set; } = new();


    }
}
