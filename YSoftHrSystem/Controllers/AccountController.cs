using Microsoft.EntityFrameworkCore.Diagnostics;
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
                    Email = model.Email,
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
        public ActionResult ComputeCompensation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ComputeCompensation(string employeeName)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == employeeName);

            if (user != null && int.TryParse(user.HireYear, out int hireYear))
            {
                int currentYear = DateTime.Now.Year;

                int yearsOfWork = currentYear - hireYear;

                decimal tazminat = yearsOfWork * 20000;

                string result = $"{employeeName} adlı çalışanın {yearsOfWork} yıllık tazminatı: {tazminat} TL";

                return View("ComputeCompensation", (object)result);
            }

            string notFoundResult = $"{employeeName} adlı kullanıcı bulunamadı";

            return View("ComputeCompensation", (object)notFoundResult);
        }

        [HttpGet]
        public ActionResult ComputeSalary()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ComputeSalary(string employeeName)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == employeeName);
            string notFoundResult = "";

            if (user != null)
            {
                if (user.Salary == null)
                {
                    string levelofTitle = user.Title;

                    switch (levelofTitle)
                    {
                        case "Level1":
                            user.Salary = "10000";
                            break;
                        case "Level2":
                            user.Salary = "20000";
                            break;
                        case "Level3":
                            user.Salary = "30000";
                            break;
                        case "Level4":
                            user.Salary = "40000";
                            break;

                        default:
                            return View("Maaş Hesaplanırken Bir Hata Oluştu");
                    }
                    _dbContext.SaveChanges();
                    string result = $"{employeeName} adlı çalışanın maaşı hesaplanmıştır. VeriTabanına kaydedilmiştir.";

                    return View("ComputeSalary", (object)result);

                }
                notFoundResult = "Maaş daha önceden hesaplanmıştır.";
            }
            else 
            {
                notFoundResult = $"{employeeName} adlı kullanıcı bulunamadı";
            }

            return View("ComputeSalary", (object)notFoundResult);
        }




    }
}
