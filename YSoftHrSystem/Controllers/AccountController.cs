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
            // Kullanıcı girişini kontrol et
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Kullanıcı doğrulandı, giriş başarılı
                // Session, Cookie veya Identity kullanarak giriş yapmış olarak işaretleyebilirsiniz.
                return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendirme
            }

            // Kullanıcı adı veya şifre hatalı
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");

            // Yanlış giriş durumunda view'e geri dön
            return View();
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

    }
}
