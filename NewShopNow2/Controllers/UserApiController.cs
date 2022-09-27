using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using NewShopNow2.ViewModel;
using NewShopNowBL2.Models;
using NewShopNowBL2.Repository;
using Newtonsoft.Json;

namespace NewShopNow2.Controllers
{
    public class UserApiController : Controller
    {

        // GET: UserApi
        StoreRepo storeRepo = new StoreRepo();
        UserRepo userRepo = new UserRepo();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AuthenticateUser(tblUser objUser)
        {
            var Email = objUser.EmailId;
            var pass = objUser.Password;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44327/api/");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<string>("User/Authenticate?email="+Email +"&password="+pass,Email);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Session["token"] = result.Content.ReadAsStringAsync().Result;
                    return RedirectToAction("Get");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return RedirectToAction("Login");

        }

        public ActionResult Get()
        {
            
            IEnumerable<tblUser> users = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44327/api/");

                //Called Member default GET All records  
                //GetAsync to send a GET request   
                // PutAsync to send a PUT request
                if (HttpContext.Session["token"] != null)
                {
                    var token = Convert.ToString(HttpContext.Session["token"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var responseTask = client.GetAsync("User/Get");
                responseTask.Wait();

                //To store result of web api response.   
                var result = responseTask.Result;

                //If success received   
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<tblUser>>();
                    readTask.Wait();

                    users = readTask.Result;
                }
                else
                {
                    //Error response received   
                    users = Enumerable.Empty<tblUser>();
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            return View(users);
        }

        public ActionResult AddUser()
        {
            UserAndStore userAndStore = new UserAndStore();
            userAndStore.lstStore = storeRepo.GetAllStores();
            return View(userAndStore);
        }

        [HttpPost]
        public ActionResult Create(UserAndStore userAndStore)
        {
           
            tblUser objUser = userAndStore.objUser;
            objUser.CreatedBy = 2;
            objUser.CreatedDate = DateTime.Now;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44327/api/");

                //HTTP POST
                if (HttpContext.Session["token"] != null)
                {
                    var token = Convert.ToString(HttpContext.Session["token"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var postTask = client.PostAsJsonAsync<tblUser>("User/Post", objUser);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Get");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(objUser);
        }

        public ActionResult UserDetails(int id)
        {
            UserAndStore userAndStore = new UserAndStore();
            userAndStore.lstStore = storeRepo.GetAllStores();


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44327/api/");
                //HTTP GET
                if (HttpContext.Session["token"] != null)
                {
                    var token = Convert.ToString(HttpContext.Session["token"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var responseTask = client.GetAsync("User/Get?id=" + id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<tblUser>();
                    readTask.Wait();

                    userAndStore.objUser = readTask.Result;
                }
            }
            return View(userAndStore);
        }

        public ActionResult EditUserApi(UserAndStore userAndStore)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44327/api/");

                if (HttpContext.Session["token"] != null)
                {
                    var token = Convert.ToString(HttpContext.Session["token"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var putTask = client.PutAsJsonAsync<tblUser>("User/Put", userAndStore.objUser);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Get");
                }
            }
            return View(userAndStore);
        }

        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44327/api/");

                //HTTP DELETE
                if (HttpContext.Session["token"] != null)
                {
                    var token = Convert.ToString(HttpContext.Session["token"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var deleteTask = client.DeleteAsync("User/Delete/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Get");
                }
            }

            return RedirectToAction("Get");
        }
    }
}