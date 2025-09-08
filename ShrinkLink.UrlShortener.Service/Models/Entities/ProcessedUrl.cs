namespace ShrinkLink.UrlShortener.Service.Models.Entities
{
    public class ProcessedUrl
    {
        public Guid Id { get; }
        public string OriginalUrl { get; private set; }
        public Guid UserId { get;private set; }
        public string Code { get; private set; }
        public string CreatedAt { get; private set; } // beware of the change of this property !!!!

        public List<Visitor> Visitors { get; set; } //navigation prop


        public ProcessedUrl(string originalUrl, string code, Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OriginalUrl = originalUrl;
            Code = code;
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}