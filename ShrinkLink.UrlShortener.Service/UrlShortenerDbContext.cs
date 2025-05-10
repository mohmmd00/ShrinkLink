
using Microsoft.EntityFrameworkCore;
using ShrinkLink.UrlShortener.Service.Models.Entities;
using ShrinkLink.UrlShortener.Service.Services;

namespace ShrinkLink.UrlShortener.Service
{
    public class UrlShortenerDbContext : DbContext
    {
        public DbSet<ShortenUrl> Links { get; set; }
        public DbSet<Visitor> Visitors { get; set; }

        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenUrl>(builder =>
            {
                builder.ToTable ("ShortenUrl");
                builder.HasKey  (x => x.Id);
                builder.Property(x => x.Code).IsRequired().HasMaxLength(UrlShortenerService.NumberOfCharsInShortLink);
                builder.HasIndex(x => x.Code).IsUnique();
                builder.Property(x => x.OriginalUrl).IsRequired().HasMaxLength(1024);
                builder.Property(x => x.ShortUrl).IsRequired();
                builder.Property(x => x.CreatedAt).IsRequired();
                builder.HasMany(S => S.Visitors)
                    .WithOne(V => V.ShortenUrl)
                    .HasForeignKey(V => V.ShortenGuidId);


            });
            modelBuilder.Entity<Visitor>(builder =>
            {
                builder.ToTable ("Visitor");
                builder.HasKey  (x => x.PrimaryId);
                builder.Property(x => x.Code).IsRequired();
                builder.Property(x => x.IpAddress).IsRequired();
                builder.Property(x => x.Country);
                builder.Property(x => x.OperatingSystem);
                builder.Property(x => x.Browser);
                builder.Property(x => x.DeviceType);
                builder.Property(x => x.UserAgent);
                builder.Property(x => x.ClickedAt);
                builder.Property(x => x.RedirectSuccessful);
                builder.HasOne(V => V.ShortenUrl)
                    .WithMany(S => S.Visitors)
                    .HasForeignKey(V => V.ShortenGuidId);

            });
        }
    }
}
