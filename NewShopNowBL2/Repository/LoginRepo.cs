using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using NewShopNowBL2.Models;
using System.Security.Cryptography;
using System.IO;

namespace NewShopNowBL2.Repository
{
    public class LoginRepo
    {
        public tblUser ValidateUser(string emailId, string password)
        {
            tblUser objUser = new tblUser();
            using (DPTContext context = new DPTContext())
            {
                objUser = context.tblUsers.Where(x => x.EmailId == emailId && x.Password == password).FirstOrDefault();
              
                    var userRoles = (from role in context.tblRoles
                                     join user in context.tblUsers
                                     on role.Id equals user.Id
                                     where user.EmailId == emailId
                                     select role.RoleName).ToArray();


            }
            return objUser;
        }

        //******************Encrypt Password********************************** 
        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}
