using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace UvlotApplication.Controllers
{
    public class WebApiController : ApiController
    {
        // GET: WebApi
       /* public ActionResult Index()
        {
            return View();
        }
        */

        public IHttpActionResult DirectDebitCallback([FromBody] JObject objs)
        {
            try
            {
                dynamic respObj = new JObject();
                respObj = objs;
                if (respObj?.status == "SUCCESSFUL")
                {
                    return Json(respObj);
                }
                else
                {
                    return Json(respObj);
                }
               // dynamic data = JObject.Parse(objs.ToString());
               
              //  return Json(respObj);

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}