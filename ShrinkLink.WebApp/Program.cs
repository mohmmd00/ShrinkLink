using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShrinkLink.WebApp.Services;

namespace ShrinkLink.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register scoped services
            builder.Services.AddScoped<AuthServiceHttpRequestHandler>();
            builder.Services.AddScoped<ShortenerServiceHttpRequestHandler>();
            builder.Services.AddScoped<AnalysisServiceHttpRequestHandler>();
            builder.Services.AddScoped<QRCodeServiceHttpRequestHandler>();

            // Get and validate configuration values
            var shortenerApiServiceAddress = builder.Configuration["ApiSettings:ShortenerAddress"]
                ?? throw new InvalidOperationException("Missing 'ApiSettings:ShortenerAddress' in configuration");

            var authorizationApiServiceAddress = builder.Configuration["ApiSettings:AuthAddress"]
                ?? throw new InvalidOperationException("Missing 'ApiSettings:AuthAddress' in configuration");

            var qrCodeApiServiceAddress = builder.Configuration["ApiSettings:QRCodeAddress"]
                ?? throw new InvalidOperationException("Missing 'ApiSettings:QRCodeAddress' in configuration");

            var analysisApiServiceAddress = builder.Configuration["ApiSettings:AnalysisAddress"]
                ?? throw new InvalidOperationException("Missing 'ApiSettings:AnalysisAddress' in configuration");

            // Register HTTP clients with validation
            builder.Services.AddHttpClient<ShortenerServiceHttpRequestHandler>("Shortener", client =>
            {
                client.BaseAddress = new Uri(shortenerApiServiceAddress);
            });

            builder.Services.AddHttpClient<AuthServiceHttpRequestHandler>("Auth", client =>
            {
                client.BaseAddress = new Uri(authorizationApiServiceAddress);
            });

            builder.Services.AddHttpClient<QRCodeServiceHttpRequestHandler>("Qrcode", client =>
            {
                client.BaseAddress = new Uri(qrCodeApiServiceAddress);
            });

            // Added missing Analysis HTTP client
            builder.Services.AddHttpClient<AnalysisServiceHttpRequestHandler>("Analysis", client =>
            {
                client.BaseAddress = new Uri(analysisApiServiceAddress);
            });

            builder.Services.AddHttpContextAccessor();

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

            









            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}