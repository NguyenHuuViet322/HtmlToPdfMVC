using LAB3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace LAB3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
                return View();
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("id") != null)
                return View("Index");
            else
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var check = _dbContext.Users.Where(p => p.UserName == user.UserName).FirstOrDefault();
                if (check != null)
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại";
                    return View();
                }
                else
                {
                    user.Password = GetMD5(user.Password);
                    _dbContext.Add(user);
                    _dbContext.SaveChanges();
                    ViewBag.RegisterSuccess = "Bạn đã đăng kí thành công";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.Error = "Thông tin bạn nhập còn sai sót";
                return View();
            }

        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("id") != null)
                return View("Index");
            else
                return View();
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (ModelState.IsValid)
            {
                var passwordEncoded = GetMD5(password);
                var user = _dbContext.Users.Where(p => p.UserName == userName).FirstOrDefault();
                if (user == null)
                {
                    ViewBag.Error = "Tên đăng nhập không tồn tại";
                    return View();
                }
                else
                {
                    if (user.Password == passwordEncoded)
                    {
                        HttpContext.Session.SetInt32("id", user.Id);
                        HttpContext.Session.SetString("username", user.UserName);
                        HttpContext.Session.SetString("name", user.Name);
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.Error = "Mật khẩu chưa chính xác";
                        return View();
                    }

                }

            }
            else
            {
                ViewBag.Error = "Thông tin đăng nhập sai";
                return View();
            }


        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("id");
            HttpContext.Session.Remove("name");
            HttpContext.Session.Remove("username");
            return View("Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;

        }
    }
}