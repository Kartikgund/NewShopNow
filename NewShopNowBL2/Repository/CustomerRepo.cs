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
    public class CustomerRepo
    {
        //Select all customers
        public DataSet getAllCustomers()
        {
            DataSet dataSet = new DataSet();
            
            try
            {
                DataTable table1 = new DataTable();
                DataTable table2 = new DataTable();
                string connStr = ConfigurationManager.ConnectionStrings["DPTContext"].ConnectionString;
                SqlConnection con = new SqlConnection(connStr);
                SqlCommand com = new SqlCommand("GetCustomer", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(com);
                
                con.Open();
                da.TableMappings.Add("table1", "tblCustomer");
                da.TableMappings.Add("table2", "tblStock");
                da.Fill(dataSet);
                table1 = dataSet.Tables[0];
                table2 = dataSet.Tables[1];
                con.Close();

                List<tblCustomer> lstCustomer = new List<tblCustomer>();
                lstCustomer = (from DataRow dr in table1.Rows

                           select new tblCustomer()
                           {
                               Id = Convert.ToInt32(dr["Id"]),
                               CustomerName = Convert.ToString(dr["CustomerName"]),
                               MobileNo = Convert.ToString(dr["MobileNo"]),
                               CreatedBy = Convert.ToInt32(dr["CreatedBy"]),
                               CreatedDate = Convert.ToDateTime(dr["CreatedDate"])
                           }).ToList();


                return dataSet;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.StackTrace);
            }

            return null;
        }

        //Add customer
        public bool addCustomer(tblCustomer customer)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                customer.CreatedDate = DateTime.Now;
                customer.CreatedBy = 3;
                context.tblCustomers.AddOrUpdate(customer);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }

        public bool DeleteCustomerById(int id)
        {
            bool result = false;
            tblCustomer objCustomer = new tblCustomer();
            using (DPTContext context = new DPTContext())
            {
                objCustomer = context.tblCustomers.SingleOrDefault(x => x.Id == id);
                context.tblCustomers.Remove(objCustomer);
                context.SaveChanges();
                result = true;
            }

            return result;
        }

        //update
        public tblCustomer getCustomerById(int id)
        {
            tblCustomer objCustomer = new tblCustomer();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                objCustomer = context.tblCustomers.Where(x => x.Id == id).FirstOrDefault();

            }

            return objCustomer;
        }

        public tblCustomer getCustomerByMobileNo(string MobileNo)
        {
            tblCustomer objCustomer = new tblCustomer();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                objCustomer = context.tblCustomers.Where(x => x.MobileNo == MobileNo).FirstOrDefault();

            }

            return objCustomer;
        }
    }
}
