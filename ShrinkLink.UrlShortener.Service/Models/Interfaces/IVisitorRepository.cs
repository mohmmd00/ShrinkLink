using ShrinkLink.UrlShortener.Service.Models.Entities;

namespace ShrinkLink.UrlShortener.Service.Models.Interfaces
{
    public interface IVisitorRepository
    {
        Task CreateAsync(Visitor visit, CancellationToken ct = default);
    }
}
