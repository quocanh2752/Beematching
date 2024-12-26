using Beematching.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
        public ActionResult DangKy()
        {
            return View();
        }
        public ActionResult CapNhatHoSo()
        {
            if (TempData["NguoiDungId"] == null)
            {
                return RedirectToAction("Dangnhap"); // Quay lại nếu không có dữ liệu
            }

            int nguoidungId = (int)TempData["NguoiDungId"];
            var nguoidung = _context.Nguoidungs.Find(nguoidungId);

            if (nguoidung == null)
            {
                return RedirectToAction("Dangnhap");
            }

            var nguoixinviec = _context.Nguoixinviecs
                .FirstOrDefault(n => n.IdNguoiDung == nguoidung.Id);

            if (nguoixinviec == null)
            {
                // Tạo hồ sơ mới nếu chưa tồn tại
                nguoixinviec = new Nguoixinviec
                {
                    IdNguoiDung = nguoidung.Id,
                    Email = nguoidung.Email // Lấy email từ thông tin đăng nhập
                };
                _context.Nguoixinviecs.Add(nguoixinviec);
                _context.SaveChanges();
            }

            return View(nguoixinviec);
        }

        [HttpGet]
        public async Task<IActionResult> XemHoSo()
        {
            // Lấy thông tin người dùng hiện tại từ session hoặc context
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập!";
                return RedirectToAction("Dangnhap");
            }

            // Lấy thông tin người dùng
            var nguoiDung = await _context.Nguoidungs
                .Include(nd => nd.Nguoixinviecs)
                .FirstOrDefaultAsync(nd => nd.Email == email);

            if (nguoiDung == null || nguoiDung.Nguoixinviecs == null || !nguoiDung.Nguoixinviecs.Any())
            {
                TempData["Error"] = "Không tìm thấy hồ sơ!";
                return RedirectToAction("CapNhatHoSo");
            }

            // Lấy thông tin người xin việc
            var nguoiXinViec = nguoiDung.Nguoixinviecs.First();

            return View(nguoiXinViec);
        }

        public IActionResult XemCongViecDaUngTuyen(List<Ungtuyen> ungTuyenList)
        {
            return View(ungTuyenList); // Trả về danh sách công việc đã ứng tuyển
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
            // Kiểm tra hồ sơ người xin việc
            var nguoixinviec = _context.Nguoixinviecs
                .FirstOrDefault(n => n.IdNguoiDung == nguoidung.Id);

            if (nguoixinviec == null || string.IsNullOrEmpty(nguoixinviec.SoDienThoai) || string.IsNullOrEmpty(nguoixinviec.AnhHoSo))
            {
                TempData["NguoiDungId"] = nguoidung.Id;

                // Chuyển hướng đến trang cập nhật hồ sơ
                return RedirectToAction("CapNhatHoSo");
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
            TempData["PopupMessage"] = "Đăng nhập thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dangky(string hoTen, string email, string matkhau, string xacnhanMatkhau, string vaiTro)
        {
            // Kiểm tra nếu các trường bị bỏ trống
            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matkhau) || string.IsNullOrEmpty(xacnhanMatkhau) || string.IsNullOrEmpty(vaiTro))
            {
                ModelState.AddModelError("", "Tất cả các trường phải được điền đầy đủ.");
                return View();
            }

            // Kiểm tra xem mật khẩu và xác nhận mật khẩu có khớp không
            if (matkhau != xacnhanMatkhau)
            {
                ModelState.AddModelError("", "Mật khẩu và xác nhận mật khẩu không khớp.");
                return View();
            }

            // Kiểm tra email đã tồn tại trong hệ thống chưa
            var emailTonTai = _context.Nguoidungs.Any(u => u.Email == email);
            if (emailTonTai)
            {
                ModelState.AddModelError("", "Email này đã được sử dụng.");
                return View();
            }

            // Kiểm tra vai trò hợp lệ
            if (vaiTro != "Người dùng" && vaiTro != "Doanh nghiệp")
            {
                ModelState.AddModelError("", "Vai trò không hợp lệ.");
                return View();
            }

            // Tạo người dùng mới
            var nguoidung = new Nguoidung
            {
                HoTen = hoTen,
                Email = email,
                MatKhau = matkhau, // Lưu ý: Cần mã hóa mật khẩu trước khi lưu
                VaiTro = vaiTro,
                TrangThai = "Hoạt động", // Gán trạng thái mặc định là "Hoạt động"
                NgayTao = DateTime.Now // Gán ngày tạo là thời điểm hiện tại
            };

            // Thêm người dùng vào cơ sở dữ liệu
            _context.Nguoidungs.Add(nguoidung);
            await _context.SaveChangesAsync();

            // Tự động đăng nhập sau khi đăng ký thành công
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, nguoidung.HoTen),
                new Claim(ClaimTypes.Email, nguoidung.Email),
                new Claim(ClaimTypes.Role, nguoidung.VaiTro)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            TempData["PopupMessage"] = "Đăng ký thành công!";
            // Chuyển hướng đến trang chủ sau khi đăng ký thành công
            return RedirectToAction("DangNhap");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSo(Nguoixinviec model, IFormFile? anhDaiDien, IFormFile? anhHoSo)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            var nguoixinviec = _context.Nguoixinviecs.Find(model.Id);

            if (nguoixinviec == null)
            {
                return RedirectToAction("Dangnhap");
            }

            // Không cho phép thay đổi email
            model.Email = nguoixinviec.Email;

            // Cập nhật thông tin khác
            nguoixinviec.SoDienThoai = model.SoDienThoai;
            nguoixinviec.NgaySinh = model.NgaySinh;
            nguoixinviec.GioiTinh = model.GioiTinh;

            // Xử lý upload ảnh
            if (anhDaiDien != null)
            {
                var anhDaiDienPath = await SaveFileAsync(anhDaiDien, "AnhDaiDien");
                nguoixinviec.AnhDaiDien = anhDaiDienPath;
            }

            if (anhHoSo != null)
            {
                var anhHoSoPath = await SaveFileAsync(anhHoSo, "AnhHoSo");
                nguoixinviec.AnhHoSo = anhHoSoPath;
            }

            _context.Update(nguoixinviec);
            await _context.SaveChangesAsync();

            TempData["PopupMessage"] = "Cập nhật thành công. Vui lòng đăng nhập lại!";
            return RedirectToAction("Dangnhap");
        }
        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            // Đường dẫn lưu file
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu file vào thư mục
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn tương đối để lưu vào database
            return $"/uploads/{folderName}/{uniqueFileName}";
        }
       


        // Đăng xuất
        public async Task<IActionResult> Dangxuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["PopupMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index");
        }



    }
}
