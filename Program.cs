using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TradeWave.Models;
using TradeWave.Data;

var builder = WebApplication.CreateBuilder(args);

// SMTP ayarlarını ekleyelim
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// EmailService bağımlılığını ekleyelim
builder.Services.AddTransient<EmailService>();

// Oturum yönetimi ve kimlik doğrulama ekleme
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum süresi
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";  // Login sayfasına yönlendirme
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Oturum süresi
        options.SlidingExpiration = true;  // Oturumun kaybolmadan yeniden aktif olması
    });

// Authorization (yetkilendirme)
builder.Services.AddAuthorization();

// PostgreSQL veritabanı bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC yapılandırması
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Oturum yönetimi
app.UseSession();

// Kimlik doğrulama
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
