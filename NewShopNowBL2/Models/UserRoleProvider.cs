using System;
using System.Linq;
using System.Web.Security;

namespace NewShopNowBL2.Models
{
    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string EmailId)
        {
            Console.WriteLine(EmailId);
            using (DPTContext context = new DPTContext())
            {
                var user = context.tblUsers.FirstOrDefault(x => x.EmailId == EmailId);
                var userRoles = (context.tblRoles.Where(y=> y.Id== user.RoleId).Select(z=>z.RoleName)).ToArray();

                return userRoles;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
