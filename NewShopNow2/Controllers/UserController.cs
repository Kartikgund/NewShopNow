using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Models;
using NewShopNowBL2.Repository;
using NewShopNow2.ViewModel;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using System.Net.Http;

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

        [Authorize(Roles = "Admin, Cashier")]
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
                ER.AddException(ex);
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
                ER.AddException(ex);
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin")]
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
                ER.AddException(ex);
                return View("Error");
            }
            

        }

        [Authorize(Roles = "Admin")]
        //List cashier
        public ActionResult ListCashier()
        {
            var users = Task.Run(async () => await userRepo.GetAllUsers()).Result;

            var cashiers = from user in users
                           where user.RoleId == 3
                           select user;
            return View(cashiers);
        }

        //fetching details by id and update data
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUser(int id)
        {
            var result = userRepo.DeleteUserById(id);
            return RedirectToAction("ListCashier");
        }
       
        /*public FileResult Export()
        {
          //  NorthwindEntities entities = new NorthwindEntities();
            DataTable dt = new DataTable("Users");
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("Id"),
                                            new DataColumn("UserName"),
                                            new DataColumn("MobileNo"),
                                            new DataColumn("EmailId"),
                                            new DataColumn("City"),
                                            new DataColumn("RoleId"),
                                            new DataColumn("StoreId"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate")});

            var users = Task.Run(async () => await userRepo.GetAllUsers()).Result;

            foreach (var user in users)
            {
                dt.Rows.Add(user.Id, user.UserName, user.MobileNo, user.EmailId,user.City,
                    user.RoleId,user.StoreId,user.CreatedBy,user.CreatedDate);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.csv");
                }
            }
        }*/

        
        
    }
}