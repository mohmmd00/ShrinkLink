using ShrinkLink.WebApp.Controllers;
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

            builder.Services.AddScoped<WebRequestHandler>();


            var baseAddress = builder.Configuration["ApiSettings:ShortenerAddress"];
            var authApiAddress = builder.Configuration["ApiSettings:AuthAddress"];



            builder.Services.AddHttpClient<WebRequestHandler>("Shortener",client =>
            {
                client.BaseAddress = new Uri(baseAddress);
            });
            builder.Services.AddHttpClient<WebRequestHandler>("Auth", client =>
            {
                client.BaseAddress = new Uri(authApiAddress);
            });
            


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
