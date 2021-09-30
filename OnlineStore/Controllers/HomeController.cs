using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        public HomeController()
        {
            db = new ApplicationDbContext();
        }
        public ActionResult Index(int page = 0)
        {
            int pageSize = 3;
            ViewBag.previousPage = page - 1 == -1 ? 0 : page - 1;
            ViewBag.nextPage = page + 1;
            MainPageViewModel model = new MainPageViewModel
            {
                section1 = db.Products.Where(p => p.Category.Id == 1).OrderBy(p => p.Id).Skip(page * pageSize).Take(pageSize).ToList(),
                section2 = db.Products.Where(p => p.Category.Id == 17).OrderBy(p => p.Id).Skip(page * pageSize).Take(pageSize).ToList(),
                section3 = db.Products.Where(p => p.Category.Id == 33).OrderBy(p => p.Id).Skip(page * pageSize).Take(pageSize).ToList(),
                BestSeller = db.Products.Where(p => p.IsBestSeller == true).Take(3).ToList()
            };
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Us & description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us page.";

            return View();
        }

        public ActionResult Help()
        {
            ViewBag.Message = "Help page.";

            return View();
        }

        public ActionResult Faqs()
        {
            ViewBag.Message = "Frequantly asked questions page.";

            return View();
        }

        public ActionResult Terms()
        {
            ViewBag.Message = "Terms & Conditions page.";

            return View();
        }

        public ActionResult Privacy()
        {
            ViewBag.Message = "Privacy Policy page.";

            return View();
        }

    }
}