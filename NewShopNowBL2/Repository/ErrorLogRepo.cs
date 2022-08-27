using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewShopNowBL2.Models;
using System.Data.Entity.Migrations;
    
namespace NewShopNowBL2.Repository
{
    public class ErrorLogRepo
    {
        public void AddException(Exception ex,string url)
        {
            tblLogError objError = new tblLogError();
            objError.ExceptionURL = url;
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
