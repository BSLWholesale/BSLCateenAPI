using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSLCanteenAPI.Controllers
{
    public class CanteenController : Controller
    {
        // GET: Canteen
        public ActionResult Index()
        {
            return View();
        }
    }
}