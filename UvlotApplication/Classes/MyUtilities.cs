using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;

namespace UvlotApplication.Classes
{
    public class MyUtilities
    {

       
        public static string DoPosts(string json, string url, string seckey, string callbackurl,string hash)
        {
            string resp;
           
            WebLog.Log("Json" + json);
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(seckey))
                    {
                        client.Headers.Add("Authorization", seckey);
                        client.Headers.Add("Hash", hash);

                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                }
            }
            catch (WebException wex)
            {
                //WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        public static string DoPost(string json, string url, [Optional]string token)
        {

            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = token;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                    WebLog.Log("Post from utility" + url + json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                        WebLog.Log("response from utility" + resp);
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }



        public static string DoRemitaPost(string url, string json)
        {

            string resp;
            try
            {
                string partnerID = ConfigurationManager.AppSettings["PartnerID"];
                string partnerkey = ConfigurationManager.AppSettings["PartnerKey"];
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(partnerkey))
                    {
                        client.Headers.Add("partnerID", partnerID);
                        client.Headers.Add("partnerKey", partnerkey);
                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                    WebLog.Log("Post from utility" + url + json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                        WebLog.Log("response from utility" + resp);
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }

        public static string DoPostToken(string json, string url, [Optional]string token)
        {

            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = token;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                    WebLog.Log("Post from utility" + url + json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                        WebLog.Log("response from utility" + resp);
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }
        public static string DoGet(string url, string seckey)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                  
                    if (!string.IsNullOrWhiteSpace(seckey))
                    {
                        client.Headers.Add("Authorization", seckey);
                        
                   }

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    resp = client.DownloadString(url);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }



    }
}