using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TradeWave.Data;

var builder = WebApplication.CreateBuilder(args);

// Oturum yönetimi ve kimlik doðrulama ekleme
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum süresi
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";  // Login sayfasýna yönlendirme
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Oturum süresi
        options.SlidingExpiration = true;  // Oturumun kaybolmadan yeniden aktif olmasý
    });

// Authorization (yetkilendirme)
builder.Services.AddAuthorization();

// PostgreSQL veritabaný baðlantýsý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC yapýlandýrmasý
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Oturum yönetimi
app.UseSession();

// Kimlik doðrulama
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Yetkilendirme
app.UseAuthorization();

// Ana sayfa yönlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // Ana sayfaya yönlendirme

app.Run();
