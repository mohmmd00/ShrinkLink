
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;
using ShrinkLink.UrlShortener.Service.Repositories;
using ShrinkLink.UrlShortener.Service.Services;

namespace ShrinkLink.UrlShortener.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("Cn1");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'Cn1' not found.");
            }
            builder.Services.AddDbContext<UrlShortenerDbContext>(x =>
                x.UseSqlServer(connectionString));


            builder.Services.AddScoped<IShortenUrlRepository, ShortenUrlRepository>();
            builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
            builder.Services.AddScoped<IVisitorRepository, VisitorRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
