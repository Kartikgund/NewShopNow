using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Models;
using NewShopNowBL2.Repository;
using NewShopNow2.ViewModel;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web.Security;

namespace NewShopNow2.Controllers
{
    public class HomeController : Controller
    {
        LoginRepo repo = new LoginRepo();
        UserRepo UR = new UserRepo();
        StoreRepo SR = new StoreRepo();
        StockRepo stockRepo = new StockRepo();
        ErrorLogRepo ER = new ErrorLogRepo();
        CustomerRepo customerRepo = new CustomerRepo();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AuthenticateUser(tblUser objUser)
        {
            string pass = repo.encrypt(objUser.Password);
            var user = repo.ValidateUser(objUser.EmailId, pass);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.EmailId,false);
                Session["User"] = user;
                Session["UserName"] = user.UserName;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.InvalidUser = "Invalid User";
                return View("Login");
            }

        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Login");
        }

         //Register
        public ActionResult RegisterUser()
        {
            UserAndStore userAndStore = new UserAndStore();
            userAndStore.lstStore = SR.GetAllStores();
            return View(userAndStore);
        }

        [HttpPost]
        public ActionResult SaveAdmin(UserAndStore userAndStore)
        {
            tblUser objAdmin = userAndStore.objUser;
            objAdmin.CreatedDate = DateTime.Now;
            objAdmin.CreatedBy = 1;
            objAdmin.RoleId = 2;
            objAdmin.Password = repo.encrypt(objAdmin.Password);
            bool result = UR.AddUser(objAdmin);
            if (result == true)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.InvalidUser = "Something went wrong Try Again";
                return RedirectToAction("RegisterUser");
            }

        }

        //Reset password using OTP verification
        public ActionResult ForgetPassword()
        {
            return View();
        }

        public ActionResult VerifyEmail(string email)
        {
            tblUser user = UR.VerifyEmail(email);
           
            bool result=false;
            if (user != null)
            {
                tblOTP objOtp = new tblOTP();
                Random r = new Random();
                int genRand = r.Next(100000, 999999);
                objOtp.EmailId = user.EmailId;
                objOtp.OTP = genRand;
                objOtp.IsUsed = 0;
                objOtp.Created_DateTime = DateTime.Now;
                result = UR.AddOtpToDb(objOtp);

                if (result)
                {
                    try
                    {
                        var senderEmail = new MailAddress("kartikgund2@gmail.com", "Kartik");
                        var receiverEmail = new MailAddress(user.EmailId, "Receiver");
                        var password = "rgpljlpevjrezdpq";
                        var sub = "OTP for Reset Password";
                        var body = "Dear " + user.UserName +  " <b>" + genRand + "</b> is your One-Time-Password. It is valid for 10 mins.";

                        MailMessage message = new MailMessage();
                        message.To.Add(user.EmailId);// Email-ID of Receiver  
                        message.Subject = sub;// Subject of Email  
                        message.From = senderEmail;// Email-ID of Sender  
                        message.IsBodyHtml = true;
                       
                        message.Body = body;
                        SmtpClient SmtpMail = new SmtpClient();

                        SmtpMail.Host = "smtp.gmail.com";
                        SmtpMail.Port = 587;
                        SmtpMail.EnableSsl = true;
                        SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                        SmtpMail.UseDefaultCredentials = false;
                        SmtpMail.Credentials = new NetworkCredential(senderEmail.Address, password);
                        SmtpMail.Send(message);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

            }
             return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyOTP(string email,string otp)
         {
            tblOTP objotp = UR.GetObjOtpByEmail(email);

            string result = "";
            //   int time = Convert.ToInt32(ts);

            if (objotp.IsUsed == 0)
            {
                TimeSpan ts = DateTime.Now - objotp.Created_DateTime;
                if (ts.Minutes <= 10)
                {
                    if (objotp.OTP == Convert.ToInt32(otp))
                    {
                        objotp.IsUsed = 1;
                        UR.AddOtpToDb(objotp);
                        result = "Valid OTP";
                    }
                    else
                    {
                        result = "Invalid OTP";
                    }
                }
                else
                {
                    result = "OTP expired";
                }
            }
            else
            {
                result = "OTP Already Used";
            }
           

           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetPassword(string email)
        {
            tblUser objUser = UR.VerifyEmail(email);
            ViewBag.EmailId = objUser.EmailId;
            return PartialView("_ResetPassword");
        }

        public ActionResult SavePassword(string email,string password1)
        {
            tblUser objUser = UR.VerifyEmail(email);
           
            objUser.Password = repo.encrypt(password1);
            UR.AddUser(objUser);
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult WriteFile()
        {
            return View();
        }

        public ActionResult SaveFile(string txtfield)
        {
            string folder = ConfigurationManager.AppSettings["Path"].ToString();
            string path = folder+"LogFile_"+ @Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy")+".txt";
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.CreateText(path).Dispose();
               
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = System.IO.File.AppendText(path))
            {
                sw.WriteLine(txtfield);
               
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Super Admin")]
        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadCustomers(string groupId)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    DataSet ds = new DataSet();
                    HttpPostedFileBase files = Request.Files[0];
                    string fileName = "CustomerUpload_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var path = ConfigurationManager.AppSettings["CustomerDocuments"].ToString();
                    var fullFilePath = path + "\\" + fileName;
                    files.SaveAs(path + "\\" + fileName);

                    string conString = string.Empty;

                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullFilePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;
                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(ds);
                                connExcel.Close();
                            }
                        }
                    }
                 /*  ds.Tables[0].Columns.Add("GroupId");
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        dr["GroupId"] = groupId;
                    }
*/
                    var status = customerRepo.BulkInsert(ds.Tables[0]);

                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    ER.AddException(ex);
                }

            }
            return Json("File Not Uploaded Successfully!");

        }

        [Authorize(Roles = "Admin, Super Admin")]
        public FileResult Export(string type)
        {
            
            DataTable dt = new DataTable("Grid");

            switch (type)
            {
                case "tblUser":
                    {
                        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(tblUser));
                        foreach (PropertyDescriptor prop in properties)
                        {
                            if (!prop.Name.Equals("tblUser"))
                            {
                                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                                                            
                            }
                        }

                        var items = Task.Run(async () => await UR.GetAllUsers()).Result;

                        foreach (var item in items)
                        {
                            var values = new object[properties.Count];
                            for (int i = 0; i < properties.Count; i++)
                            {
                                values[i] = properties[i].GetValue(item);
                            }
                            dt.Rows.Add(values);
                        }
                        break;
                    }

                case "tblStore":
                    {
                        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(tblStore));
                        foreach (PropertyDescriptor prop in properties)
                        {
                            if (!prop.Name.Equals("tblStore"))
                            {
                                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                            }
                        }

                        var items = SR.GetAllStores();

                        foreach (var item in items)
                        {
                            var values = new object[properties.Count];
                            for (int i = 0; i < properties.Count; i++)
                            {
                                values[i] = properties[i].GetValue(item);
                            }
                            dt.Rows.Add(values);
                        }
                        break;
                    }

                case "tblStock":
                    {
                        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(tblStock));
                        foreach (PropertyDescriptor prop in properties)
                        {
                            if (!prop.Name.Equals("tblStock") && !prop.Name.Equals("tblTransactionItems"))
                            {
                                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                            }
                        }

                        var items = stockRepo.getAllProduct();

                        foreach (var item in items)
                        {
                            var values = new object[dt.Columns.Count];
                            for (int i = 0; i < properties.Count; i++)
                            {
                                if(properties[i].DisplayName != "tblTransactionItems")
                                {
                                    values[i] = properties[i].GetValue(item);
                                }
                                
                            }
                            dt.Rows.Add(values);
                        }
                        break;
                    }

            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", type+".xls");
                }
            }
        }
    }
}