using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mediamart.Controllers
{
    [RoutePrefix("tra-cuu-bao-hanh")]
    public class SearchWarrantiController : Controller
    {
        [Route]
        // GET: SearchWarranti
        public ActionResult Index()
        {
            return View();
        }
    }
}