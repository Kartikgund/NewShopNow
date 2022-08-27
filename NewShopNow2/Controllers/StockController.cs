using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Repository;
using NewShopNowBL2.Models;

namespace NewShopNow2.Controllers
{
    public class StockController : Controller
    {
        ErrorLogRepo ER = new ErrorLogRepo();
        StockRepo stockRepo = new StockRepo();
        // GET: Stock
        public ActionResult ListStock()
        {
            var listStocks = stockRepo.getAllProduct();
            return View(listStocks);
        }

        public ActionResult AddProduct()
        {
            tblStock product = new tblStock();
            return View(product);
        }

        public ActionResult SaveProduct(tblStock product)
        {
            try
            {
                stockRepo.saveProduct(product);
                return RedirectToAction("ListStock");
            }
            catch (Exception ex)
            {
                string absoluteurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                ER.AddException(ex, absoluteurl);
                return View("Error");
            }
        }

        //Update Stock
        public ActionResult Details(int id)
        {
            var objStoke = stockRepo.FindProductById(id);
            return View("AddProduct", objStoke);
        }

        public ActionResult Delete(int id)
        {
            stockRepo.DeleteProductById(id);
            return RedirectToAction("ListStock");
        }

        public ActionResult GetAjaxProductList()
        {

                var lstProduct = stockRepo.getAllProduct();
                // var jsonlstProduct = JsonConvert.SerializeObject(lstProduct);
                return Json(new { data = lstProduct }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductById(string Id)
        {
            tblStock stock = stockRepo.FindProductById(Convert.ToInt32(Id));
            return Json(stock, JsonRequestBehavior.AllowGet);
        }
    }
}