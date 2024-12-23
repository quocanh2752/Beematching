using Beematching.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Beematching.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BeematchingContext _context;

        public HomeController(ILogger<HomeController> logger, BeematchingContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)  // Kiểm tra xem người dùng đã đăng nhập chưa
            {
                var userName = User.Identity.Name;  // Lấy tên người dùng từ claims
                ViewBag.Message = $"Xin chào, {userName}";
            }
            else
            {
                ViewBag.Message = "Xin chào, khách!";
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dangnhap(string email, string matkhau)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matkhau))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không được để trống.");
                return View();
            }

            // Kiểm tra thông tin đăng nhập
            var nguoidung = _context.Nguoidungs
                .FirstOrDefault(u => u.Email == email && u.MatKhau == matkhau);

            if (nguoidung == null)
            {
                ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ.");
                return View();
            }

            // Tạo thông tin đăng nhập cho người dùng
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, nguoidung.HoTen),
                new Claim(ClaimTypes.Email, nguoidung.Email),
                new Claim(ClaimTypes.Role, nguoidung.VaiTro)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Đăng nhập và lưu trữ thông tin người dùng trong cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Chuyển hướng đến trang chủ hoặc trang khác sau khi đăng nhập thành công
            return RedirectToAction("Index");
        }

        // Đăng xuất
        public async Task<IActionResult> Dangxuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "Đăng xuất thành công!";
            return RedirectToAction("Index");
        }



    }
}
