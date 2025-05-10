using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;

namespace ShrinkLink.UrlShortener.Service.Repositories
{
    public class VisitorRepository : IVisitorRepository
    {
        private readonly UrlShortenerDbContext _context;

        public VisitorRepository(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Visitor visit, CancellationToken ct = default)
        {
            await _context.Visitors.AddAsync(visit, ct);
            await _context.SaveChangesAsync();
        }
    }
}
