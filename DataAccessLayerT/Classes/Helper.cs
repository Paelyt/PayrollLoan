using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Utilities;
using static Utilities.CryptographyManager;

namespace DataAccessLayerT
{
    public class Helper
    {
        static string prepaidStr = "OFFLINE_PREPAID";


        public static string DoPost(string url, [Optional]string Authorization, [Optional]string MERCHANT_ID, [Optional]string API_KEY, [Optional]string REQUEST_ID, [Optional]string REQUEST_TS, [Optional]string API_DETAILS_HASH, string json)
        {
            string resp;
            try
            {
                string partnerID = ConfigurationManager.AppSettings["PartnerID"];
                string partnerkey = ConfigurationManager.AppSettings["PartnerKey"];



                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";

                    if (!string.IsNullOrWhiteSpace(partnerID))
                    {

                        client.Headers.Add("partnerID", partnerID);
                        client.Headers.Add("partnerKey", partnerkey);
                      
                    }
                    /*  if (!string.IsNullOrWhiteSpace(MERCHANT_ID))
                    {

                         client.Headers.Add("partnerID", partnerID);
                        client.Headers.Add("partnerKey", partnerkey);
                      For Test
                         client.Headers.Set("MERCHANT_ID", MERCHANT_ID);
                         client.Headers.Set("API_KEY", API_KEY);
                         client.Headers.Set("REQUEST_ID", REQUEST_ID);
                         client.Headers.Set("REQUEST_TS", REQUEST_TS);
                         client.Headers.Set("API_DETAILS_HASH", API_DETAILS_HASH);
                        
                } */
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
                // WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }

        private static readonly Random Random = new Random();
        public static string RandomString(int length, Mode mode = Mode.AlphaNumeric)
        {
            var characters = new List<char>();

            if (mode == Mode.Numeric || mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaNumericLower)
                for (var c = '0'; c <= '9'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaUpper)
                for (var c = 'A'; c <= 'Z'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericLower || mode == Mode.AlphaLower)
                for (var c = 'a'; c <= 'z'; c++)
                    characters.Add(c);

            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[Random.Next(s.Count)]).ToArray());
        }
        public enum Mode
        {
            AlphaNumeric = 1,
            AlphaNumericUpper = 2,
            AlphaNumericLower = 3,
            AlphaUpper = 4,
            AlphaLower = 5,
            Numeric = 6
        }
       
        private static void WaitTime(double seconds)
        {
            DateTime dt = DateTime.Now + TimeSpan.FromSeconds(seconds);

            do { } while (DateTime.Now < dt);
        }
        public static string ConvertToCurrency(string amount)
        {
            return string.Format("{0:N2}", Convert.ToDecimal(amount));
        }
        public static string ConvertToNairaCurrency(string amount)
        {
            return string.Format("₦{0:N2}", Convert.ToDecimal(amount));
        }
        public static string ConvertToNairaCurrencyNoZero(string amount)
        {
            string newamount = string.Format("₦{0:N2}", Convert.ToDecimal(amount));
            return newamount.Remove(newamount.Length - 3, 3);

        }
        public static string ConvertTonNairaCurrencyNoZero(string amount)
        {
            string newamount = string.Format("N{0:N2}", Convert.ToDecimal(amount));
            return newamount.Remove(newamount.Length - 3, 3);

        }
        public static string ConvertTourrencyNoZero(string amount)
        {
            string newamount = string.Format("{0:N2}", Convert.ToDecimal(amount));
            return newamount.Remove(newamount.Length - 3, 3);

        }

        public static bool isNumeric(string date)

        {

            try

            {

                int dt = int.Parse(date);

                return true;

            }
            catch

            {

                return false;

            }

        }




        public static BVNC BVNValidationResps(string bvnNumber)
        {

            BVNC BC = new BVNC();
            //errormessage = "Enquiry failed";
            try
            {
                if (bvnNumber.Length < 11)
                {
                    BC.errormessage = "Please enter valid BVN number";
                    BC.respCode = "0001";
                    BC.respDescription = BC.errormessage;
                    return BC;
                }

                var Url = ConfigurationManager.AppSettings["ValidateBVN"];
                var secKey = ConfigurationManager.AppSettings["secKey"];

                Url = Url.Replace("{$BVNnumber}", bvnNumber.ToString()).Trim().Replace("{$secKey}", secKey.ToString()).Trim();
                var resp = DoGet(Url);


                dynamic resps = JObject.Parse(resp);
                if (resps.status != "success")
                {
                    BC.respCode = "009";
                    BC.respDescription = resps.message;
                    BC.errormessage = "Please enter valid BVN number";
                }
                else
                {
                    BC.respCode = "00";
                    BC.respDescription = "Request Successful";
                    BC.FirstNAme = resps.data.first_name;
                    BC.MiddleName = resps.data.middle_name;
                    BC.LastName = resps.data.last_name;
                    BC.Dateofbirth = resps.data.date_of_birth;
                    BC.Phone = resps.data.phone_number;
                    BC.RegistrationDate = resps.data.registration_date;
                    BC.EnrollmentBank = resps.data.enrollment_bank == null ? "" : resps.data.enrollment_bank;
                    BC.EnrollmentBank = DataManager.DataReader.GetBankNameByCode(BC.EnrollmentBank);
                    BC.EnrollmentBranch = resps.data.enrollment_branch;
                    //BC.image_base_64 = resps.data.image_base_64;
                    BC.address = resps.data.address;
                    BC.gender = resps.data.gender;
                    BC.email = resps.data.email;
                    BC.watch_listed = resps.data.watch_listed;
                    BC.Nationality = resps.data.nationality;
                    BC.marital_status = resps.data.marital_status;
                    BC.state_of_residence = resps.data.state_of_residence;
                    BC.lga_of_residence = resps.data.lga_of_residence;
                    BC.errormessage = "";
                }
            }
            catch (Exception ex)
            {
                BC.respCode = "0091";
                BC.respDescription = "Error validating BVN";
                BC.errormessage = BC.respDescription;
                WebLog.Log(ex.Message);
            }


            return BC;
        }
        public static string DoGet(string Url)
        {

            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    resp = client.DownloadString(Url);
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

        public partial class BVNC
        {

            public string respCode { get; set; }

            public string respDescription { get; set; }
            public string BVN { get; set; }

            public string Phone { get; set; }

            public string FirstNAme { get; set; }

            public string LastName { get; set; }
            public string MiddleName { get; set; }

            public string Dateofbirth { get; set; }
            public string RegistrationDate { get; set; }

            public string EnrollmentBank { get; set; }

            public string EnrollmentBranch { get; set; }


            public string gender { get; set; }

            public string Nationality { get; set; }


            public string image_base_64 { get; set; }

            public string address { get; set; }

            public string email { get; set; }

            public string watch_listed { get; set; }

            public string marital_status { get; set; }

            public string state_of_residence { get; set; }

            public string lga_of_residence { get; set; }

            public string errormessage { get; set; }

        }

        public class Studentrec
        {
            public string respCode { get; set; }
            public string respDescription { get; set; }
            public string Lastname { get; set; }
            public string Firstname { get; set; }
            public string Othername { get; set; }
            public string Faculty { get; set; }
            public string Department { get; set; }
            public string CreditLimit { get; set; }
            public string NumberOfBorrows { get; set; }
            public string OutstandingBalance { get; set; }
        }
    }
}
