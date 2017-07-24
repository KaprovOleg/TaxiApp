using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data.Entity;
using TaxiApp.Models;

namespace TaxiApp.Providers
{
    public class AppRoleProvider : RoleProvider
    {
        public override string ApplicationName { get; set; }

        //
        public override string[] GetRolesForUser(string username)
        {
            string[] roles = new string[] { };
            using (AppContext db1 = new AppContext())
            {
                AppUser user = db1.AppUsers.FirstOrDefault(u => u.UserName == username);
                if (user!=null)
                {
                    AppUserRole role = db1.AppUserRoles.Find(user.AppUserRoleID);
                    if (role != null) roles = new string[] { role.Name };
                }
            }
            return roles;
        }
        //
        public override string[] GetUsersInRole(string roleName)
        {
            string[] roles = new string[] { };
            return roles;
        }
        //
        public override bool IsUserInRole(string username, string roleName)
        {
            bool result = false;
            using (AppContext db1 = new AppContext())
            {
                AppUser user = db1.AppUsers.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    AppUserRole role = db1.AppUserRoles.Find(user.AppUserRoleID);
                    if (role != null)
                    {
                        if (role.Name == roleName) result = true;
                    }
                }
            }
            return result;
        }
        //
        //

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