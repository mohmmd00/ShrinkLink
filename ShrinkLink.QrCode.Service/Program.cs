
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QRCodeApi.Services;
using ShrinkLink.QrCode.Service.Models.Interfaces;
using ShrinkLink.QrCode.Service.Services;

namespace ShrinkLink.QrCode.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {



            var builder = WebApplication.CreateBuilder(args);

            var shortenerAddress = builder.Configuration["ApiSettings:ShortenerAddress"];
            if (string.IsNullOrEmpty(shortenerAddress))
            {
                throw new InvalidOperationException("ShortenerAddress is not configured.");
            }


            // Add services to the container.




            builder.Services.AddScoped<IQRCodeService,QRCodeService>();



            builder.Services.AddScoped<IShortenerServiceHttpRequestHandler,ShortenerServiceHttpRequestHandler>();

            builder.Services.AddHttpClient("Shortener", Client =>
            {
                Client.BaseAddress = new Uri(shortenerAddress);
            });




            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebApp", builder =>
                {
                    builder.WithOrigins("https://localhost:7233")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
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
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
