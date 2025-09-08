
using System.Text;
using ShrinkLink.Analyze.Service.Models.Interfaces;
using ShrinkLink.Analyze.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace ShrinkLink.Analyze.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            var shortenerAddress = builder.Configuration["ApiSettings:ShortenerAddress"];
            if (string.IsNullOrEmpty(shortenerAddress))
            {
                throw new InvalidOperationException("ShortenerAddress is not configured.");
            }



            builder.Services.AddScoped<IAnalyzeService,AnalyzeService>();

            builder.Services.AddScoped<IAuthServiceHttpRequestHandler , AuthServiceHttpRequestHandler>();
            builder.Services.AddHttpClient<IShortenerServiceHttpRequestHandler, ShortenerServiceHttpRequestHandler>();


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

            builder.Services.AddHttpClient("Shortener", Client =>
            {
                Client.BaseAddress = new Uri(shortenerAddress);
            });



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowClientWithCredentials", policy =>
                {
                    policy.WithOrigins("https://localhost:7233") 
                        .AllowCredentials()                   
                        .AllowAnyHeader()
                        .AllowAnyMethod();
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowClientWithCredentials");

            app.MapControllers();

            app.Run();
        }
    }
}
