using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Models;
using NewShopNowBL2.Repository;
using NewShopNow2.ViewModel;

namespace NewShopNow2.Controllers
{
    public class UserController : Controller
    {
        UserRepo userRepo = new UserRepo();
        StoreRepo storeRepo = new StoreRepo();
        ErrorLogRepo ER = new ErrorLogRepo();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //fetching admin details by id and update data
        public ActionResult UserProfile()
        {
            try
            {
                var user = (tblUser)HttpContext.Session["User"];
                var objUser = userRepo.GetUserDetailsById(user.Id);
                UserAndStore userAndStore = new UserAndStore();
                userAndStore.lstStore = storeRepo.GetAllStores();
                userAndStore.objUser = objUser;
                //objAdmin.Password = repo.Decrypt(objAdmin.Password);
                return View("UserProfile", userAndStore);
            }
           
            catch (Exception ex)
            {
                string absoluteurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                ER.AddException(ex, absoluteurl);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UpdateUserData(UserAndStore userAndStore)
        {
            try
            {
                tblUser objUser = userAndStore.objUser;
                bool result = userRepo.UpdateUser(objUser);
                return RedirectToAction("Index", "Home");
                
            }
            catch(Exception ex)
            {
                string absoluteurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                ER.AddException(ex, absoluteurl);
                return View("Error");
            }
        }

        public ActionResult AddUser()
        {
            UserAndStore userAndStore = new UserAndStore();
            userAndStore.lstStore = storeRepo.GetAllStores();
            //objAdmin.Password = repo.Decrypt(objAdmin.Password);
            return View(userAndStore);
        }

        [HttpPost]
        public ActionResult SaveUser(UserAndStore userAndStore)
        {
            try
            {
                var user = (tblUser)HttpContext.Session["User"];
                tblUser objUser = userAndStore.objUser;
                objUser.CreatedBy = user.Id;
                objUser.CreatedDate = DateTime.Now;
                userRepo.AddUser(objUser);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                string absoluteurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                ER.AddException(ex, absoluteurl);
                return View("Error");
            }
            

        }
        //List cashier
        public ActionResult ListCashier()
        {
            var users = userRepo.GetAllUsers();
            var cashiers = from user in users
                           where user.RoleId == 3
                           select user;
            return View(cashiers);
        }

        //fetching admin details by id and update data
        public ActionResult UserDetails(int id)
        {
            var objUser = userRepo.GetUserDetailsById(id);
            UserAndStore userAndStore = new UserAndStore();
            userAndStore.lstStore = storeRepo.GetAllStores();
            userAndStore.objUser = objUser;
            //objAdmin.Password = repo.Decrypt(objAdmin.Password);
            return View(userAndStore);
        }

        //Delete User
        public ActionResult DeleteUser(int id)
        {
            var result = userRepo.DeleteUserById(id);
            return RedirectToAction("ListCashier");
        }


    }
}