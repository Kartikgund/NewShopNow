using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Repository;
using NewShopNowBL2.Models;

namespace NewShopNow2.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        StoreRepo storerepo = new StoreRepo();
        ErrorLogRepo ER = new ErrorLogRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult ListStore()
        {
            var lstStores = storerepo.GetAllStores();
            return View(lstStores);
        }

        [Authorize(Roles = "Super Admin,Admin")]
        //Add new Store
        public ActionResult AddStore()
        {
            tblStore objStore = new tblStore();
            return View(objStore);
        }

        public ActionResult SaveStore(tblStore store)
        {
            try
            {
               storerepo.SaveStore(store);
                return RedirectToAction("ListStore");
            }catch (Exception ex)
            {
                string absoluteurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                ER.AddException(ex);
                return View("Error");
            }
        }
        //Update Store Details
        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult Details(int id)
        {
            var objStore = storerepo.FindStoreById(id);
            return View("AddStore", objStore);
        }

        [Authorize(Roles = "Super Admin,Admin")]
        public ActionResult Delete(int id)
        {
            storerepo.DeleteStoreById(id);
            return RedirectToAction("ListStore");
        }
    }
}