using Microsoft.EntityFrameworkCore;
using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;

namespace ShrinkLink.UrlShortener.Service.Repositories
{
    public class ProcessedUrlRepository : IProcessedUrlRepository
    {
        private readonly UrlShortenerDbContext _context;

        public ProcessedUrlRepository(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(ProcessedUrl link, CancellationToken ct = default)
        {
            await _context.Links.AddAsync(link, ct);
            var result = await _context.SaveChangesAsync(ct);

            return result > 0; // true if at least one row was affected
        }


        public async Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default)
        {
            return await _context.Links.AnyAsync(x => x.Code == code, ct);
        }
        public async Task<ProcessedUrl> FetchWantedUrl(string code, CancellationToken ct = default)
        {
            var link = await _context.Links.FirstOrDefaultAsync(x => x.Code == code, ct);
            if (link != null)
            {
                return link;
            }
            return null;
        }
        public async Task<List<ProcessedUrl>> FetchAllProcessedUrlsByUserId(Guid userId, CancellationToken ct = default)
        {
            return await _context.Links.Where(link => link.UserId == userId).ToListAsync(ct);
        }
        public async Task<bool> DeleteAsync(Guid userId, string code, CancellationToken ct = default)
        {
            var link = await _context.Links.FirstOrDefaultAsync(x => x.Code == code && x.UserId == userId, ct);
            if (link != null)
            {
                _context.Links.Remove(link);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteAllByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var links = await _context.Links.Where(x => x.UserId == userId).ToListAsync(ct);
            if (!links.Any())
            {
                return false;
            }

            _context.Links.RemoveRange(links);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}