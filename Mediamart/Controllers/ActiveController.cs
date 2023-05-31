using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mediamart.Models;

namespace Mediamart.Controllers
{
    [RoutePrefix("kich-hoat")]
    public class ActiveController : Controller
    {
        MediaMEntities db = new MediaMEntities();
        [Route]
        public ActionResult Index(string serial)
        {
            var model = new Product();
            if (!string.IsNullOrEmpty(serial))
            {
                var product = db.Products.SingleOrDefault(a => a.Code == serial);
                if (product != null)
                {
                    model = product;
                }
            }

            return View(model);
        }
    }
}