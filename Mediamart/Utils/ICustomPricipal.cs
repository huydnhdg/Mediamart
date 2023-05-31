using Mediamart.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Mediamart.Utils
{
    interface ICustomPricipal : IPrincipal
    {
        string Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
    public class CustomPrincipal : ICustomPricipal
    {
        public IIdentity Identity { get; private set; }
        private MediaMEntities context = new MediaMEntities();

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }
        public bool IsInRole(string role)
        {
            //check xem quyền này có ở tài khoản này không
            var roleManager = context.AspNetUsers.FirstOrDefault(a => a.UserName == Identity.Name);
            return roleManager.AspNetRoles.All(r => r.Name == role || r.Id == role);
        }
        public bool IsInModule(string module)
        {
            //check xem module này có trong quyền này không
            
            var roleManager = context.AspNetUsers.FirstOrDefault(a => a.UserName == Identity.Name);
            if(roleManager.AspNetRoles.FirstOrDefault(r=>r.Id == Mediamart.Utils.Common.Roles.ADMIN) != null)
            {
                if (module.Contains(".user"))
                {
                    return false;
                }
                return true;
            }
            foreach (var item in roleManager.AspNetRoles.ToList())
            {
                var list_module = context.AspModuleRoles;
                var result = list_module.FirstOrDefault(r => r.RoleId == item.Id && r.ModuleId == module);
                if (result != null)
                {
                    return true;
                }
            }
            return false;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class CustomPrincipalSerializeModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsInModule { get; set; }
    }
}
