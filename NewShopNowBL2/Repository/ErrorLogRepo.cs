using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewShopNowBL2.Models;
using System.Data.Entity.Migrations;
using context = System.Web.HttpContext;
    
namespace NewShopNowBL2.Repository
{
    public class ErrorLogRepo
    {
        public void AddException(Exception ex)
        {
            tblLogError objError = new tblLogError();
            objError.ExceptionURL = context.Current.Request.Url.AbsolutePath;
            objError.ExceptionType = ex.GetType().Name.ToString();
            objError.ExceptionSource = ex.ToString();
            objError.ExceptionMsg = ex.Message.ToString();
            objError.Logdate = DateTime.Now;
            using (DPTContext context = new DPTContext())
            {
                context.tblLogErrors.AddOrUpdate(objError);
                context.SaveChanges();
             
            }

          
        }
    }
}
