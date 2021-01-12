using DataAccessLayerT;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UvlotApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult HomePage()
        {
            //var datetime = "12/2/2019";
            //DateTime dates = DateTime.ParseExact(datetime, "dd/MM/yyyy", null);

            /* var datetime = "30/12/2019";
              datetime = "30-04-2019";
             DateTime date = DateTime.ParseExact(datetime, "dd/MM/yyyy", null);
             */
            var ids = DateTime.Now.Year.ToString().Substring(2);
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult termcondition()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}