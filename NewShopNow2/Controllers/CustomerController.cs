using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Models;
using NewShopNowBL2.Repository;

namespace NewShopNow2.Controllers
{
    public class CustomerController : Controller
    {
        CustomerRepo customerRepo = new CustomerRepo();
        // GET: Customer
        public ActionResult ListCustomer()
        {
            var list = customerRepo.getAllCustomers();
            return View(list);
        }

        public ActionResult GetCustomerByNumber(string MobileNo)
        {
            tblCustomer customer = customerRepo.getCustomerByMobileNo(MobileNo);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
    }
}