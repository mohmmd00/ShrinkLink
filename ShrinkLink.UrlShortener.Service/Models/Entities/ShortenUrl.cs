namespace ShrinkLink.UrlShortener.Service.Models.Entities
{
    public class ShortenUrl
    {
        public Guid Id { get; }
        public string OriginalUrl { get; private set; } = string.Empty;
        public string ShortUrl { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public string CreatedAt { get; private set; } // beware of the change of this property !!!!

        public List<Visitor> Visitors { get; set; } //navigation prop


        public ShortenUrl(string originalUrl, string code, string shortUrl)
        {
            Id = Guid.NewGuid();
            OriginalUrl = originalUrl;
            ShortUrl = shortUrl;
            Code = code;
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}