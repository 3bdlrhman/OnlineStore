using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OnlineStore.Models;
using PayPal.Api;

namespace OnlineStore.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db;
        public OrdersController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Checkout()
        {
            // Get the User
            var UserId = User.Identity.GetUserId();
            // Get the active order  shoppingCart
            var ActiveOrder = db.Orders.Where(o => o.User.Id == UserId).Where(o => o.IsActive).FirstOrDefault();
            if (ActiveOrder == null)
                return HttpNotFound("You Have No Orders Yet");

            // create a new shipping 

            return View(ActiveOrder.Items);
        }

        public ActionResult AddOrderItem(int? Id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user == null)
                return Json("Login First", JsonRequestBehavior.AllowGet);

            var product = db.Products.Find(Id);
            var ExistingOrder = db.Orders.Where(o => o.User.Id == user.Id).Where(o => o.IsActive).FirstOrDefault();

            if (ExistingOrder == null || ExistingOrder.Deliverd || !ExistingOrder.IsActive) // then create new order
            {
                OrderItem orderItem = new OrderItem { Product = product, Quantity = 1 };
                Models.Order order = new Models.Order { IsActive = true, OrderDate = DateTime.UtcNow, FullName = user.FirstName + " " + user.LastName, MobileNumber = user.PhoneNumber, User = user, Items = new OrderItem[] { orderItem } };
                db.OrderItems.Add(orderItem);
                db.Orders.Add(order);
                db.SaveChanges();
            }
            else // then add order item to the existing order
            {
                OrderItem orderItem = db.OrderItems.Where(i => i.Product.Id == Id && i.order.Id == ExistingOrder.Id).FirstOrDefault();
                if (orderItem != null)
                {
                    orderItem.Quantity += 1;
                    db.SaveChanges();
                }
                else
                {
                    OrderItem newOrderItem = new OrderItem { Product = product, Quantity = 1 };
                    db.OrderItems.Add(newOrderItem);
                    ExistingOrder.Items.Add(newOrderItem);
                    db.SaveChanges();
                }
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveOrderItem(int? id, int? count)
        {
            var UserId = User.Identity.GetUserId();
            var ExistingOrder = db.Orders.Where(o => o.User.Id == UserId).Where(o => o.IsActive).FirstOrDefault();

            if (ExistingOrder == null)
                return Json("fail", JsonRequestBehavior.AllowGet);

            var item = db.OrderItems.Where(oi => oi.Product.Id == id && oi.order.Id == ExistingOrder.Id).FirstOrDefault();
            if (item == null)
                return Json("fail", JsonRequestBehavior.AllowGet);

            if (item.Quantity > 1 && count.HasValue && count.Value == 1)
            {
                item.Quantity = item.Quantity - 1;
                db.SaveChanges();
            }
            else
            {
                db.OrderItems.Remove(item);
                db.SaveChanges();
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int page = 0)
        {
            int pageSize = 10;

            var orders = db.Orders.OrderBy(o => o.Id).Skip(page * pageSize).Take(pageSize);
            ViewBag.previousPage = page - 1 == -1 ? 0 : page - 1;
            ViewBag.nextPage = page + 1;

            return View(orders);
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,OrderDate,FullName,MobileNumber,City,Address")] Models.Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Models.Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Payment(Shippings shippings)
        {
            try
            {
                var UserId = User.Identity.GetUserId();
                var user = db.Users.Find(User.Identity.GetUserId());
                var ExistingOrder = db.Orders.Where(o => o.User.Id == UserId).Where(o => o.IsActive).FirstOrDefault();
                ExistingOrder.IsActive = false;
                db.Shippings.Add(shippings);
                db.SaveChanges();

                shippings.User = user;
                shippings.Order = ExistingOrder;
                db.SaveChanges();

                if (shippings.PaymentMethod == "paypal")
                    return RedirectToAction("PaymentWithPaypal");
                ViewBag.id = ExistingOrder.Id;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        // PAYPAL : Payment options
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Home/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = ex;
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Nokia T400",
                currency = "USD",
                price = "1",
                quantity = "1",
                sku = "sku"
            });
            
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = "1"
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = "3", // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "This should be done by next day",
                invoice_number = "392751120", //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }
        // PAYPAL : End

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
