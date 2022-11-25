// Referencing Ajax and Jquery in view 
//uses custom ProductsVM
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using Exam_Prep.Models.DBModels;
using Exam_Prep.Models.ProductsVM;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Exam_Prep.Models;
//using PagedList; 
namespace Exam_Prep.Controllers
{
    public class JsonController : Controller
    {
        public BikeStoresEntities db = new BikeStoresEntities();
           

        // ----------------------------------------------------------custom CRUD-------------------------------------------------------------------------------------------------------\\
        //Actionresults don't work with async they work with Json.
        //the edited version of staffs just using products and Json and showing in models
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            var products = db.products.Include(p => p.brand).Include(p => p.category);
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.product_name.Contains(searchString));
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(products.ToList()); //.ToPagedList(pageNumber, pageSize));
        }

        //------------Edit
        public JsonResult Edit(int? id)
        {
            product product = db.products.Find(id);
            var SerializedProduct = new product //remember to serialise object --- to complex for html/javascrpit
            {
                //Key :value Json standardisaton 
                product_id = product.product_id,
                product_name = product.product_name,
                category_id = product.category_id,
                brand_id = product.brand_id,
                list_price = product.list_price,
                model_year = product.model_year,
                //*** to make these work we added to product table in Model1/Bikestores
                brands = db.brands.ToList().Select(x => new brand { brand_id = x.brand_id, brand_name = x.brand_name }).ToList(), //***   public List<brand> brands { get; set; }
                categories = db.categories.ToList().Select(x => new category { category_id = x.category_id, category_name = x.category_name }).ToList() //***  public List<category> categories { get; set; }
                //*** 
            };
            //WHEN SENDING JSON INFO THIS LINE WILL NEVER FAIL YOU. HELPS GET RID OF ANY LANGUAGE BARRIERS
            return new JsonResult { Data = new { product = SerializedProduct }, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; //again serialise it-- gets used in succes ajax
        }
        //posting 
        [HttpPost]
        public JsonResult Edit([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] product product)
        {
            var message = "";
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "200 OK";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return new JsonResult { Data = new { status = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //------------------details
        public JsonResult Details(int? id)
        {
            product prodInDb = db.products.Where(x => x.product_id == id).FirstOrDefault();
            customDetails newProd = new customDetails();
            newProd.product_name = prodInDb.product_name;
            newProd.model_year = prodInDb.model_year;
            newProd.list_price = prodInDb.list_price;
            newProd.brand_name = prodInDb.brand.brand_name;
            newProd.category_name = prodInDb.category.category_name;
            newProd.storeQuantities = (
                        from stock in db.stocks.ToList()
                        join store in db.stores.ToList() on stock.store_id equals store.store_id
                        where stock.product_id == id
                        group stock by stock.store.store_name into groupedStores
                        select new customStoreQuantity
                        {
                            store_name = groupedStores.Key,
                            quantity = (int)groupedStores.Sum(oi => oi.quantity)
                        }).ToList();
            return new JsonResult { Data = new { product = newProd }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //-----------Delete
        public JsonResult Delete(int? id)
        {
            product prodInDb = db.products.Where(x => x.product_id == id).FirstOrDefault();
            customDetails newProd = new customDetails();
            newProd.product_name = prodInDb.product_name;
            newProd.model_year = prodInDb.model_year;
            newProd.list_price = prodInDb.list_price;
            newProd.brand_name = prodInDb.brand.brand_name;
            newProd.category_name = prodInDb.category.category_name;
            return new JsonResult { Data = new { product = newProd }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            var message = "";
            try
            {
                product product = db.products.Find(id);
                db.products.Remove(product);
                db.SaveChanges();
                message = "200 Ok";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return new JsonResult { Data = new { product = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //-------------- Create
        public ActionResult Create()
        {
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name");
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] product product)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", product.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        //-------------Practice examples------- extra methods
        public JsonResult GetQuantity(int id)
        {//Accessor = public | Return type = JsonResult | Name = GetQuantity | returning an int called id


            //CONVERTING TO INT FROM INT NULLABLE!!!!!! --- (int) -- db.stock
            int qty = (int)db.stocks.Where(x => x.product_id == id).FirstOrDefault().quantity;  //FirstOrDefault is Linq Lambda -- means we are only returning on thing 


            return new JsonResult { Data = new { quantity = qty }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //can reuse the above line. Is format for all Json returns 
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
