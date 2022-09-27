using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using NewShopNowBL2.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity;

namespace NewShopNowBL2.Repository
{
    public class UserRepo
    {
        public bool AddUser(tblUser user)
        {
            bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                context.tblUsers.AddOrUpdate(user);
                context.SaveChanges();
                Result = true;
            }

            return Result;
        }

        public bool UpdateUser(tblUser user)
        {
            bool Result = false;
            string connStr = ConfigurationManager.ConnectionStrings["DPTContext"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            string query = "UPDATE TBLUSERS SET USERNAME='"+ user.UserName +"'," +
                "EMailID='" +user.EmailId+"',"+
                "MOBILENO='"+ user.MobileNo + "'," +
                "CITY='" + user.City + "'," +
                "STOREID =" + user.StoreId + "," +
                "ROLEID =" + user.RoleId + " where Id=" + user.Id;

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            try
            {
                cmd.ExecuteNonQuery();
                Result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                con.Close();
            }

            return Result;
        }

        public async Task<List<tblUser>> GetAllUsers()
        {
            List<tblUser> lstUsers = new List<tblUser>();
            using (DPTContext context = new DPTContext())
            {

                lstUsers =await context.tblUsers.ToListAsync();
                return lstUsers;
            }

        }



        public List<tblUser> GetAllAdmin()
        {
            List<tblUser> lstAdmin = new List<tblUser>();
            using (DPTContext context = new DPTContext())
            {

                lstAdmin = context.tblUsers.Where(x => x.RoleId == 2).ToList();
                return lstAdmin;
            }

        }

        public List<tblUser> GetAllCashiers()
        {

            List<tblUser> lstCashiers = new List<tblUser>();
            using (DPTContext context = new DPTContext())
            {
                try
                {
                    lstCashiers = context.tblUsers.Where(x => x.RoleId == 3).ToList();

                    return lstCashiers;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        public tblUser VerifyEmail(string email)
        {
            tblUser objUser = new tblUser();
            using (DPTContext context = new DPTContext())
            {
                objUser = context.tblUsers.Where(x => x.EmailId == email).FirstOrDefault();
            }
            return objUser;
        }

        //add otp obj to db
        public bool AddOtpToDb(tblOTP objOtp)
        {
            bool result = false;
            using (DPTContext context = new DPTContext())
            {
                context.tblOTPs.AddOrUpdate(objOtp);
                context.SaveChanges();
                result = true;
            }

            return result;

        }

        public tblOTP GetObjOtpByEmail(string email)
        {
            tblOTP objOtp = new tblOTP();
            using (DPTContext context = new DPTContext())
            {
                objOtp = context.tblOTPs.Where(x => x.EmailId == email)
                    .OrderByDescending(t => t.Created_DateTime).FirstOrDefault();
            }
            return objOtp;
        }

        public tblUser GetUserDetailsById(int id)
        {
            tblUser objCashier = new tblUser();
            string connStr = ConfigurationManager.ConnectionStrings["DPTContext"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            string query = "select * from tblUsers where Id=" + id;

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                objCashier.Id = Convert.ToInt32(dr["Id"]);
                objCashier.UserName = Convert.ToString(dr["UserName"]);
                objCashier.EmailId = Convert.ToString(dr["EmailId"]);
                objCashier.MobileNo = Convert.ToString(dr["MobileNo"]);
                objCashier.RoleId = Convert.ToInt32(dr["RoleId"]);
                objCashier.StoreId = Convert.ToInt32(dr["StoreId"]);
                objCashier.City = Convert.ToString(dr["City"]);
                objCashier.Password = Convert.ToString(dr["Password"]);
                objCashier.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                objCashier.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
            }


            return objCashier;
        }

        public bool DeleteUserById(int id)
        {
            bool result = false;
            using (DPTContext context = new DPTContext())
            {
                var objCashier = context.tblUsers.SingleOrDefault(x => x.Id == id);
                context.tblUsers.Remove(objCashier);
                context.SaveChanges();
                result = true;
            }

            return result;
        }
    }
}
