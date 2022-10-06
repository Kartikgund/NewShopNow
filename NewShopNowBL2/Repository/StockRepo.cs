using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewShopNowBL2.Models;
using System.Data.Entity.Migrations;
using System.Configuration;
using System.Data.SqlClient;

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

        public List<Object> GetStockData()
        {
            List<Object> stockdata = new List<Object>();
            string connStr = ConfigurationManager.ConnectionStrings["DPTContext"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            string query = "select ProductName,ProductQty from tblstock";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            List<string> pName = new List<string>();
            List<string> pQty = new List<string>();

            
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string name = Convert.ToString(dr["ProductName"]);
                string qty = Convert.ToString(dr["ProductQty"]);
                pName.Add(name);
                pQty.Add(qty);
            }
            stockdata.Add(pName);
            stockdata.Add(pQty);

            return stockdata;
        }


    }
}
