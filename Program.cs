using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TradeWave.Data;

var builder = WebApplication.CreateBuilder(args);

// Oturum y�netimi ve kimlik do�rulama ekleme
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum s�resi
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";  // Login sayfas�na y�nlendirme
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Oturum s�resi
        options.SlidingExpiration = true;  // Oturumun kaybolmadan yeniden aktif olmas�
    });

// Authorization (yetkilendirme)
builder.Services.AddAuthorization();

// PostgreSQL veritaban� ba�lant�s�
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC yap�land�rmas�
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Oturum y�netimi
app.UseSession();

// Kimlik do�rulama
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Yetkilendirme
app.UseAuthorization();

// Ana sayfa y�nlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // Ana sayfaya y�nlendirme

app.Run();
