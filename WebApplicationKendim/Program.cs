using WebApplicationKendim.Utility;
using Microsoft.EntityFrameworkCore;
using WebApplicationKendim.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<UygulamaDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<UygulamaDbContext>().AddDefaultTokenProviders();
        builder.Services.AddRazorPages();

        builder.Services.AddScoped<IKitapTurleriRepository, KitapTurleriRepository>();
        builder.Services.AddScoped<IKitapRepository, KitapRepository>();
        builder.Services.AddScoped<IKiralamaRepository, KiralamaRepository>();
        builder.Services.AddScoped<IEmailSender, EmailSender>();

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
        app.MapRazorPages();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Rol oluþturma metodu
        await CreateRolesAsync(app.Services);

        app.Run();
    }

    // Rol oluþturma metodu
    private static async Task CreateRolesAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
			else if (!await roleManager.RoleExistsAsync("Ogrenci"))
			{
				await roleManager.CreateAsync(new IdentityRole("Ogrenci"));
			}
			// Diðer rolleri buraya ekleyebilirsiniz

		}
    }
}
