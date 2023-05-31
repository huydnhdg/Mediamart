using Mediamart.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Utils
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        MediaMEntities context = new MediaMEntities();
        public string Role { get; set; }
        public string Module { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string User = httpContext.User.Identity.Name;
            bool authorize = false;
            var listRole = new List<AspNetRole>();
            //admin thì auto qua
            if (!string.IsNullOrEmpty(User))
            {
                var query = context.AspNetUsers.Where(a => a.UserName == User).FirstOrDefault();
                listRole = query.AspNetRoles.ToList();
                if (query.AspNetRoles.FirstOrDefault(a => a.Id == Mediamart.Utils.Common.Roles.ADMIN) != null)
                {
                    return true;
                }
            }

            if (!string.IsNullOrEmpty(Module) && listRole.Count() > 0)
            {
                foreach (var item in listRole)
                {
                    //chặn theo module
                    var module = context.AspModuleRoles.FirstOrDefault(a => a.RoleId == item.Id && a.ModuleId == Module);
                    if (module != null)
                    {
                        return true;
                    }
                }
            }
            //đăng nhập là qua
            if (!string.IsNullOrEmpty(User))
            {
                return true;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}