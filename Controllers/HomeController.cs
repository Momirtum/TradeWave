using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TradeWave.Models;
using TradeWave.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace TradeWave.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Markets()
        {
            return View();
        }
        [Authorize]
        public IActionResult WatchList()
        {
            return View();
        }
        [Authorize]
        public IActionResult Wallet()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult Profile()
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Home"); // Kullanıcı oturum açmamışsa login'e yönlendir
            }

            var user = _context.User.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return RedirectToAction("Register", "Home"); // Kullanıcı bulunamadıysa kayıt sayfasına yönlendir
            }

            return View(user);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Profile(User model)
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            var userInDb = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            // Update user properties
            userInDb.Name = model.Name;
            userInDb.Surname = model.Surname;
            userInDb.WalletAdress = model.WalletAdress;
            userInDb.CreationDate = userInDb.CreationDate.ToUniversalTime();
            userInDb.WalletAdress = userInDb.WalletAdress;
            // Mark the entity as modified and save changes
            _context.User.Update(userInDb);
            await _context.SaveChangesAsync();
            

            // Provide feedback to the user
            TempData["Message"] = "Profil başarıyla güncellendi!";
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [AllowAnonymous]
        public IActionResult SendMail()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.Password != HashPassword(password))
            {
                ViewBag.Error = "Şifre ve mail adresinizi kontrol ediniz";
                return View();
            }

            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.Name, user.Name), 
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect("/Home/Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                ViewBag.Error = "Bu mail adresi zaten kullanılmakta";
                return View();
            }

            string hashedpassword = HashPassword(password);
            _context.User.Add(new User
            {
                Name = firstName,
                Surname = lastName,
                Email = email,
                Password = hashedpassword,
                CreationDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, firstName + lastName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect("/Home/Index");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword(string token)
        {
            var user = _context.User.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                ViewBag.Error = "Geçersiz veya süresi dolmuş token.";
                return View();
            }

            return View(new ResetPasswordViewModel { Token = token });
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ResetPasswordViewModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.Error = "Şifreler uyuşmuyor.";
                return View(model);
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.ResetToken == model.Token && u.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                ViewBag.Error = "Geçersiz veya süresi dolmuş token.";
                return View(model);
            }

            var hashedPassword = HashPassword(model.NewPassword);
            user.Password = hashedPassword;
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            // Kullanıcıyı doğrudan giriş yaptır
            return await Login(user.Email, model.NewPassword);
        }



        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> SendMail(string email, [FromServices] EmailService emailService)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Bu mail adresi sistemde kayıtlı değil";
                return View();
            }

            // Token oluştur
            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            // Şifre sıfırlama linkini oluştur
            var resetLink = Url.Action("ForgotPassword", "Home", new { token = user.ResetToken }, Request.Scheme);

            // Mail içeriği
            string emailBody = $@"
    <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 40px 0; text-align: center;'>
        <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: auto; box-shadow: 0px 4px 12px rgba(0, 0, 0, 0.2);'>

            <h2 style='color: #2c3e50; font-size: 24px;'>🔒 Şifre Sıfırlama Talebi</h2>
            
            <p style='font-size: 16px; color: #555;'>
                Sayın Kullanıcımız, <br> 
                Hesabınıza ait şifre sıfırlama işlemi talebinde bulundunuz. 
                Eğer bu talep size aitse, aşağıdaki butona tıklayarak şifrenizi güvenle sıfırlayabilirsiniz.
            </p>

            <p>
                <a href='{resetLink}' 
                   style='display: inline-block; padding: 15px 30px; font-size: 18px; font-weight: bold; color: white; background: linear-gradient(to right, #007bff, #0056b3); text-decoration: none; border-radius: 6px; box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1); margin-top: 20px;'>🔑 Şifremi Sıfırla</a>
            </p>

            <p style='margin-top: 20px; font-size: 14px; color: #777;'>
                ⏳ Bu bağlantı <strong>1 saat</strong> boyunca geçerlidir.  
                <br> Eğer bu işlemi siz yapmadıysanız, lütfen bu e-postayı dikkate almayın veya  
                <a href='mailto:support@tradingwave.com' style='color: #007bff; font-weight: bold;'>destek ekibimizle</a> iletişime geçin.
            </p>

            <hr style='border: 0; height: 1px; background: #ddd; margin: 25px 0;'>

            <p style='font-size: 12px; color: gray;'>© 2024 <strong>TradeWave</strong> | Güvenliğiniz Bizim İçin Önemli</p>
        </div>
    </div>";




        // Mail gönder
        await emailService.SendEmailAsync(user.Email, "Şifre Sıfırlama Talebi", emailBody);

        ViewBag.Message = "Şifre sıfırlama linki e-posta adresinize gönderildi.";
            return Redirect("/Home/Login");
        }


        [HttpPost]
        [Route("Home/AddToWatchlist")]
        public async Task<IActionResult> AddToWatchlist([FromBody] Coin coin)
        {
            var sessionUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var existingCoin = await _context.Watchlistuser.FirstOrDefaultAsync(w => w.UserID == Convert.ToInt32(sessionUserID) && w.CoinName == coin.CoinName);

            if (existingCoin == null)
            {
                _context.Watchlistuser.Add(new Watchlistuser { UserID = Convert.ToInt32(sessionUserID) , CoinSymbol = coin.CoinSymbol , CoinName = coin.CoinName });
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("Coin already in watchlist.");
        }


        [HttpPost]
        [Route("Home/RemoveFromWatchlist")]
        public async Task<IActionResult> RemoveFromWatchlist([FromBody] Coin coin)
        {
            var sessionUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var coinToRemove = await _context.Watchlistuser.FirstOrDefaultAsync(c => c.UserID == Convert.ToInt32(sessionUserID) && c.CoinName == coin.CoinName);
            if (coinToRemove != null)
             {
                _context.Watchlistuser.Remove(coinToRemove);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound("Coin not found in watchlist.");
        }



        [HttpGet]
        [Route("Home/GetWatchlist")]
        public async Task<IActionResult> GetWatchlist()
        {
            var sessionUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var watchlist = await _context.Watchlistuser
                .Where(w => w.UserID == Convert.ToInt32(sessionUserID))
                .ToListAsync();

            return Ok(watchlist);
        }

        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            // Kullanıcının kimliği
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Home");

            var user = _context.User.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return RedirectToAction("Login", "Home");

            // DB'den sil
            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            // Oturumu ve cookieleri temizle
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Login sayfasına yönlendir
            return RedirectToAction("Login", "Home");
        }
        private string HashPassword(string NewPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(NewPassword));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<User> GetCurrentUser()
        {
            // Oturumdaki kullanıcı bilgilerini almak
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return null;  // Kullanıcı kimliği yoksa, kullanıcı oturum açmamış demektir
            }

            // Kullanıcı bilgilerini veritabanından almak
            var user = await _context.User.FindAsync(userId);

            return user;
        }

    }

}
