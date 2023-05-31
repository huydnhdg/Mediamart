using Mediamart.Models;
using Mediamart.Utils;
using System.Web.Mvc;


namespace Mediamart.Areas.Admin.Controllers
{
    [Authorize]
    [Permission]
    public abstract partial class BaseController : Controller
    {
        public MediaMEntities db = new MediaMEntities();
        public BaseController()
        {

        }
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;

            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
                TempData["Alert"] = "Success!";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
                TempData["Alert"] = "Warning!";
            }
            else if (type == "danger")
            {
                TempData["AlertType"] = "alert-danger";
                TempData["Alert"] = "Danger!";
            }
            else if (type == "info")
            {
                TempData["AlertType"] = "alert-info";
                TempData["Alert"] = "Info!";
            }
            else if (type == "primary")
            {
                TempData["AlertType"] = "alert-primary";
                TempData["Alert"] = "Primary!";
            }
            else
            {
                TempData["AlertType"] = "alert-default";
                TempData["Alert"] = "Nothing!";
            }

        }
    }
}