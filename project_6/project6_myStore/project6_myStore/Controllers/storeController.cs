using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project6_myStore.Models;

namespace project6_myStore.Controllers
{
    public class storeController : Controller
    {
        private myStoreEntities db = new myStoreEntities();

        public ActionResult login_signup()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        //public ActionResult login(string email, string password)
        //{
        //    var logged_user = db.users.FirstOrDefault(u => u.email == email && u.password == password);

        //    if (logged_user == null)
        //    {
        //        ViewBag.error = "Please Make Sure To Enter Your Info Correctly";
        //        return View("login_signup");
        //    }

        //    Session["logged_user"] = logged_user;
        //    var cart = db.carts.FirstOrDefault(l => l.user_id == logged_user.id);
        //    //var cartItems1 = db.cart_item
        //    //.Where(l => l.cart_id == cart.id).ToList();

        //    //Session["cartItems1"] = cartItems1;


        //    return RedirectToAction("Index", "Home");
        //}
        public ActionResult login(string email, string password)
        {
            var logged_user = db.users.FirstOrDefault(u => u.email == email && u.password == password);


            if (logged_user == null)
            {
                ViewBag.error = "Please Make Sure To Enter Your Info Correctly";
                return View("login_signup");
            }

            Session["logged_user"] = logged_user;
            Session["UserID"] = logged_user.id;


            var cart = db.carts.FirstOrDefault(l => l.user_id == logged_user.id);

            // Ensure the cart exists before proceeding
            if (cart != null)
            {
                var cartItems1 = db.cart_item
                    .Where(l => l.cart_id == cart.id)
                    .ToList(); // Execute the query and store the results in a list

                Session["cartItems1"] = cartItems1;
            }
            else
            {
                Session["cartItems1"] = new List<project6_myStore.Models.cart_item>(); // Store an empty list if no cart exists
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult signup(user users_new,string email, string password, string cofirm_pass)
        {
            var newUser = db.users.FirstOrDefault(u => u.email == email);

            if (newUser != null)
            {
                ViewBag.usedEmail = "The Email is already used for an account please Login";
                return View("login_signup");
            }
            else
            {
                if (password == cofirm_pass)
                {
                    users_new.email = email;
                    users_new.password = password;

                    db.users.Add(users_new);
                    db.SaveChanges();

                    cart newCart = new cart();
                    newCart.user_id = users_new.id;
                    db.carts.Add(newCart);
                    db.SaveChanges();

                    Session["logged_user"] = users_new;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.usedEmail = "Please make sure to enter the same Password";
                    return View("login_signup");
                }
            }
        }
        public ActionResult profile()
        {
            user user = Session["logged_user"] as user;
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);

        }
        [HttpPost]
        public ActionResult profile([Bind(Include = "firstName,lastName,,address")] user user)
        {

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("profile");
            }
            return View(user);


        }

            public ActionResult shopAll()
        {
            var shopALL = db.products.ToList();
            return View(shopALL);
        }
            public ActionResult shop(int? id)
        {
            var shopByCategory = db.products
            .Where(l => l.category_id == id).ToList();

            return View(shopByCategory);
        }

        public ActionResult shop_details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product details = db.products.Find(id);
            if (details == null)
            {
                return HttpNotFound();
            }
            return View(details);
        }
       
        public ActionResult addToCart(int? id, string size, int quantity)
        {
            var user = Session["logged_user"] as user;

            if(user == null)
            {
                TempData["notLogedin"] = true;
                return RedirectToAction("shop_details", new { id = id });
            }
            else
            {
                var cart = db.carts.FirstOrDefault(l => l.user_id == user.id);
                if(cart == null)
                {
                    cart newCart = new cart();
                    newCart.user_id = user.id;
                    db.carts.Add(newCart);
                    db.SaveChanges();

                    cart_item cart_Item = new cart_item();
                    cart_Item.size = size;
                    cart_Item.quantity = quantity;
                    cart_Item.product_id = id;
                    cart_Item.cart_id = newCart.id;
                    db.cart_item.Add(cart_Item);
                    db.SaveChanges();

                    TempData["Success"] = true;
                    return RedirectToAction("shop_details", new { id = id });
                }
                else
                {
                    cart_item cart_Item = new cart_item();
                    cart_Item.size = size;
                    cart_Item.quantity = quantity;
                    cart_Item.product_id = id;
                    cart_Item.cart_id = cart.id;
                    db.cart_item.Add(cart_Item);
                    db.SaveChanges();

                    TempData["Success"] = true;
                    return RedirectToAction("shop_details", new { id = id });

                }
            }  
        }

        public ActionResult cart()
        {
            var user = Session["logged_user"] as user;
            var cart = db.carts.FirstOrDefault(l => l.user_id == user.id);


            var cartItems = db.cart_item
            .Where(l => l.cart_id == cart.id).ToList();

            var cartItems1 = db.cart_item
            .Where(l => l.cart_id == cart.id).ToList();

            Session["cartItems1"] = cartItems1;

            return View(cartItems);

            
            
        }

        public ActionResult DeleteCartItem(int id)
        {

            cart_item item = db.cart_item.Find(id);
            db.cart_item.Remove(item);
            db.SaveChanges();
            return RedirectToAction("cart");
            
        }

        [HttpPost]
        public ActionResult updateCartItem(int? id, int quantity)
        {

            cart_item item = db.cart_item.Find(id);
            item.quantity = quantity;

            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("cart");

        }

        public ActionResult CheckOut()
        {
            var user = Session["logged_user"] as user;
            var cart = db.carts.FirstOrDefault(l => l.user_id == user.id);


            var cartItems = db.cart_item
            .Where(l => l.cart_id == cart.id).ToList();

            return View(cartItems);
        }

        [HttpPost]
        public ActionResult CheckOut(decimal amount, decimal shipping)
        {
            var user = Session["logged_user"] as user;

            transaction newtrans = new transaction();
            newtrans.amount = amount + shipping;
            newtrans.status = "Shipped";
            newtrans.paypal_transaction_id = "1";
            db.transactions.Add(newtrans);
            db.SaveChanges();

            order newOrder = new order();
            newOrder.user_id = user.id;
            newOrder.transaction_id = newtrans.id;
            db.orders.Add(newOrder);
            db.SaveChanges();

            var cart = db.carts.FirstOrDefault(l => l.user_id == user.id);


            var cartItems = db.cart_item
            .Where(l => l.cart_id == cart.id);

            foreach(var item in cartItems)
            {
                order_item order_Item = new order_item();
                order_Item.product_id = item.product_id;
                order_Item.quantity = item.quantity;
                order_Item.size = item.size;
                order_Item.order_id = newOrder.id;
                db.order_item.Add(order_Item);
                

                db.cart_item.Remove(item);
                

            }
            db.SaveChanges();
            return RedirectToAction("Index", "Home");



        }

























        // GET: store
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        // GET: store/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: store/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: store/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,firstName,lastName,email,password,img,address,type")] user user)
        {
            if (ModelState.IsValid)
            {
                db.users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: store/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: store/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "firstName,lastName,address")] user user)
        {

            var user2 = db.users.Find((int)Session["UserID"]);
            user2.firstName = user.firstName;
            user2.lastName = user.lastName; 
            user2.address = user.address;
            
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            
            
        }

        // GET: store/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: store/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
