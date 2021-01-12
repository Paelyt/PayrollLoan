using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UvlotApplication.HelperClasses;

namespace UvlotApplication.Classes
{
    public class Utility
    {
        public bool ValidateNum(string Number, string Number1 = "")
        {
            try
            {
                Regex nonNumericRegex = new Regex(@"\D");
                if (nonNumericRegex.IsMatch(Number.ToString()) || nonNumericRegex.IsMatch(Number1.ToString()))
                {
                    //Contains non numeric characters.
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }




        public static bool ValidatePhoneNumber(string phoneNumber, out string validPhoneNumber)
        {
            bool _isValid = false; validPhoneNumber = ""; string myPhoneNumber = phoneNumber.Replace("+", ""); try
            {
                if (myPhoneNumber.Length < 11) { return false; }
                // myPhoneNumber.Length > 11               
                if (myPhoneNumber.Substring(0, 3) == "234"
                    && myPhoneNumber.Length < 13)
                {
                    return false;
                }
                if (myPhoneNumber.Substring(0, 3) != "234" && myPhoneNumber.Length > 11)
                {
                    return false;
                }
                if (myPhoneNumber.Substring(0, 3) == "234" && myPhoneNumber.Length > 13)
                {
                    return false;
                }
                if (myPhoneNumber.Length == 11)
                {
                    validPhoneNumber = "234" + myPhoneNumber.Substring(1, 10);
                }
                if (myPhoneNumber.Length == 13)
                {
                    validPhoneNumber = myPhoneNumber;
                }
                _isValid = true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                _isValid = false;
            }
            return _isValid;
        }

        public string ConvertToCurrency(string amount)
        {
            return string.Format("{0:N2}", Convert.ToDecimal(amount));
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i].Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }


            return dt;
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

        public static DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
        {
            OleDbConnection oledbConn = new OleDbConnection(connString);
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {

                oledbConn.Open();
                using (DataTable Sheets = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null))
                {

                    for (int i = 0; i < Sheets.Rows.Count; i++)
                    {
                        string worksheets = Sheets.Rows[i]["TABLE_NAME"].ToString();
                        OleDbCommand cmd = new OleDbCommand(String.Format("SELECT * FROM [{0}]", worksheets), oledbConn);
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        oleda.SelectCommand = cmd;

                        oleda.Fill(ds);
                    }

                    dt = ds.Tables[0];
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            finally
            {

                oledbConn.Close();
            }

            return dt;

        }


        public string PayrollLoanCalc(double PA, int LT, double INTR)

        {
            // Fomular to get Repayment amount for each month using the Loan Amount, Loan tenure and the Interest .
            try
            {

                // PA= Principal Amount/Loan Amount.
                // LT= Loan Tenure.
                // INTR= Interest Rate in percentage.
                // Multilpy the principal amount with the loan tenure, 
                // then multiply your result with the interest rate which is in percentage 
                // then add the pricipal amount to the newly derived answer 
                // and finally divide the over all result with the Loan tenure.

                string monthlyRepayment = (((PA + (PA * LT) * (INTR * 0.01))) / LT).ToString();



                return monthlyRepayment;

            }
            catch

            {

                return "0";

            }

        }





      /*  public void SendEmail(dynamic lvm)
        {
            try
            {
                var tAlert = new TransactionAlert
                {
                    PhoneNumber = lvm.PhoneNumber,
                    Sender = "Uvlot",
                    Message = $"Dear {lvm.Firstname.ToUpper()} {lvm.Surname.ToUpper()}, {ConfigurationManager.AppSettings["SmsWelcomeMessage"]}"
                };

                // var smsResponse = NotificationService.SendSms(tAlert);

                var bodyTxt = System.IO.File.ReadAllText(Server.MapPath("~/EmailNotifications/WelcomeEmailNotification.html"));
                bodyTxt = bodyTxt.Replace("$MerchantName", $"{lvm.Firstname} {lvm.Surname}");
                var msgHeader = $"Welcome to Uvlot";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }*/

    }

    public class TransactionAlert
    {
        public virtual string BaseUrl { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Sender { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Message { get; set; }
    }


    public static class Utilities
    {
        public static string Before(this string @this, string a)
        {
            try
            {
                var posA = @this.IndexOf(a, StringComparison.Ordinal);
                return posA == -1 ? "" : @this.Substring(0, posA);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static string After(this string @this, string a)
        {
            try
            {
                var posA = @this.LastIndexOf(a, StringComparison.Ordinal);
                if (posA == -1)
                {
                    return "";
                }
                var adjustedPosA = posA + a.Length;
                return adjustedPosA >= @this.Length ? "" : @this.Substring(adjustedPosA);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


    
}
}