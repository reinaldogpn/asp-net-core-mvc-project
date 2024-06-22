using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using SalesWebMvc.Services;
namespace SalesWebMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SalesWebMvcContext>(
                options => options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMvcContext"),
                new MySqlServerVersion(new Version("8.0.2")),
                builder => builder.MigrationsAssembly("SalesWebMvc")));

            // Register created services
            builder.Services.AddScoped<SeedingService>();
            builder.Services.AddScoped<SellerService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            using (var scope = app.Services.CreateScope())
            {
                SeedingService seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
                // Chamar o método do serviço
                seedingService.Seed();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
