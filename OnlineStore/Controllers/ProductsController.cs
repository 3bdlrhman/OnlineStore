using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        public ActionResult Index(int page=0)
        {
            int pageSize = 10;
            var products = db.Products.OrderBy(o => o.Id).Skip(page * pageSize).Take(pageSize);
            ViewBag.previousPage = page - 1 == -1 ? 0 : page - 1;
            ViewBag.nextPage = page + 1;
            return View(products);
        }

        [AllowAnonymous]
        // GET: Products
        public ActionResult ProductByCategory(string category)
        {
            Category selectedCategory;
            ICollection<Product> allProducts;

            selectedCategory = db.Categories.Where(c => c.Name.Contains(category)).FirstOrDefault();
            if (selectedCategory == null)
                return HttpNotFound();
            allProducts = db.Products.Where(p => p.Category.Id == selectedCategory.Id).ToList();
            if(allProducts == null)
                return HttpNotFound();

            return View(allProducts);
        }

        [AllowAnonymous]
        public ActionResult ProductByFilter(string filter, string value)
        {
            ICollection<Product> products;
            try
            {
                switch (filter)
                {
                    case "Price":
                        int v = int.Parse(value);
                        products = db.Products.Where(p => p.Price <= v && p.Price >= (v / 2)).ToList();
                        break;
                    case "CustomerReview":
                        var ratings = Decimal.Parse(value);
                        products = db.Products.Where(p => p.Rating >= ratings).ToList();
                        break;
                    case "Discount":
                        var DisRate = Decimal.Parse(value);
                        products = db.Products.Where(p => p.Discount == DisRate).ToList();
                        break;
                    case "Electronics":
                        products = db.Products.Where(p => p.Category.Name.ToLower().Contains(value)).ToList();
                        break;
                    case "new":
                        products = db.Products.Where(p => p.IsNewArrival).ToList();
                        break;
                    case "FreeDelivery":
                        products = db.Products.Where(p => p.IsFreeDelivery).ToList();
                        break;
                    default:
                        products = db.Products.ToList();
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View("ProductByCategory", products);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SearchProducts(string searchString)
        {
            // if the serach string is a category name
            var selectedCategory = db.Categories.Where(c => c.Name.Contains(searchString)).FirstOrDefault();
            if (selectedCategory != null)
            {
                var allProducts = db.Products.Where(p => p.Category.Id == selectedCategory.Id);
                if (allProducts == null)
                    return HttpNotFound();
                return View("ProductByCategory", allProducts.ToList());
            }
            var products = db.Products.Where(p => p.Name.Contains(searchString));

            return View("ProductByCategory", products.ToList());
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Details,img,Price,Discount,Rating,Quantity,IsNewArrival,IsFreeDelivery,IsFeatured,IsBestSeller")] Product product, string category, HttpPostedFileBase img)
        {
            var selectedCategory = db.Categories.Where(c => c.Name.ToLower().Contains(category.Trim().ToLower())).FirstOrDefault();
            if (selectedCategory == null)
            {
                db.Categories.Add(new Category { Name = category });
                db.SaveChanges();
                selectedCategory = db.Categories.Where(c => c.Name.ToLower().Contains(category.Trim().ToLower())).FirstOrDefault();
            }
            if (ModelState.IsValid)
            {
                string imgName;
                if (img != null && img.ContentLength > 0)
                {
                    // extract only the filename
                    imgName = Path.GetFileName(img.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/images"), imgName);
                    img.SaveAs(path);
                    product.img = imgName;
                }

                db.Products.Add(product);
                db.SaveChanges();
                addProductToCategory(product, selectedCategory);
                return RedirectToAction("Index");
            }

            return View(product);
        }

        private void addProductToCategory(Product product, Category categoryItem)
        {
            try
            {
                categoryItem.Products.Add(product);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Details,img,Price,Discount,Rating,Quantity,IsNewArrival,IsFreeDelivery,IsFeatured,IsBestSeller")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}