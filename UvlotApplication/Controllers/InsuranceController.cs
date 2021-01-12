using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace UvlotApplication.Controllers
{
    public class InsuranceController : ApiController
    {

        public class UserModel
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string SurName { get; set; }

            public static List<UserModel> getUsers()
            {
                List<UserModel> users = new List<UserModel>()
{
                new UserModel (){ ID=1, Name="Anubhav", SurName="Chaudhary" },
                new UserModel (){ ID=2, Name="Mohit", SurName="Singh" },
                new UserModel (){ ID=3, Name="Sonu", SurName="Garg" },
                new UserModel (){ ID=4, Name="Shalini", SurName="Goel" },
                new UserModel (){ ID=5, Name="James", SurName="Bond" },
};
                return users;
            }
        }
        public HttpResponseMessage Json(object value)
        {
            try
            {

                var resp = JsonSerialize(value);
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(resp, Encoding.UTF8, "application/json")
                };
                return response;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return null;
            }
        }
        [HttpPost]
        [Route("Controllers/Insurance/UniversalInsurancetest")]
        public HttpResponseMessage UniversalInsurancetest([FromBody] JObject objs)
        {
            // This is to get list of Insurance company in nigeria
            // var InsuranceList = _dr.getList();

            // I need to get the list of Plans on Life insurance
            // i need to check current Customer Plan
            // Uprgrade if exist from the api
            // Create New if non existence
            // Then Move it to the next Phase  which is 
            // need to work with univeral on this .
           
            //var name = json?.remitaTransRef;
            var obj =
                new JObject(
                           new JProperty("status", "Test"));
                           
            return Json(objs);
        }
        public static string JsonSerialize(object obj)
        {
            try
            {
                var stringifiedObj = JsonConvert.SerializeObject(obj, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                       
                    });
                return stringifiedObj;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return null;
            }
        }

    }
}
