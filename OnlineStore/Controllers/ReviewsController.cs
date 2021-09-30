using Microsoft.AspNet.Identity;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Controllers
{
    public class ReviewsController : Controller
    {
        ApplicationDbContext db;
        public ReviewsController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Reviews
        public ActionResult GetReview(int id)
        {
            var reviews = db.Reviews.Where(r => r.ProductId == id).ToList();
            
            return Json(reviews, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddReview(string comment, int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var review = new Reviews { ProductId = id, Comment = comment, Rate = 4.4M, user= user };
            db.Reviews.Add(review);
            db.SaveChanges();
            return Json(review);
        }
    }
}