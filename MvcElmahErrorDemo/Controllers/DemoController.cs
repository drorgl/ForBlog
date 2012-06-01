using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcElmahErrorDemo.Controllers
{
    public class DemoController : Controller
    {
        //
        // GET: /Demo/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ThrowError()
        {
            throw new Exception("Test exception");

            return View();
        }

        public ActionResult ShowError(string id)
        {
            ViewBag.id = id;
            return View();
        }
    }
}
