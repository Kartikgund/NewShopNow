using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using NewShopNowBL2.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace NewShopNowBL2.Repository
{
    public class StoreRepo
    {
        public List<tblStore> GetAllStores()
        {
            List<tblStore> lstStores = new List<tblStore>();
            try
            {
                
                string connStr = ConfigurationManager.ConnectionStrings["DPTContext"].ConnectionString;
                SqlConnection con = new SqlConnection(connStr);
                SqlDataAdapter da = new SqlDataAdapter("select * from tblStore", con);
                DataSet ds = new DataSet();
                da.Fill(ds, "tblStore");
                foreach (DataRow row in ds.Tables["tblStore"].Rows)
                {
                    tblStore store = new tblStore();
                    store.Id = (int)row["Id"];
                    store.StoreName = (string)row["StoreName"];
                    store.Address = (string)row["Address"];
                    store.City = (string)row["City"];
                    store.ContactNo = (string)row["ContactNo"];
                    store.CreatedBy = (int)row["CreatedBy"];
                    store.CreatedDate = (DateTime)row["CreatedDate"];
                    store.StartedDate = (DateTime)row["StartedDate"];
                    lstStores.Add(store);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("seomething went wrong" + e);
            }

            return lstStores;
        }

        public bool SaveStore(tblStore store)
        {
            bool result = false;
            using (DPTContext context = new DPTContext())
            {
                store.CreatedDate = DateTime.Now;
                store.CreatedBy = 2;
                store.StartedDate = DateTime.Now;
                context.tblStores.AddOrUpdate(store);
                context.SaveChanges();
                result = true;
            }

            return result;
        }

        public tblStore FindStoreById(int id)
        {
            tblStore objStore = new tblStore();
            using (DPTContext context = new DPTContext())
            {
                objStore = context.tblStores.Where(x => x.Id == id).FirstOrDefault();

            }
            return objStore;
        }

        public bool DeleteStoreById(int id)
        {
            bool result = false;
            tblStore objStore = new tblStore();
            using (DPTContext context = new DPTContext())
            {
                objStore = context.tblStores.Where(x => x.Id == id).FirstOrDefault();
                context.tblStores.Remove(objStore);
                context.SaveChanges();
                result = true;
            }
            return result;
        }
    
}
}
