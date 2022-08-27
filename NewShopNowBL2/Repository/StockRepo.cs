using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewShopNowBL2.Models;
using System.Data.Entity.Migrations;

namespace NewShopNowBL2.Repository
{
    public class StockRepo
    {
        public List<tblStock> getAllProduct()
        {
            List<tblStock> listStock = new List<tblStock>();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                listStock = context.tblStocks.ToList();

            }

            return listStock;
        }
        public bool saveProduct(tblStock product)
        {
            bool result = false;
            using (DPTContext context = new DPTContext())
            {
                product.CreatedDate = DateTime.Now;
                product.CreatedBy = 2;
                context.tblStocks.AddOrUpdate(product);
                context.SaveChanges();
                result = true;
            }

            return result;
        }

        public tblStock FindProductById(int id)
        {
            tblStock objStock = new tblStock();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                objStock = context.tblStocks.Where(x => x.Id == id).FirstOrDefault();

            }
            return objStock;
        }

        public bool DeleteProductById(int id)
        {
            tblStock objStock = new tblStock();
            using (DPTContext context = new DPTContext())
            {
                objStock = context.tblStocks.Where(x => x.Id == id).FirstOrDefault();
                context.tblStocks.Remove(objStock);
                context.SaveChanges();
            }

            return true;
        }
    }
}
