using Microsoft.EntityFrameworkCore;
using ShrinkLink.Auth.Service.Models.Entities;

namespace ShrinkLink.Auth.Service
{
    public class AuthDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public AuthDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.ToTable("Users");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).IsRequired();
                builder.Property(x => x.Username).IsRequired().HasMaxLength(64);
                builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
                builder.Property(x => x.PasswordHash).IsRequired();
                builder.Property(x => x.PasswordSalt).IsRequired();
                builder.Property(x => x.CreatedAt);
                builder.Property(x => x.LastLogin);


                //using indexing for faster searching
                builder.HasIndex(x => x.Username).IsUnique();
                builder.HasIndex(x => x.Email).IsUnique();

                builder.HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId);

            });

            modelBuilder.Entity<Role>(builder =>
            {
                builder.ToTable("Roles");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).IsRequired();
                builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
                builder.Property(x => x.Description).HasMaxLength(256); 


                builder.HasIndex(x => x.Name).IsUnique();
                builder.HasMany(x => x.Users)
                    .WithOne(x => x.Role)
                    .HasForeignKey(x => x.RoleId);


            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
