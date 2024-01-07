using System;
using System.Linq;
using System.Web.Mvc;
using YSoftHrSystem.Models;
using YSoftHrSystem.ViewModels;

namespace YSoftHrSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ReCapProjectDBContext _dbContext;

        public AccountController()
        {
            _dbContext = new ReCapProjectDBContext();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Session["UserId"] = user.Id;
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View();
        }

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı kaydını gerçekleştir
                _dbContext.Users.Add(new User
                {
                    // Diğer kullanıcı bilgileri buraya eklenir
                    Username = model.Username,
                    // Password değerini kontrol et ve atama yap
                    Password = !string.IsNullOrEmpty(model.Password) ? model.Password : throw new ArgumentNullException(nameof(model.Password), "Password cannot be null or empty.")
                });
                _dbContext.SaveChanges();

                // Kullanıcı başarıyla kaydedildi, giriş yapabilirsiniz veya başka bir sayfaya yönlendirebilirsiniz.
                return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendirme
            }

            // Model geçerli değilse, formu tekrar göster
            return View(model);
        }
        [HttpGet]
        public ActionResult TazminatHesapla()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Hesapla(string employeeName)
        {
            // Kullanıcı adına göre veritabanından giriş yılı bilgisini çek
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == employeeName);

            if (user != null && int.TryParse(user.HireYear, out int hireYear))
            {
                // Şuanki yılı al
                int currentYear = DateTime.Now.Year;

                // Çalışma yılı hesapla
                int yearsOfWork = currentYear - hireYear;

                // Tazminatı hesapla (örnek katsayı: 20000)
                decimal tazminat = yearsOfWork * 20000;

                string result = $"{employeeName} adlı çalışanın {yearsOfWork} yıllık tazminatı: {tazminat} TL";

                // Burada "TazminatHesapla" view'ını çağırarak sonucu gösteriyoruz.
                return View("TazminatHesapla", (object)result);
            }

            // Kullanıcı bulunamadıysa veya HireYear dönüşümü başarısız olduysa
            string notFoundResult = $"{employeeName} adlı kullanıcı bulunamadı";

            // Burada "TazminatHesapla" view'ını çağırarak sonucu gösteriyoruz.
            return View("TazminatHesapla", (object)notFoundResult);
        }




    }
}
