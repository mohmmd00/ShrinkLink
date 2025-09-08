using Microsoft.EntityFrameworkCore;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.UrlShortener.Service.Models.DataTransferObjects;
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
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Visitor>> ReadAllVisitorsAsync(CancellationToken ct = default)
        {
            return await _context.Visitors.ToListAsync(ct);
        }

        public async Task<Visitor?> ReadByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Visitors
                .FirstOrDefaultAsync(v => v.PrimaryId == id, ct);
        }

        public async Task<List<Visitor>> ReadByUrlIdAsync(Guid urlId, CancellationToken ct = default)
        {
            return await _context.Visitors
                .Where(v => v.ShortenGuidId == urlId)
                .ToListAsync(ct);
        }

        public async Task<List<Visitor>> ReadByUserIdAndUrlIdAsync(Guid userId, Guid urlId, CancellationToken ct = default)
        {
            return await _context.Visitors
                .Join(_context.Links,
                    visitor => visitor.ShortenGuidId,
                    link => link.Id,
                    (visitor, link) => new { Visitor = visitor, Link = link })
                .Where(x => x.Link.UserId == userId && x.Link.Id == urlId)
                .Select(x => x.Visitor)
                .ToListAsync(ct);
        }

        public async Task<List<Visitor>> ReadVisitsByUserIdAndCodeAsync(Guid userId,
            string code, CancellationToken ct = default)
        {
            return await _context.Visitors
                .Join(_context.Links,
                    visitor => visitor.ShortenGuidId,
                    link => link.Id,
                    (visitor, link) => new { Visitor = visitor, Link = link })
                .Where(x => x.Link.UserId == userId && x.Link.Code == code)
                .Select(x => x.Visitor)
                .ToListAsync(ct);
        }

        public async Task<List<Visitor>> ReadByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Visitors
                .Join(_context.Links,
                    visitor => visitor.ShortenGuidId,
                    link => link.Id,
                    (visitor, link) => new { Visitor = visitor, Link = link })
                .Where(x => x.Link.UserId == userId)
                .Select(x => x.Visitor)
                .ToListAsync(ct);
        }

        public async Task<int> GetVisitCountByUrlCodeAsync(string code, CancellationToken ct = default)
        {
            return await _context.Visitors
                .CountAsync(v => v.ProcessedUrlCode == code, ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var visitor = await _context.Visitors
                .FirstOrDefaultAsync(v => v.PrimaryId == id, ct);

            if (visitor != null)
            {
                _context.Visitors.Remove(visitor);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}