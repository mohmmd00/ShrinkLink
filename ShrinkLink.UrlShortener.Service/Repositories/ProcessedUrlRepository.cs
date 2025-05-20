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

        public async Task CreateAsync(ProcessedUrl link, CancellationToken ct = default)
        {
            await _context.Links.AddAsync(link, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> IsCodeExistsAsync(string code, CancellationToken ct = default)
        {
            return await _context.Links.AnyAsync(x => x.Code == code, ct);
        }

        public async Task<ProcessedUrl> FetchWantedUrl(string code, CancellationToken ct = default)
        {
            return await _context.Links.FirstOrDefaultAsync(x => x.Code == code, ct);
        }

    }
}
