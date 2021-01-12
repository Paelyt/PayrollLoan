using DataAccessLayerT.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UvlotApplication.Classes;
using DataAccessLayerT;
using UvlotApplication.HelperClasses;
using System.Text;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.Globalization;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using IronPdf;
using System.Net;
using System.Threading;
using ClosedXML.Excel;
using System.Web.Script.Serialization;

namespace UvlotApplication.Controllers
{
    public class AdminController : Controller
    {
        UserController _usr = new UserController();
        DataWriter _DM = new DataWriter();
        DataReader _DR = new DataReader();
        PageAuthentication _pa = new PageAuthentication();
        DataReader dp = new DataReader();
        UvlotEntities uvdb = new UvlotEntities();
        DataAccessLayerT.DataManager.Page pages = new DataAccessLayerT.DataManager.Page();
        string user = "";
        string id = "";
        List<string> rol = new List<string>();
        User users = new User();
        LoanApplication LoanAp = new LoanApplication();
        LoanApproval LoanApproval = new LoanApproval();
        LoansController lc = new LoansController();
        Utility utilities = new Utility();
        double repayment = 0;
        string respMessage = "";
        int AppStatFk = 0;
        int userid = 0;
        string InstCode = "";
        private static HttpWebRequest _req;
        private static HttpWebResponse _response;
        List<Menus> MenuList = new List<Menus>();
        string LoggedinInst = "";
        // GET: Admin

        public ActionResult TestRecommed()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {

                    GetMenus();
                }
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {

            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {

                    
                    //TempData["RepaymentEfficiency"] = _DR.RepaymentEfficiency();

                    double DefaultLoans = _DR.RepaymentEfficiency();
                    DefaultLoans = DefaultLoans / 100;
                    DefaultLoans = Math.Round((float)DefaultLoans, 4);
                    TempData["NODefaultLoans"] = DefaultLoans;
                    TempData["RepaymentEfficiency"] = DefaultLoans;


                    // TempData["TotalAcceptanceRate"] = _DR.GetTotalAcceptanceRate();

                    var AppDisbursed = _DR.getCountOFLoans();
                    var AppSubmitted = _DR.getCountOFLoansNotDisbursed();
                    TempData["AppDisbursed"] = AppDisbursed;
                    TempData["AppSubmitted"] = AppSubmitted;
                    var Total = Math.Round(AppDisbursed / AppSubmitted, 4);
                    TempData["TotalAcceptanceRate"] = Total;

                    // TempData["GetAbandonrate"] = _DR.GetAbandonrate();

                    var AbandonApplication = _DR.GetAbandonrate();
                    TempData["GetAbandonApp"] = AbandonApplication;
                    var TotalApplication = _DR.getCountOFLoansNotDisbursed();
                    TempData["TotalApplication"] = TotalApplication;
                    var  AbandonApplications = Math.Round(AbandonApplication/TotalApplication,4);
                    AbandonApplications = AbandonApplications * 100;
                    TempData["GetAbandonrate"] = AbandonApplications;
                    // return (float)AbandonApplications;
                    string refNum = "15752941567981";

                    var c = _DR.KpiLoanAppToRemitaGeneration(refNum);

                    GetData();
                    GetLoanOwnedAndCount();
                    GetDataAmount();
                    GetMenus();

                    if (User.EmploymentStatus_FK == 1)
                    {
                        return RedirectToAction("MyApplicationsStatus", "Admin");
                    }
                    //else
                    //{
                    //    return RedirectToAction("index", "Admin");
                    //}
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

            return View();
        }

        public float TotalAcceptanceRate()
        {
            try
            {
                var Value = _DR.GetTotalAcceptanceRate();
                return 0;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }



        public ActionResult GetLoanOwnedAndCount()
         {
            try
            {

                int InstFK = 3;
                int Appstat = 0;
                var records = _DR.GetLoanOwnedAndCount();
               
                var m = _DR.GetLoanOwnedAndCount().OrderByDescending(x=>x.Month).ToList();
                List<GetLoanOwnedAndCount> GLA = new List<GetLoanOwnedAndCount>();
                GLA = m;

                foreach (var n in GLA)
                {
                    if(n.Month == 1)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "January";
                    }
                    if (n.Month == 2)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "Feburary";
                    }
                    if (n.Month == 3)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "March";
                    }
                    if (n.Month == 4)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "April";
                    }
                    if (n.Month == 5)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "May";
                    }
                    if (n.Month == 6)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "June";
                    }
                    if (n.Month == 7)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "July";
                    }
                    if (n.Month == 8)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "August";
                    }
                    if (n.Month == 9)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "September";
                    }
                    if (n.Month == 10)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "October";

                    }
                    if (n.Month == 11)
                        {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "November";
                        }
                    if (n.Month == 12)
                    {
                        var Amt = Math.Round(n.TotalAmount, 0);
                        n.TotalAmounts = MyUtility.ConvertToCurrency(Amt.ToString());
                        n.TotalAmount = Amt;
                        n.Months = "December";
                    }
                }
                return Json(GLA, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult GetData()
        {
            try
            {

                int InstFK = 3;
                int Appstat = 0;
                var records = _DR.GetChartReportCount(InstFK, Appstat);
                record rec = new record();
                //  rec.RecTotalAmount = 
                rec.Recname = Convert.ToString(records[3]=="2"? "Recommend": "Recommend");
                rec.RecCount = Convert.ToInt16(records[2]);
                
                rec.Approvename = Convert.ToString(records[0] == "1" ? "Approve" : "Approve");
                rec.ApproveCount = Convert.ToInt16(records[1]);


                rec.Pendingname = Convert.ToString(records[5] == "3" ? "Pending" : "Pending");
                rec.PendingCount = Convert.ToInt16(records[4]);

                rec.Disbursename = Convert.ToString(records[7] == "6" ? "Disburse" : "Disburse");
                rec.DisursedCount = Convert.ToInt16(records[6]);

                return Json(rec, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult GetDataAmount()
        {
            try
            {

                int InstFK = 3;
                int Appstat = 0;
                var records = _DR.GetChartReportAmount(InstFK, Appstat);
                record rec = new record();
               
             /*   rec.Recname = Convert.ToString(records[3] == "2" ? "Recommend" : "Recommend");
                rec.RecCount = Convert.ToInt16(records[2]);
                rec.RecTotalAmount = Convert.ToDouble(records[2]);
*/
                rec.Approvename = Convert.ToString(records[1] == "1" ? "Approve" : "Approve");
                rec.ApproveCount = Convert.ToInt16(records[0]);
                rec.ApproveTotalAmount = Convert.ToDouble(records[2]);
                
             /*   rec.Pendingname = Convert.ToString(records[5] == "3" ? "Pending" : "Pending");
                rec.PendingCount = Convert.ToInt16(records[4]);
                rec.PendingTotalAmount = Convert.ToDouble(records[2]); ;
*/
                rec.Disbursename = Convert.ToString(records[4] == "6" ? "Disburse" : "Disburse");
                rec.DisursedCount = Convert.ToInt16(records[3]);
                rec.DisbursedTotalAmount = Convert.ToDouble(records[5]);
                return Json(rec, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult PayrollDisbursedLoans()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {

                    ViewBag.Data = _DR.PayrollDisbursedLoans();
                    Session["AllTransaction"] = ViewBag.Data;
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollDisbursedLoans(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {

                    ViewBag.Data = _DR.PayrollDisbursedLoans();
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        [HttpGet]
        public ActionResult PayrollBorroweredLoans()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.Data = _DR.PayrollBorroweredLoans();
                    Session["AllTransaction"] = ViewBag.Data;
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollBorroweredLoans(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    
                    ViewBag.Data = _DR.PayrollBorroweredLoans();
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        
        [HttpGet]
        public ActionResult PayrollRevenueEarned()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollRevenueEarned(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {



                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));

                    ViewBag.Data = _DR.PayrollRevenueEarned(StartDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        
              [HttpGet]
        public ActionResult PayrollRevenueReceived()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollRevenueReceived(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {



                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));

                    ViewBag.Data = _DR.PayrollRevenueReceived(StartDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpGet]
        public ActionResult PayrollOutstandingLoan()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollOutstandingLoan(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {



                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));

                    ViewBag.Data = _DR.PayrollOutstandingLoan(StartDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }



        [HttpGet]
        public ActionResult PayrollLoanDueForDebit()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollLoanDueForDebit(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {


                   
                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));
                  
                    ViewBag.Data = _DR.PayrollLoanDueForDebit(StartDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpGet]
        public ActionResult PayrollApplicationRelated()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
               
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        
        [HttpPost]
        public ActionResult PayrollApplicationRelated(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                   
               
                    var EDate = Convert.ToString(form["EDate"]); //(DateTime)LoanAP.DateModified;
                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));
                    DateTime EndDate = DateTime.ParseExact(EDate, "MM/dd/yyyy", new CultureInfo("en-US"));

                    ViewBag.Data = _DR.PayrollApplicationRelated(StartDate, EndDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }



        [HttpGet]
        public ActionResult PayrollRepayment()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollRepayment(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {


                    var EDate = Convert.ToString(form["EDate"]); //(DateTime)LoanAP.DateModified;
                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));
                    DateTime EndDate = DateTime.ParseExact(EDate, "MM/dd/yyyy", new CultureInfo("en-US"));

                    ViewBag.Data = _DR.PayrollRepayment(StartDate, EndDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }



        [HttpGet]
        public ActionResult PayrollregistrationRelated()
        {
            try
               {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollregistrationRelated(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                     var EDate = Convert.ToString(form["EDate"]); //(DateTime)LoanAP.DateModified;
                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));
                    DateTime EndDate = DateTime.ParseExact(EDate, "MM/dd/yyyy", new CultureInfo("en-US"));

                    ViewBag.Data = _DR.PayrollregistrationRelated(StartDate, EndDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }



        [HttpGet]
        public ActionResult PayrollUserRelated()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult PayrollUserRelated(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    var EDate = Convert.ToString(form["EDate"]); //(DateTime)LoanAP.DateModified;
                    var SDate = Convert.ToString(form["SDate"]); //(DateTime)LoanAP.DateModified;

                    DateTime StartDate = DateTime.ParseExact(SDate, "MM/dd/yyyy", new CultureInfo("en-US"));
                    DateTime EndDate = DateTime.ParseExact(EDate, "MM/dd/yyyy", new CultureInfo("en-US"));

                    ViewBag.Data = _DR.PayrollUserRelated(StartDate, EndDate);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        [HttpGet]
        public ActionResult invoices(string Refid, int instid, double TA)
        {
            try
            {
                GetMenus();
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                Institution Inst = new Institution();
                Inst = _DR.getInstitutionById(instid);
                TempData["TotalAmount"] = Math.Round(TA, 3);
                TempData["VAT"] = 0;
                var TodaysDate = MyUtility.getCurrentLocalDateTime();
                TempData["Date"] = TodaysDate;
                TempData["DateDue"] = TodaysDate.AddDays(10);
                return View(Inst);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Export(string GridHtml)
        {

            var uri = new Uri("http://www.google.com/ncr");
            // turn page into pdf
            var htmlToPdf = new HtmlToPdf();
            var pdf = htmlToPdf.RenderUrlAsPdf(uri);
            // save resulting pdf into file
            pdf.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), "UrlToPdf.Pdf"));
            return View();
            //using (MemoryStream stream = new System.IO.MemoryStream())
            //{


            //    var cssText = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/css/test.css"));
            //    var html = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/css/test.html"));
            //    StringReader sr = new StringReader(GridHtml);
            //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            //    pdfDoc.Open();
            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            //    pdfDoc.Close();
            //    return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            //}


            // Number 2 Action 
            ////byte[] pdf; // result will be here

            //var cssText = System.IO.File.ReadAllText(HostingEnvironment.MapPath("/css/test.css"));
            //var html = System.IO.File.ReadAllText(HostingEnvironment.MapPath("/Admin/Invoices.cshtml"));

            //using (var memoryStream = new MemoryStream())
            //{
            //    var document = new Document(PageSize.A4, 50, 50, 60, 60);
            //    var writer = PdfWriter.GetInstance(document, memoryStream);
            //    document.Open();

            //    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
            //    {
            //        using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
            //        {
            //            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream);
            //        }

            //    }

            //    document.Close();


            //    return File(memoryStream.ToArray(), "application/pdf", "Grid.pdf");
            //}


        }

        [HttpPost]
        public ActionResult DisburseList(string ItemList, FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                userid = User.ID;

                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                form["DisburseLoanComment"] = "Disbursed";
                form["Accept"] = "6";
                LoanApproval.CommentBy = userid;
                LoansController Ln = new LoansController();
                LoanLedger LoanLedger = new LoanLedger();
                AppStatFk = 1;
                TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                var LoanAps = new AppLoan();
                AppLoan Apploan = new AppLoan();
                //var LoanAps = _DR.GetLoanApplication(ItemList, AppStatFk);
                //Apploan = LoanAps;

                LoanApplication LoanAp = new LoanApplication();
                // LoanAp.LoanRefNumber = LoanAps.LoanRefNumber;
                LoanAp.ApplicationStatus_FK = Convert.ToInt16(form["Accept"]);
                string[] arr = ItemList.Split(',');
                double LoanAmount = 0;
                double salaryAmount = 0;
                bool LoanLegder = false;
                List<LoansLedger> LedgerRec = new List<LoansLedger>();
                //  if (respMessage == "0")
                //{
                if (arr.Length == 1)
                {
                    ItemList = ItemList.Before("+");
                    LoanAps = _DR.GetLoanApplication(ItemList, AppStatFk);
                    Apploan = LoanAps;

                    LoanAp.LoanRefNumber = LoanAps.LoanRefNumber;
                    LoanAp.ApplicationStatus_FK = Convert.ToInt16(form["Accept"]);
                    LoanAmount = Convert.ToDouble(LoanAps.LoanAmount);
                    salaryAmount = Convert.ToDouble(LoanAps.Salary);
                    InstCode = LoanAps.InstitutionCode;
                    LoanLegder = Ln.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanAps.LoanTenure, out repayment, out respMessage, InstCode);
                    if (respMessage == "0")
                    {
                        var resp = DisburseCash(LoanAps);
                        if (resp != null)
                        {
                            dynamic results = JObject.Parse(resp);
                            if (results?.status == "true")
                            {
                                LedgerRec = _DR.getLedgerRecord(LoanAps.LoanRefNumber);
                                var result = FinallDisbursement(LedgerRec, LoanLedger, LoanAps, LoanAp);
                                if (result != 0)
                                {
                                    //I am Here 
                                    LoanAp = UpdateLoanApproval(LoanAps, form, out LoanApproval);
                                    LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                                    // return RedirectToAction("DisburseLoan");
                                    // return RedirectToAction();   
                                    return Json(new { response = "true", Data = results?.message });

                                }
                            }
                            else
                            {
                                TempData["Error"] = results?.message;
                                return Json(new { response = "false", Data = results?.message });
                            }
                        }

                        return Json(new { response = "false", Data = "Please Try Again" });
                    }
                }
                //  }
                else if (arr.Length > 1)
                {
                    for (var m = 0; m < arr.Length; m++)
                    {
                        arr[m] = arr[m].Before("+");
                        LoanAps = _DR.GetLoanApplication(arr[m], AppStatFk);
                        LoanAp.LoanRefNumber = LoanAps.LoanRefNumber;
                        LoanAp.ApplicationStatus_FK = Convert.ToInt16(form["Accept"]);
                        LoanAmount = Convert.ToDouble(LoanAps.LoanAmount);
                        salaryAmount = Convert.ToDouble(LoanAps.Salary);
                        InstCode = LoanAps.InstitutionCode;
                        LoanLegder = Ln.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanAps.LoanTenure, out repayment, out respMessage, InstCode);
                        if (respMessage == "0")
                        {
                            var resp = DisburseCash(LoanAps);
                            if (resp != null)
                            {
                                dynamic results = JObject.Parse(resp);
                                if(results?.status == "true")
                                { 
                                LedgerRec = _DR.getLedgerRecord(LoanAps.LoanRefNumber);
                                var result = FinallDisbursement(LedgerRec, LoanLedger, LoanAps, LoanAp);
                                if (result != 0)
                                {
                                    LoanAp = UpdateLoanApproval(LoanAps, form, out LoanApproval);
                                    LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                                    return Json(new
                                    {
                                        response = "true",
                                        //Data = "Loan Disbursed"
                                        Data = results?.message
                                    });
                                }
                            }
                           else
                            {
                                TempData["Error"] = results?.message;
                                return Json(new { response = "false", Data = results?.message });
                            }
                            }
                        }
                    }
                    return RedirectToAction("Disburse");
                    
                }

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public ActionResult CompanyProfile()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            return View();
        }
        public ActionResult Acknowledgement(string Refid)
        {
            if (Refid == null || Refid == "")
            {
                return RedirectToAction("/");
            }
            var LoanApps = _DR.LoanDetails(Refid);

            if (LoanApps == null)
            {
                return RedirectToAction("/");
            }
            LoanApps.ConvertedAmount = utilities.ConvertToCurrency(LoanApps.LoanAmount.ToString());
            GetMenus();
            return View(LoanApps);
        }
        [HttpPost]
        public ActionResult CreateInstitution(Institution instM)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                ViewBag.listInstitution = _DR.getAllInstitutionType();
                Institution inst = new Institution
                {
                    ContactEmailAddress = instM.ContactEmailAddress,
                    ContactPhoneNo = instM.ContactPhoneNo,
                    DateCreated = MyUtility.getCurrentLocalDateTime(),
                    HeadOfInstition = instM.HeadOfInstition,
                    InstitutionAddress = instM.InstitutionAddress,
                    InstitutionEmailAddress = instM.InstitutionEmailAddress,
                    InstitutionPhoneNo = instM.InstitutionPhoneNo,
                    InstitutionCode = instM.InstitutionCode,
                    IsVisible = 1,
                    Name = instM.Name,
                    InstitutionType_FK = instM.InstitutionType_FK,
                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss")
                };

                var response = _DR.ValidateInstitution(inst);
                if (response == false)
                {
                    DataWriter.CreateIntitution(inst);
                    TempData["ErrMsg"] = "Record Added";
                }
                else if (response == true)
                {
                    TempData["ErrMsg"] = "Record Already Exist";
                }
                ViewBag.Data = _DR.getAllInstitution();
                GetMenus();

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult CreateUser(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                User Users = new User();
                string password = Convert.ToString(form["password"]);
                string rpassword = Convert.ToString(form["cpassword"]);
                Users.EmailAddress = Convert.ToString(form["email"]);
                Users.Firstname = Convert.ToString(form["fname"]);
                Users.Lastname = Convert.ToString(form["lname"]);
                Users.PhoneNumber = Convert.ToString(form["phone"]);
                Users.ReferralLevel = 0;
                string pasw = password;
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(password, HashName.SHA256);
                WebLog.Log("Users" + password);
                WebLog.Log("Users1" + rpassword);
                WebLog.Log("Users" + Users.EmailAddress);
                WebLog.Log("Users1" + Users.Firstname);
                WebLog.Log("Users1" + Users.Lastname);
                WebLog.Log("Users1" + Users.PhoneNumber);
                bool validatepas = _usr.ValidatePassword(password, rpassword);
                if (validatepas == false)
                {
                    TempData["ErrMsg"] = "Password And Confirm Password Must Match";
                    WebLog.Log("ValidatePass" + validatepas);
                    return View("CreateUser");
                }
                if (validatepas == true)
                {
                    bool val = _DR.Validate(Users.EmailAddress, Users.PhoneNumber);
                    WebLog.Log("val" + val);
                    if (val == true)
                    {
                        WebLog.Log("val1" + val);
                        TempData["ErrMsg"] = "User Already Exist ! Please Check Phone Or Email";
                        GetMenus();
                        return View("CreateUser");
                        //return RedirectToAction("CreateUser");
                    }
                    else if (val == false)
                    {
                        WebLog.Log("valfalse" + val);
                        password = EncrypPassword;
                        Users.PaswordVal = password;
                        var Userid = _DM.InsertUser(Users);
                        if (Userid != 0)
                        {
                            /*UserRole UserRoles = new UserRole();
                            UserRoles.User_FK = Users.ID;
                            UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["DefaultUser"]);
                            UserRoles.IsVisible = 1;
                            _DM.InsertUserRoles(UserRoles);*/
                            GetMenus();
                            UserController uc = new UserController();
                           // Users.EmailAddress = "sayoola@paelyt.com";
                           // pasw = "password";
                            uc.SendEmail(Users.EmailAddress,pasw);
                            TempData["SucMsg"] = "User Created Succesfully";
                            //return View("CreateUser");
                            return RedirectToAction("CreateUser");
                        }
                    }
                    GetMenus();
                }
                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public void EmptyTemp()
        {
            TempData["SucMsg"] = null; TempData["ErrMsg"] = null; TempData["Error"] = null;
        }

        [HttpPost]
        public ActionResult UpdateLoanApp(DataAccessLayerT.Classes.LoanApplication LoanApp,FormCollection form)
        {
            try
            {
                //TempData["SucMsg"] = null; TempData["ErrMsg"] = null;
                EmptyTemp();

                DataAccessLayerT.DataManager.LoanApplication LoanApps = new LoanApplication();
                DataAccessLayerT.DataManager.EmployerLoanDetail EmpDetails = new DataAccessLayerT.DataManager.EmployerLoanDetail();
                var FlutterwaveBankCode = dp.GetFlutterwaveCode(LoanApp.BankCode);
                LoanApps.ID = LoanApp.ID;
                LoanApps.Institution_FK = LoanApp.institutionFk;
                LoanApps.RepaymentMethod_FK = LoanApp.RepaymentMethod_FK;
                //LoanApps.MeansOfIDFilePath = LoanApp.ValueTime;
                LoanApps.AccomodationType_FK = LoanApp.AccomodationType_FK;
                LoanApps.AccountName = LoanApp.AccountName;
                LoanApps.AccountNumber = LoanApp.AccountNumber;
                LoanApps.ApplicantID = LoanApp.ApplicantID;
                LoanApps.ApplicationStatus_FK = LoanApp.ApplicationStatus_FK;
                LoanApps.BankCode = Convert.ToString(LoanApp.BankCode);
                LoanApps.RemitaBankCode = Convert.ToString(FlutterwaveBankCode.Code);
                LoanApps.BVN = LoanApp.BVN;
                LoanApps.ClosestBusStop = LoanApp.ClosestBusStop;
                LoanApps.ContactAddress = LoanApp.ContactAddress;
                // LoanApps.CreatedBy = LoanApp.CreatedBy; //Change To User ID
                LoanApps.DateCreated = LoanApp.DateCreated;
                LoanApps.DateModified = LoanApp.DateModified;
               // LoanApps.DateOfBirth = Convert.ToString(LoanApp.DateOfBirth);
                LoanApps.DateOfBirth = Convert.ToString(form["DateOfBirth"]).Remove(0,1);
                LoanApps.EmailAddress = LoanApp.EmailAddress;
                LoanApps.ExistingLoan = LoanApp.ExistingLoan;
                LoanApps.ExistingLoan_NoOfMonthsLeft = LoanApp.ExistingLoan_NoOfMonthsLeft;
                LoanApps.ExistingLoan_OutstandingAmount = LoanApp.ExistingLoan_OutstandingAmount;
                LoanApps.Firstname = LoanApp.Firstname;
                LoanApps.Gender_FK = LoanApp.Gender_FK;
                LoanApps.IdentficationNumber = LoanApp.IdentficationNumber;
                LoanApps.Landmark = LoanApp.Landmark;
                LoanApps.LGA_FK = LoanApp.LGA_FK;
                //LoanApps.LoanAmount = LoanApp.LoanAmount;
                LoanApps.LoanAmount = Convert.ToDouble(form["LoanAmount"]);
                LoanApps.LoanComment = "";
                LoanApps.LoanRefNumber = LoanApp.LoanRefNumber;
                LoanApps.LoanTenure = LoanApp.LoanTenure;
                LoanApps.MaritalStatus_FK = LoanApp.MaritalStatus_FK;
                LoanApps.MeansOfID_FK = LoanApp.MeansOfID_FK;
                LoanApps.NOK_EmailAddress = LoanApp.NOK_EmailAddress;
                LoanApps.NOK_FullName = LoanApp.NOK_FullName;
                LoanApps.NOK_HomeAddress = LoanApp.NOK_HomeAddress;
                LoanApps.NOK_PhoneNumber = LoanApp.NOK_PhoneNumber;
                LoanApps.NOK_Relationship = LoanApp.NOK_Relationship;
                LoanApps.Organization = LoanApp.Organization;
                LoanApps.Othernames = LoanApp.Othernames;
                LoanApps.PhoneNumber = LoanApp.PhoneNumber;
                LoanApps.RepaymentMethod_FK = LoanApp.RepaymentMethod_FK;
                LoanApps.StateofResidence_FK = LoanApp.StateofResidence_FK;
                LoanApps.Surname = LoanApp.Surname;
                LoanApps.Title_FK = LoanApp.Title_FK;
                LoanApps.IsVisible = 1;
                LoanApps.ValueDate = LoanApp.ValueDate;//MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                LoanApps.ValueTime = LoanApp.ValueTime; //MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
                LoanApps.LoanProduct_FK = 2;
                LoanApps.ApplicationStatus_FK = 3;

                EmpDetails.LoanApplication_FK = LoanApps.ID;
                EmpDetails.Department = LoanApp.Department;
                EmpDetails.Designation = LoanApp.Designation;
                EmpDetails.EmployerID = LoanApp.ApplicantID;
                EmpDetails.EmploymentStatus_FK = LoanApp.Contract;
                //EmpDetails.NetMonthlyIncome = LoanApp.SalaryAmount;
                EmpDetails.NetMonthlyIncome = Convert.ToDouble(form["SalaryAmount"]);
                EmpDetails.OfficialEmailAddress = LoanApp.OfficialEmail;
                EmpDetails.LengthOfServiceInMth = LoanApp.LOS;
                EmpDetails.Occupation = LoanApp.Occupation;


                var resp = _DM.UpdateMyLoanApplication(LoanApps);
                if (resp > 0)
                {
                    _DM.UpdateEmployeeInfo(EmpDetails);
                    TempData["Succes"] = null;
                    // _DM.updateEmployee(EmpDetails);
                    TempData["SucMsg"] = "Loan Application Details Succesfuly Updated";
                    // return RedirectToAction("GetApplication", new { @Refid = LoanApps.LoanRefNumber });
                    return RedirectToAction("MyApplications");
                }
                if (resp == 0)
                {
                    TempData["Error"] = "Please Kindly Refix";
                    return RedirectToAction("GetApplication", new { @Refid = LoanApps.LoanRefNumber });
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<AppLoan> getComment(int LoanFk)
        {
            try
            {
                var comment = _DR.GetLoanComments(LoanFk);
                return comment;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public void GetApplicantInfo(string Refid, TableObjects.LoanApplication LoanApp, AppLoan Apploan, AppLoan LoanApps)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                // AppLoan LoanApps = new AppLoan();
                Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                Apploan.ContactAddress = string.IsNullOrEmpty(LoanApps.ContactAddress) ? "none" : LoanApps.ContactAddress;
                Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;

                Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                Apploan.Repayment = string.IsNullOrEmpty(LoanApps.Repayment) ? "none" : LoanApps.Repayment;
                Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                Apploan.Organization =
                    string.IsNullOrEmpty(LoanApps.Organization) ? "none" : LoanApps.Organization;
                Apploan.NOK_Relationship =
                    string.IsNullOrEmpty(LoanApps.NOK_Relationship) ? "none" : LoanApps.NOK_Relationship;
                Apploan.NOK_PhoneNumber = string.IsNullOrEmpty(LoanApps.NOK_PhoneNumber) ? "none" : LoanApps.NOK_PhoneNumber;
                Apploan.NOK_HomeAddress = string.IsNullOrEmpty(LoanApps.NOK_HomeAddress) ? "none" : LoanApps.NOK_HomeAddress;
                Apploan.NOK_FullName = string.IsNullOrEmpty(LoanApps.NOK_FullName) ? "none" : LoanApps.NOK_FullName;
                Apploan.NOK_EmailAddress =
                    string.IsNullOrEmpty(LoanApps.NOK_EmailAddress) ? "none" : LoanApps.NOK_EmailAddress;
                Apploan.MeansOfIdentifications =
                    string.IsNullOrEmpty(LoanApps.MeansOfIdentifications) ? "none" : LoanApps.MeansOfIdentifications;
                Apploan.MaritalStatus =
                    string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
                Apploan.LoanAmount = LoanApps.LoanAmount;
                string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = utilities.ConvertToCurrency(LoanAmt);
                Apploan.Landmark =
                    string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                Apploan.IdentficationNumber = string.IsNullOrEmpty(LoanApps.IdentficationNumber) ? "none" : LoanApps.IdentficationNumber;
                Apploan.ExistingLoan = LoanApps.ExistingLoan;
                if(Apploan.ExistingLoan == true)
                {
                    Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                    string ConvertedExistingAmount = Convert.ToString(Apploan.ExistingLoan_OutstandingAmount);
                    Apploan.ConvertedExistingAmount = utilities.ConvertToCurrency(ConvertedExistingAmount);
                    
                }
                Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                Apploan.Firstname =
                    string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.ID = LoanApps.ID;
                Apploan.LoanRefNumber =
                    string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                Apploan.ClosestBusStop =
               string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.Department =
                string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                Apploan.Occupation =
               string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                Apploan.EmployeeStatus = string.IsNullOrEmpty(LoanApps.EmployeeStatus) ? "none" : LoanApps.EmployeeStatus;
                //Apploan.ApplicationStatus = LoanApps.AppStat == 3 ? "Recommended" : "none" ;
                Apploan.Designation = string.IsNullOrEmpty(LoanApps.Designation) ? "none" : LoanApps.Designation;
                Apploan.Salary = LoanApps.Salary;
                string SalAmt = Convert.ToString(Apploan.Salary);
                Apploan.ConvertedSalary = utilities.ConvertToCurrency(SalAmt);
                Apploan.LengthOfServices = Convert.ToString(LoanApps.LengthOfService)+"Year";
                Apploan.LengthOfServiceMonths = Convert.ToString(LoanApps.LengthOfServiceMonth)+"Month";
                Apploan.GuarSurname = string.IsNullOrEmpty(LoanApps.GuarSurname) ? "none" : LoanApps.GuarSurname;
                Apploan.GuarOthernames = string.IsNullOrEmpty(LoanApps.GuarOthernames) ? "none" : LoanApps.GuarOthernames;
                Apploan.GuarEmail = string.IsNullOrEmpty(LoanApps.GuarEmail) ? "none" : LoanApps.GuarEmail;
                Apploan.GuarPhone = string.IsNullOrEmpty(LoanApps.GuarPhone) ? "none" : LoanApps.GuarPhone;
                Apploan.GuarRelationship = string.IsNullOrEmpty(LoanApps.GuarRelationship) ? "none" : LoanApps.GuarRelationship;
                Apploan.GuarContact = string.IsNullOrEmpty(LoanApps.GuarContact) ? "none" : LoanApps.GuarContact;
                Apploan.InstitutionCode = string.IsNullOrEmpty(LoanApps.InstitutionCode) ? "none" : LoanApps.InstitutionCode;
                 //string IdentImage = LoanApps.IdentficationNumberImage.After("C:\\Users\\Reliance Limited\\Documents\\Visual Studio 2015\\Projects\\UvlotApplication\\UvlotApplication\\").Replace("\\", "/") ;
                // string IdentImage = LoanApps.IdentficationNumberImage.After("C:\\Users\\Reliance Limited\\Documents\\Visual Studio 2015\\Projects\\UvlotApplication\\UvlotApplication\\").Replace("\\", "/");
                string IdentImage = LoanApps.IdentficationNumberImage.After("h:\\root\\home\\paelyt-001\\www\\Uvlot\\").Replace("\\", "/");

                WebLog.Log("Image Path" + IdentImage);
                string slash = "/";
                Apploan.IdentficationNumberImage = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                Apploan.ID = LoanApps.ID;
                Apploan.institutionFk = LoanApps.institutionFk;
                Apploan.bank_codes = LoanApps.bank_codes;
                Apploan.flutterwaveBanCode = string.IsNullOrEmpty(LoanApps.flutterwaveBanCode) ? "none" : LoanApps.flutterwaveBanCode;
                Apploan.DisburseDate = LoanApps.DisburseDate;
                var comment = getComment(LoanApps.ID);
                ViewBag.comment = comment;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }

        [HttpGet]
        public ActionResult UpdateApplication(AppLoan LoanAp)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoanRecord = _DR.getApplicantEmpInfo(LoanAp.ID);
                DataAccessLayerT.Classes.LoanApplication LP = new DataAccessLayerT.Classes.LoanApplication();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                if (LoanAp.ID != 0)
                {
                    LoansController Ln = new LoansController();
                    TempData["ErrMsg"] = "Application Not Valid";

                    LP.SalaryAmount = Convert.ToUInt64(LoanRecord.SalaryAmount);
                    LP.Designation = LoanRecord.Designation;
                    LP.Department = LoanRecord.Department;
                    LP.ApplicantID = LoanRecord.ApplicantID;
                    LP.OfficialEmail = LoanRecord.OfficialEmail;
                    LP.LOS = Convert.ToInt32(LoanRecord.LOS);
                    LP.Occupation = LoanRecord.Occupation;
                    LP.LoanTenure = LoanRecord.LoanTenure;
                    LP.LoanAmount = LoanRecord.LoanAmount;
                    LP.Organization = LoanRecord.Organization;
                    ViewData["nemploymentStatus"] = new SelectList(Ln.GetAppStatus(), "Value", "Text", LoanRecord.EmployeeStatusFK);
                }
                return View(LP);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        //public HttpResponseMessage MyAction([FromBody] JObject json)
        //{

        //}
        [HttpPost]
        public ActionResult UpdateApplication(int id, Classes.TableObjects.LoanApplication LoanAp)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                LoansController Ln = new LoansController();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                EmployerLoanDetail EMPLD = new EmployerLoanDetail();
                EMPLD.ID = id;
                EMPLD.NetMonthlyIncome = Convert.ToDouble(LoanAp.SalaryAmount);
                EMPLD.LengthOfServiceInMth = Convert.ToInt16(LoanAp.LOS);
                EMPLD.EmployerID = LoanAp.ApplicantID;
                EMPLD.Occupation = LoanAp.Occupation;
                EMPLD.Designation = LoanAp.Designation;
                EMPLD.Department = LoanAp.Department;
                EMPLD.NetMonthlyIncome = Convert.ToDouble(LoanAp.SalaryAmount);
                EMPLD.EmploymentStatus_FK = LoanAp.Contract;

                double repaymentmsg = 0;
                string respmsg = "";
                var appUser = user;
                var User = _DR.getUser(user);
                GetMenus();
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (id != 0)
                {
                    var LoanAmt = Convert.ToDouble(LoanAp.LoanAmount);
                    ViewData["nemploymentStatus"] = new SelectList(Ln.GetAppStatus(), "Value", "Text", EMPLD.EmploymentStatus_FK);
                    bool ValidateLoan = lc.PayrollLoanCalculation((double)EMPLD.NetMonthlyIncome, LoanAmt, LoanAp.LoanTenure, out repaymentmsg, out respmsg, LoanAp.Organization);
                    if (ValidateLoan == false)
                    {
                        TempData["ErrMsg"] = "Loan Amount Invalid";
                    }
                    var EmpAppUpdate = _DM.UpdateEmpInfo(EMPLD);
                    TempData["ErrMsg"] = "Update Succesful";
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult RecommendStudentLoan(string Refid, TableObjects.LoanApplication LoanApp, AppLoan Apploan)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("RecommendStudentLoan");
                }
                AppStatFk = 3;
                GetMenus();
                var LoanApps = _DR.GetStudentLoanApplication(Refid, AppStatFk);

                if (LoanApps == null)
                {
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("RecommendStudentLoan");
                }

                /*Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                Apploan.ContactAddress = string.IsNullOrEmpty(LoanApps.ContactAddress) ? "none" : LoanApps.ContactAddress;
                Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;

                Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                Apploan.Repayment = string.IsNullOrEmpty(LoanApps.Repayment) ? "none" : LoanApps.Repayment;
                Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                Apploan.Organization =
                    string.IsNullOrEmpty(Apploan.Organization) ? "none" : Apploan.Organization;
                Apploan.NOK_Relationship =
                    string.IsNullOrEmpty(LoanApps.NOK_Relationship) ? "none" : LoanApps.NOK_Relationship;
                Apploan.NOK_PhoneNumber = string.IsNullOrEmpty(LoanApps.NOK_PhoneNumber) ? "none" : LoanApps.NOK_PhoneNumber;
                Apploan.NOK_HomeAddress = string.IsNullOrEmpty(LoanApps.NOK_HomeAddress) ? "none" : LoanApps.NOK_HomeAddress;
                Apploan.NOK_FullName = string.IsNullOrEmpty(LoanApps.NOK_FullName) ? "none" : LoanApps.NOK_FullName;
                Apploan.NOK_EmailAddress =
                    string.IsNullOrEmpty(LoanApps.NOK_EmailAddress) ? "none" : LoanApps.NOK_EmailAddress;
                Apploan.MeansOfIdentifications =
                    string.IsNullOrEmpty(LoanApps.MeansOfIdentifications) ? "none" : LoanApps.MeansOfIdentifications;
                Apploan.MaritalStatus =
                    string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
                Apploan.LoanAmount = LoanApps.LoanAmount;
                string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = utilities.ConvertToCurrency(LoanAmt);
                Apploan.Landmark =
                    string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                Apploan.IdentficationNumber = string.IsNullOrEmpty(LoanApps.IdentficationNumber) ? "none" : LoanApps.IdentficationNumber;
                Apploan.ExistingLoan = LoanApps.ExistingLoan;
                Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                Apploan.Firstname =
                    string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.ID = LoanApps.ID;
                Apploan.LoanRefNumber =
                    string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                Apploan.ClosestBusStop =
               string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.Department =
                string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                Apploan.Occupation =
               string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                Apploan.EmployeeStatus = string.IsNullOrEmpty(LoanApps.EmployeeStatus) ? "none" : LoanApps.EmployeeStatus;
                //Apploan.ApplicationStatus = LoanApps.AppStat == 3 ? "Recommended" : "none" ;
                Apploan.Salary = LoanApps.Salary;
                string SalAmt = Convert.ToString(Apploan.Salary.ToString());
                Apploan.ConvertedSalary = utilities.ConvertToCurrency(SalAmt);
                */
                GetApplicantInfo(Refid, LoanApp, Apploan, LoanApps);
                //  TempData["LoanObj"] = LoanApp;
                if (Apploan.ApplicationStatus == "2")
                {
                    TempData["Msg"] = "Loan Application Already Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "1")
                {
                    TempData["Msg"] = "Loan Application Already Approved By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "7")
                {
                    TempData["Msg"] = "Loan Application Not Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "5")
                {
                    TempData["Msg"] = "Loan Application Declined By" + User.Firstname;
                }
                TempData["LoanObj"] = Apploan;
                GetMenus();
                // return View(LoanApp);
                return View(Apploan);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult Recommend(string Refid, TableObjects.LoanApplication LoanApp, AppLoan Apploan)
        {
            try
            {
                 TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("RecommendLoan");
                }
                AppStatFk = 3;
                GetMenus();
                var LoanApps = _DR.GetLoanApplication(Refid, AppStatFk);

                if (LoanApps == null)
                {
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("RecommendLoan");
                }

                /*Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                Apploan.ContactAddress = string.IsNullOrEmpty(LoanApps.ContactAddress) ? "none" : LoanApps.ContactAddress;
                Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;

                Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                Apploan.Repayment = string.IsNullOrEmpty(LoanApps.Repayment) ? "none" : LoanApps.Repayment;
                Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                Apploan.Organization =
                    string.IsNullOrEmpty(Apploan.Organization) ? "none" : Apploan.Organization;
                Apploan.NOK_Relationship =
                    string.IsNullOrEmpty(LoanApps.NOK_Relationship) ? "none" : LoanApps.NOK_Relationship;
                Apploan.NOK_PhoneNumber = string.IsNullOrEmpty(LoanApps.NOK_PhoneNumber) ? "none" : LoanApps.NOK_PhoneNumber;
                Apploan.NOK_HomeAddress = string.IsNullOrEmpty(LoanApps.NOK_HomeAddress) ? "none" : LoanApps.NOK_HomeAddress;
                Apploan.NOK_FullName = string.IsNullOrEmpty(LoanApps.NOK_FullName) ? "none" : LoanApps.NOK_FullName;
                Apploan.NOK_EmailAddress =
                    string.IsNullOrEmpty(LoanApps.NOK_EmailAddress) ? "none" : LoanApps.NOK_EmailAddress;
                Apploan.MeansOfIdentifications =
                    string.IsNullOrEmpty(LoanApps.MeansOfIdentifications) ? "none" : LoanApps.MeansOfIdentifications;
                Apploan.MaritalStatus =
                    string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
                Apploan.LoanAmount = LoanApps.LoanAmount;
                string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = utilities.ConvertToCurrency(LoanAmt);
                Apploan.Landmark =
                    string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                Apploan.IdentficationNumber = string.IsNullOrEmpty(LoanApps.IdentficationNumber) ? "none" : LoanApps.IdentficationNumber;
                Apploan.ExistingLoan = LoanApps.ExistingLoan;
                Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                Apploan.Firstname =
                    string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.ID = LoanApps.ID;
                Apploan.LoanRefNumber =
                    string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                Apploan.ClosestBusStop =
               string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.Department =
                string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                Apploan.Occupation =
               string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                Apploan.EmployeeStatus = string.IsNullOrEmpty(LoanApps.EmployeeStatus) ? "none" : LoanApps.EmployeeStatus;
                //Apploan.ApplicationStatus = LoanApps.AppStat == 3 ? "Recommended" : "none" ;
                Apploan.Salary = LoanApps.Salary;
                string SalAmt = Convert.ToString(Apploan.Salary.ToString());
                Apploan.ConvertedSalary = utilities.ConvertToCurrency(SalAmt);
                */
                GetApplicantInfo(Refid, LoanApp, Apploan, LoanApps);
                //  TempData["LoanObj"] = LoanApp;
                if (Apploan.ApplicationStatus == "2")
                {
                    TempData["Msg"] = "Loan Application Already Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "1")
                {
                    TempData["Msg"] = "Loan Application Already Approved By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "7")
                {
                    TempData["Msg"] = "Loan Application Not Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "5")
                {
                    TempData["Msg"] = "Loan Application Declined By" + User.Firstname;
                }
                TempData["LoanObj"] = Apploan;
                GetMenus();
                // return View(LoanApp);
                return View(Apploan);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult Recommend(FormCollection form, TableObjects.LoanApplication LoanApps, DataAccessLayerT.DataManager.AppLoan LoanApp)
        {
            try
            {
                // TempData["SucMsg"] = null; TempData["ErrMsg"] = null; TempData["Error"] = null;
                EmptyTemp();
                GetMenus();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                LoanApp = (DataAccessLayerT.DataManager.AppLoan)TempData["LoanObj"];

                LoanApproval.Comment = Convert.ToString(form["comment"]).Replace(",", "").Trim();
                LoanApproval.CommentBy = Convert.ToInt16(userid);
                var ido = Convert.ToString(form["Accept"]);
                LoanApproval.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanApproval.DateCreated = MyUtility.getCurrentLocalDateTime();
                LoanApproval.ValueDate = LoanApp.ValueDate;
                LoanApproval.ValueTime = LoanApp.ValueTime;
                LoanApproval.LoanApplication_FK = LoanApp.ID;
                LoanApproval.IsVisible = 1;
                LoanAp.LoanRefNumber = LoanApp.LoanRefNumber;
                LoanAp.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanAp.LoanComment = Convert.ToString(form["comment"]).Replace(",", "").Trim();
                LoanAp.DateModified = MyUtility.getCurrentLocalDateTime();
                /* Create user After Loan Approved 
                users.Firstname = LoanApp.Firstname;
                users.Lastname = LoanApp.Surname;
                users.PhoneNumber = LoanApp.PhoneNumber;
                users.PaswordVal = ConfigurationManager.AppSettings["RecommendLevelPassword"];
                var password = users.PaswordVal;
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(password, HashName.SHA256);
                users.EmailAddress = LoanApp.EmailAddress;
                users.UserAddress = LoanApp.ContactAddress;
                users.ValueDate = MyUtility.getCurrentLocalDateTime();
                users.IsVisible = 1;
                users.LastUpdated = null;
                users.PinNumber = "";
                users.PaswordVal = EncrypPassword;
                //users.Title_FK = Convert.ToInt16(LoanApp.Title);
                //users.GenderID = Convert.ToInt16(LoanApp.Gender);
                users.DateOfBirth = Convert.ToString(LoanApp.DateOfBirth);
                users.ReferralCode = "";
                users.MyReferralCode = "";
                users.Audit = "";
                users.ReferralLevel = 0;
                */
                LoansController lc = new LoansController();
                // call this guy from here .

                if (LoanApproval.ApplicationStatus_FK == 8)
                {
                    _DM.UpdateLoanApp(LoanAp);
                    TempData["ErrMsg"] = "Loan Application Rejected";
                   // TempData["SucMsg"] = null; TempData["Error"] = null;
                    LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                    //  return RedirectToAction("Reject");
                    return RedirectToAction("RecommendLoan");
                }
                var salary = _DR.GetSalary(LoanApp.ID);
                if (salary == null)
                {
                    TempData["ErrMsg"] = "Please Check The Salary";
                   // TempData["SucMsg"] = null; TempData["Error"] = null;
                    //  RedirectToAction("Reject");
                    return RedirectToAction("RecommendLoan");
                }
                InstCode = LoanApp.InstitutionCode;
                var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
                var salaryAmount = Convert.ToDouble(salary.NetMonthlyIncome);
                var LoanLegder = lc.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage, InstCode);

                if (respMessage == "0")
                {

                    var recommend = _DM.UpdateLoanApp(LoanAp);
                    if (recommend > 0)
                    {
                        LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                        if (LoanApproval.ID > 0)
                        {
                            // Send Email to Users After Recommending
                            LoggedinInst = LoggedInuser.LoggedInInstitution();
                            LoanApp.InstitutionEmailAddress = LoggedinInst;
                            lc.SendEmail(LoanApp);
                            // Send Mail to The Next Level
                            // var Roleid = 3;
                            int Roleid = Convert.ToInt16(ConfigurationManager.AppSettings["ApproveRole"]);
                            
                            var EmailList = _DR.GetNextLevelUser(Roleid);
                            if (EmailList.Count > 1)
                            {
                                var i = 0;
                                for (i = 0; i < EmailList.Count; i++)
                                {
                                    user = EmailList[i];
                                    SendNextLevelEmail(user,LoggedinInst);
                                }
                            }
                            if (EmailList.Count == 1)
                            {
                                SendNextLevelEmail(EmailList[0],LoggedinInst);
                            }

                            //bool valid = _DR.Validate(users.EmailAddress, users.PhoneNumber);
                            //  if (valid == false)
                            //  {
                            //if (users.EmailAddress == null || users.EmailAddress == "")
                            //      {
                            //          users.EmailAddress = users.PhoneNumber;
                            //      }
                            // var CreateUser = _DM.InsertUser(users);
                            /*  UserRole UserRoles = new UserRole();
                              UserRoles.User_FK = users.ID;
                              UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["DefaultUser"]);
                              UserRoles.IsVisible = 1;
                              _DM.InsertUserRoles(UserRoles);*/
                            // return RedirectToAction("Acknowledgement");
                            //    TempData["Offer"] = "1";
                            //    return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });
                            //}
                            TempData["Offer"] = "1";

                            //  return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });
                            TempData["SucMsg"] = "Loan Recommendation Complete!";
                            //TempData["ErrMsg"] = null; TempData["Error"] = null;
                            //   return RedirectToAction("Recommend", new { @Refid = LoanApp.LoanRefNumber });
                            return RedirectToAction("RecommendLoan");
                        }
                        else
                        {
                            TempData["Error"] = "Error Recommending Loan! Try Again";
                          //  TempData["ErrMsg"] = null; TempData["SucMsg"] = null;
                            return RedirectToAction("Recommend", new { @Refid = LoanApp.LoanRefNumber });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Error Recommending Loan! Try Again";
                       // TempData["ErrMsg"] = null; TempData["SucMsg"] = null;
                        return RedirectToAction("Recommend", new { @Refid = LoanApp.LoanRefNumber });
                    }
                }
                else
                {
                    TempData["Error"] = respMessage;
                    TempData["ErrMsg"] = null; TempData["SucMsg"] = null;
                    return RedirectToAction("Recommend", new { @Refid = LoanApp.LoanRefNumber });
                }



            }

            catch (Exception ex)
            {
                if (LoanApp == null)
                    return RedirectToAction("RecommendLoan");
                return null;
            }

        }




        [HttpGet]
        public ActionResult Recommends(string Refid, TableObjects.LoanApplication LoanApp, AppLoan Apploan)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("RecommendLoans");
                }
                AppStatFk = 3;
                GetMenus();
                var LoanApps = _DR.GetLoanApplication(Refid, AppStatFk);

                if (LoanApps == null)
                {
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("RecommendLoans");
                }

                /*Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                Apploan.ContactAddress = string.IsNullOrEmpty(LoanApps.ContactAddress) ? "none" : LoanApps.ContactAddress;
                Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;

                Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                Apploan.Repayment = string.IsNullOrEmpty(LoanApps.Repayment) ? "none" : LoanApps.Repayment;
                Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                Apploan.Organization =
                    string.IsNullOrEmpty(Apploan.Organization) ? "none" : Apploan.Organization;
                Apploan.NOK_Relationship =
                    string.IsNullOrEmpty(LoanApps.NOK_Relationship) ? "none" : LoanApps.NOK_Relationship;
                Apploan.NOK_PhoneNumber = string.IsNullOrEmpty(LoanApps.NOK_PhoneNumber) ? "none" : LoanApps.NOK_PhoneNumber;
                Apploan.NOK_HomeAddress = string.IsNullOrEmpty(LoanApps.NOK_HomeAddress) ? "none" : LoanApps.NOK_HomeAddress;
                Apploan.NOK_FullName = string.IsNullOrEmpty(LoanApps.NOK_FullName) ? "none" : LoanApps.NOK_FullName;
                Apploan.NOK_EmailAddress =
                    string.IsNullOrEmpty(LoanApps.NOK_EmailAddress) ? "none" : LoanApps.NOK_EmailAddress;
                Apploan.MeansOfIdentifications =
                    string.IsNullOrEmpty(LoanApps.MeansOfIdentifications) ? "none" : LoanApps.MeansOfIdentifications;
                Apploan.MaritalStatus =
                    string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
                Apploan.LoanAmount = LoanApps.LoanAmount;
                string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = utilities.ConvertToCurrency(LoanAmt);
                Apploan.Landmark =
                    string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                Apploan.IdentficationNumber = string.IsNullOrEmpty(LoanApps.IdentficationNumber) ? "none" : LoanApps.IdentficationNumber;
                Apploan.ExistingLoan = LoanApps.ExistingLoan;
                Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                Apploan.Firstname =
                    string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.ID = LoanApps.ID;
                Apploan.LoanRefNumber =
                    string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                Apploan.ClosestBusStop =
               string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.Department =
                string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                Apploan.Occupation =
               string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                Apploan.EmployeeStatus = string.IsNullOrEmpty(LoanApps.EmployeeStatus) ? "none" : LoanApps.EmployeeStatus;
                //Apploan.ApplicationStatus = LoanApps.AppStat == 3 ? "Recommended" : "none" ;
                Apploan.Salary = LoanApps.Salary;
                string SalAmt = Convert.ToString(Apploan.Salary.ToString());
                Apploan.ConvertedSalary = utilities.ConvertToCurrency(SalAmt);
                */
                GetApplicantInfo(Refid, LoanApp, Apploan, LoanApps);
                //  TempData["LoanObj"] = LoanApp;
                if (Apploan.ApplicationStatus == "2")
                {
                    TempData["Msg"] = "Loan Application Already Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "1")
                {
                    TempData["Msg"] = "Loan Application Already Approved By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "7")
                {
                    TempData["Msg"] = "Loan Application Not Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "5")
                {
                    TempData["Msg"] = "Loan Application Declined By" + User.Firstname;
                }
                TempData["LoanObj"] = Apploan;
                GetMenus();
                // return View(LoanApp);
                return View(Apploan);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult Recommends(FormCollection form, TableObjects.LoanApplication LoanApps, DataAccessLayerT.DataManager.AppLoan LoanApp)
        {
            try
            {
                // TempData["SucMsg"] = null; TempData["ErrMsg"] = null; TempData["Error"] = null;
                EmptyTemp();
                GetMenus();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                LoanApp = (DataAccessLayerT.DataManager.AppLoan)TempData["LoanObj"];

                LoanApproval.Comment = Convert.ToString(form["comment"]).Replace(",", "").Trim();
                LoanApproval.CommentBy = Convert.ToInt16(userid);
                var ido = Convert.ToString(form["Accept"]);
                LoanApproval.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanApproval.DateCreated = MyUtility.getCurrentLocalDateTime();
                LoanApproval.ValueDate = LoanApp.ValueDate;
                LoanApproval.ValueTime = LoanApp.ValueTime;
                LoanApproval.LoanApplication_FK = LoanApp.ID;
                LoanApproval.IsVisible = 1;
                LoanAp.LoanRefNumber = LoanApp.LoanRefNumber;
                LoanAp.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanAp.LoanComment = Convert.ToString(form["comment"]).Replace(",", "").Trim();
                LoanAp.DateModified = MyUtility.getCurrentLocalDateTime();
                /* Create user After Loan Approved 
                users.Firstname = LoanApp.Firstname;
                users.Lastname = LoanApp.Surname;
                users.PhoneNumber = LoanApp.PhoneNumber;
                users.PaswordVal = ConfigurationManager.AppSettings["RecommendLevelPassword"];
                var password = users.PaswordVal;
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(password, HashName.SHA256);
                users.EmailAddress = LoanApp.EmailAddress;
                users.UserAddress = LoanApp.ContactAddress;
                users.ValueDate = MyUtility.getCurrentLocalDateTime();
                users.IsVisible = 1;
                users.LastUpdated = null;
                users.PinNumber = "";
                users.PaswordVal = EncrypPassword;
                //users.Title_FK = Convert.ToInt16(LoanApp.Title);
                //users.GenderID = Convert.ToInt16(LoanApp.Gender);
                users.DateOfBirth = Convert.ToString(LoanApp.DateOfBirth);
                users.ReferralCode = "";
                users.MyReferralCode = "";
                users.Audit = "";
                users.ReferralLevel = 0;
                */
                LoansController lc = new LoansController();
                // call this guy from here .

                if (LoanApproval.ApplicationStatus_FK == 8)
                {
                    _DM.UpdateLoanApp(LoanAp);
                    TempData["ErrMsg"] = "Loan Application Rejected";
                    // TempData["SucMsg"] = null; TempData["Error"] = null;
                    LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                    //  return RedirectToAction("Reject");
                    return RedirectToAction("RecommendLoans");
                }
                var salary = _DR.GetSalary(LoanApp.ID);
                if (salary == null)
                {
                    TempData["ErrMsg"] = "Please Check The Salary";
                    // TempData["SucMsg"] = null; TempData["Error"] = null;
                    //  RedirectToAction("Reject");
                    return RedirectToAction("RecommendLoans");
                }
                InstCode = LoanApp.InstitutionCode;
                var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
                var salaryAmount = Convert.ToDouble(salary.NetMonthlyIncome);
                var LoanLegder = lc.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage, InstCode);

                if (respMessage == "0")
                {

                    var recommend = _DM.UpdateLoanApp(LoanAp);
                    if (recommend > 0)
                    {
                        LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                        if (LoanApproval.ID > 0)
                        {
                            // Send Email to Users After Recommending
                            LoggedinInst = LoggedInuser.LoggedInInstitution();
                            LoanApp.InstitutionEmailAddress = LoggedinInst;
                            lc.SendEmail(LoanApp);
                            // Send Mail to The Next Level
                            // var Roleid = 3;
                            int Roleid = Convert.ToInt16(ConfigurationManager.AppSettings["ApproveRole"]);

                            var EmailList = _DR.GetNextLevelUser(Roleid);
                            if (EmailList.Count > 1)
                            {
                                var i = 0;
                                for (i = 0; i < EmailList.Count; i++)
                                {
                                    user = EmailList[i];
                                    SendNextLevelEmail(user, LoggedinInst);
                                }
                            }
                            if (EmailList.Count == 1)
                            {
                                SendNextLevelEmail(EmailList[0], LoggedinInst);
                            }

                            //bool valid = _DR.Validate(users.EmailAddress, users.PhoneNumber);
                            //  if (valid == false)
                            //  {
                            //if (users.EmailAddress == null || users.EmailAddress == "")
                            //      {
                            //          users.EmailAddress = users.PhoneNumber;
                            //      }
                            // var CreateUser = _DM.InsertUser(users);
                            /*  UserRole UserRoles = new UserRole();
                              UserRoles.User_FK = users.ID;
                              UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["DefaultUser"]);
                              UserRoles.IsVisible = 1;
                              _DM.InsertUserRoles(UserRoles);*/
                            // return RedirectToAction("Acknowledgement");
                            //    TempData["Offer"] = "1";
                            //    return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });
                            //}
                            TempData["Offer"] = "1";

                            //  return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });
                            TempData["SucMsg"] = "Loan Recommendation Complete!";
                            //TempData["ErrMsg"] = null; TempData["Error"] = null;
                            //   return RedirectToAction("Recommend", new { @Refid = LoanApp.LoanRefNumber });
                            return RedirectToAction("RecommendLoans");
                        }
                        else
                        {
                            TempData["Error"] = "Error Recommending Loan! Try Again";
                            //  TempData["ErrMsg"] = null; TempData["SucMsg"] = null;
                            return RedirectToAction("Recommends", new { @Refid = LoanApp.LoanRefNumber });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Error Recommending Loan! Try Again";
                        // TempData["ErrMsg"] = null; TempData["SucMsg"] = null;
                        return RedirectToAction("Recommends", new { @Refid = LoanApp.LoanRefNumber });
                    }
                }
                else
                {
                    TempData["Error"] = respMessage;
                    TempData["ErrMsg"] = null; TempData["SucMsg"] = null;
                    return RedirectToAction("Recommends", new { @Refid = LoanApp.LoanRefNumber });
                }



            }

            catch (Exception ex)
            {
                if (LoanApp == null)
                    return RedirectToAction("RecommendLoans");
                return null;
            }

        }



        [HttpGet]
        public ActionResult Approve(string Refid, TableObjects.LoanApplication LoanApp, AppLoan Apploan)
        {
            try
            {
                /* TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                 if(TempData["Error"] == "")
                 {
                     TempData["Error"] = "";
                 }*/
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 2;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("ApproveLoan");
                }

                var LoanApps = _DR.GetLoanApplication(Refid, AppStatFk);
                if (LoanApps == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("ApproveLoan");
                }
                /*Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                Apploan.ContactAddress = string.IsNullOrEmpty(LoanApps.ContactAddress) ? "none" : LoanApps.ContactAddress;
                Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;

                Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                Apploan.Repayment = string.IsNullOrEmpty(LoanApps.Repayment) ? "none" : LoanApps.Repayment;
                Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                Apploan.Organization =
                    string.IsNullOrEmpty(Apploan.Organization) ? "none" : Apploan.Organization;
                Apploan.NOK_Relationship =
                    string.IsNullOrEmpty(LoanApps.NOK_Relationship) ? "none" : LoanApps.NOK_Relationship;
                Apploan.NOK_PhoneNumber = string.IsNullOrEmpty(LoanApps.NOK_PhoneNumber) ? "none" : LoanApps.NOK_PhoneNumber;
                Apploan.NOK_HomeAddress = string.IsNullOrEmpty(LoanApps.NOK_HomeAddress) ? "none" : LoanApps.NOK_HomeAddress;
                Apploan.NOK_FullName = string.IsNullOrEmpty(LoanApps.NOK_FullName) ? "none" : LoanApps.NOK_FullName;
                Apploan.NOK_EmailAddress =
                    string.IsNullOrEmpty(LoanApps.NOK_EmailAddress) ? "none" : LoanApps.NOK_EmailAddress;
                Apploan.MeansOfIdentifications =
                    string.IsNullOrEmpty(LoanApps.MeansOfIdentifications) ? "none" : LoanApps.MeansOfIdentifications;
                Apploan.MaritalStatus =
                    string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
                Apploan.LoanAmount = LoanApps.LoanAmount;
                string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = utilities.ConvertToCurrency(LoanAmt);
                Apploan.Landmark =
                    string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                Apploan.IdentficationNumber = string.IsNullOrEmpty(LoanApps.IdentficationNumber) ? "none" : LoanApps.IdentficationNumber;
                Apploan.ExistingLoan = LoanApps.ExistingLoan;
                Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                Apploan.Firstname =
                    string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.ID = LoanApps.ID;
                Apploan.LoanRefNumber =
                    string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                Apploan.ClosestBusStop =
               string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.Department =
                string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                Apploan.Occupation =
               string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                Apploan.EmployeeStatus = string.IsNullOrEmpty(LoanApps.EmployeeStatus) ? "none" : LoanApps.EmployeeStatus;

                Apploan.Salary = LoanApps.Salary;
                string SalAmt = Convert.ToString(Apploan.Salary);
                Apploan.ConvertedSalary = utilities.ConvertToCurrency(SalAmt);*/
                GetApplicantInfo(Refid, LoanApp, Apploan, LoanApps);
                // the New Loan Comment 
                var comment = getComment(LoanApps.ID);
                ViewBag.comment = comment;
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult Approve(FormCollection form, TableObjects.LoanApplication LoanApps, DataAccessLayerT.DataManager.AppLoan LoanApp)
        {
            try
            {
                //TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                EmptyTemp();
                double InterestValue = 0;
                double PoratedAmt = 0;
                LoansLedger Ledger = new LoansLedger();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                LoanApp = (DataAccessLayerT.DataManager.AppLoan)TempData["LoanObj"];

                DataAccessLayerT.DataManager.LoanApplication LoanAp = new DataAccessLayerT.DataManager.LoanApplication();
                DataAccessLayerT.DataManager.LoanApproval LoanApproval = new DataAccessLayerT.DataManager.LoanApproval();
                LoanApproval.Comment = Convert.ToString(form["comment"]);
                LoanApproval.CommentBy = Convert.ToInt16(userid);
                var ido = Convert.ToString(form["Accept"]);
                LoanApproval.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanApproval.DateCreated = MyUtility.getCurrentLocalDateTime();
                LoanApproval.ValueDate = LoanApp.ValueDate;
                LoanApproval.ValueTime = LoanApp.ValueTime;
                LoanApproval.LoanApplication_FK = LoanApp.ID;
                LoanApproval.IsVisible = 1;
                LoanAp.LoanRefNumber = LoanApp.LoanRefNumber;
                LoanAp.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanAp.LoanComment = Convert.ToString(form["comment"]);
                LoanAp.DateModified = MyUtility.getCurrentLocalDateTime();
                var OfferLetterCheck = Convert.ToString(form["OfferLetter"]);
                var RemitaStandingCheck = Convert.ToString(form["RemitaMandate"]);

                if (LoanApproval.ApplicationStatus_FK == 9)
                {
                    _DM.UpdateLoanApplication(LoanAp);
                    // TempData["ErrMsg"] = LoanAp.LoanComment;
                    TempData["ErrMsg"] = "Loan Application Rejected";
                    LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                    //return RedirectToAction("Reject");
                    return RedirectToAction("ApproveLoan");
                }
                LoansController Ln = new LoansController();
                InstCode = LoanApp.InstitutionCode;
                var salary = _DR.GetSalary(LoanApp.ID);
                if (salary == null)
                {
                    TempData["ErrMsg"] = "Please Check The Salary";
                    RedirectToAction("Reject");
                }
                var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
                var salaryAmount = Convert.ToDouble(salary.NetMonthlyIncome);
                var LoanLegder = Ln.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage, InstCode);
                double TotalRepayment = Convert.ToDouble(repayment) * LoanApp.LoanTenure;
               // TotalRepayment = Math.Round(TotalRepayment, 0);
                TotalRepayment = Math.Round(TotalRepayment + Convert.ToDouble(repayment));
                LoanApp.TotalRepayment = TotalRepayment;
                if (respMessage == "0")
                {
                    // For Remita Direct Debit
                    string respCode = "";
                    if (RemitaStandingCheck == "31" && LoanApproval.ApplicationStatus_FK == 1)
                    {
                        respCode = SendRemitaLink(LoanApp, appUser, TotalRepayment);
                        if (respCode != "00")
                        {
                            TempData["Error"] = "Remita Standing Order Failed ! Please Try Again.";
                            return RedirectToAction("Approve", new { @Refid = LoanApp.LoanRefNumber });

                        }
                    }
                   
                    var resp = LoansLedger(LoanApp);

                    var recommend = _DM.UpdateLoanApplication(LoanAp);
                    if (recommend > 0)
                    {
                        // LoanApproval = _DM.UpdateLoanApproval(LoanApproval,LoanAp);
                        LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                        if (LoanApproval.ID > 0)
                        {
                            // For Sending EMails to Customers
                            LoggedinInst = LoggedInuser.LoggedInInstitution();
                            LoanApp.InstitutionEmailAddress = LoggedinInst;
                            var OfferLetterurl = ConfigurationManager.AppSettings["OfferLetterUrlLive"];
                            LoanApp.Contract = OfferLetterurl + LoanApp.LoanRefNumber;
                            // LoanApp.Contract = "http://localhost:28957/Loans/OfferLetter?Refid=" + LoanApp.LoanRefNumber;
                            var repayments = Convert.ToString(repayment);
                            LoanApp.RepaymentAmount = utilities.ConvertToCurrency(repayments);
                            //LoanApp.Contract = OfferLetter + LoanApp.LoanRefNumber;
                            LoansController Lc = new LoansController();

                         
                            // LoanApp.Contract = "http://www.uvlot.com/Loans/OfferLetter?Refid="+LoanApp.LoanRefNumber;
                            // LoanApp.GuaContract ="http://localhost:28957/Loans/GuarantorLetter?Refid=" + LoanApp.LoanRefNumber;
                            var GuaOfferLetterurl = ConfigurationManager.AppSettings["GuaOfferLetterUrlLive"];
                            LoanApp.GuaContract = GuaOfferLetterurl + LoanApp.LoanRefNumber;
                            // Sends Guarantor Letter
                             //lc.SendGuarantorLetter(Gua);
                             // lc.SendEmail(LoanApp);
                            // Send Mail to Disburse
                            // SendNextLevelEmail(user);
                            if (OfferLetterCheck == "30")
                            {
                                lc.SendEmail(LoanApp);
                                // lc.SendGuarantorLetter(LoanApp);
                            }
                           /* if (RemitaStandingCheck == "31" && LoanApproval.ApplicationStatus_FK == 1)
                            {
                                SendRemitaLink(LoanApp, appUser, TotalRepayment);
                            }
                            */

                            int Roleid = Convert.ToInt16(ConfigurationManager.AppSettings["DisburseRole"]);
                            var EmailList = _DR.GetNextLevelUser(Roleid);
                            if (EmailList.Count > 1)
                            {
                                var i = 0;
                                for (i = 0; i < EmailList.Count; i++)
                                {
                                    user = EmailList[i];
                                    SendNextLevelEmail(user, LoggedinInst);
                                }
                            }
                            if (EmailList.Count == 1)
                            {
                                SendNextLevelEmail(EmailList[0], LoggedinInst);
                            }
                            TempData["Offer"] = "0";
                            return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });
                            // TempData["SucMsg"] = "Loan Approved ";
                            //return RedirectToAction("ApproveLoan", new { @Refid = LoanApp.LoanRefNumber });

                        }
                        else
                        {
                            TempData["Error"] = "Error Approving Loan! Try Again";
                            return RedirectToAction("ApproveLoan", new { @Refid = LoanApp.LoanRefNumber });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Error Approving Loan! Try Again";
                        return RedirectToAction("ApproveLoan", new { @Refid = LoanApp.LoanRefNumber });
                    }
                }
                else if (respMessage != "0")
                {
                    TempData["Error"] = respMessage;
                    return RedirectToAction("Approve", new { @Refid = LoanApp.LoanRefNumber });

                }

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public ActionResult completedLoans()
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

        [HttpGet]
        public ActionResult RemitaTransacts()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var rec = _DR.getUser(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                int instFk = Convert.ToInt16(rec.PinNumber);
                // ViewBag.Data = _DR.GetRemitaTransact(rec.ID);
                ViewBag.Data = _DR.GetRemitaTransact(user);
                GetMenus();
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult RemitaTransact()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var rec = _DR.getUser(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                int instFk = Convert.ToInt16(rec.PinNumber);
                // ViewBag.Data = _DR.GetRemitaTransact(rec.ID);
                ViewBag.Data = _DR.GetRemitaTransact(user);
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string Codility()
        {
            try
            {

                return "";
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string SendRemitaLink(dynamic RemObj,string email,double TotalRepayment)
        {
            try
            {
                DataAccessLayerT.DataManager.AppLoan LoanApp = new DataAccessLayerT.DataManager.AppLoan();
                string repaymentmsg = "";
                string respmsg = "";
                LoansController Ln = new LoansController();
                dynamic obj = new JObject();
                var Tenure = RemObj.LoanTenure;
                obj.payerName = RemObj.Firstname;
                obj.payerEmail =RemObj.EmailAddress;
                obj.payerPhone = RemObj.PhoneNumber;
                obj.payerBankCode = RemObj.bank_codes;
                obj.refNumber = RemObj.LoanRefNumber;
                obj.payerAccount = RemObj.AccountNumber;
                //obj.amount = RemObj.LoanAmount;
                obj.amount = TotalRepayment.ToString();
                obj.startDate = MyUtility.getCurrentLocalDateTime().AddDays(1).ToString("dd'/'MM'/'yyyy");
                obj.endDate = MyUtility.getCurrentLocalDateTime().AddMonths(RemObj.LoanTenure).ToString("dd'/'MM'/'yyyy");
                //obj.mandateType = "DD";
                obj.maxNoOfDebits = Tenure.ToString(); //form["maxNoOfdebit"];

                string json = obj.ToString();
                string mandateSetupurl = ConfigurationManager.AppSettings["mandateSetupurl"];
                var data = MyUtilities.DoRemitaPost(mandateSetupurl, json);
                if (data == null)
                {
                   return ViewBag.Data = "Connectivity error";
                   //return View("CreateDirectDebit");
                }
                dynamic jObj = JObject.Parse(data);
                string myDDstr = "";
                if (jObj.respCode != "00")
                {
                   ViewBag.Data = jObj.respDescription;
                   //return View("CreateDirectDebit");
                }
                if (jObj.respCode == "00")
                {
                   
                    string link = jObj?.mandateform;
                    lc.SendRemita(RemObj,link);
                    ViewBag.Data = jObj.respDescription;
                    InsertRemitaObj(RemObj, jObj, email);
                }
                myDDstr = jObj.mandateform.ToString();
              
                return jObj.respCode;
                
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }

        [HttpGet]
        public ActionResult SetupRemita()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var rec = _DR.getUser(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                ViewBag.nBanks = dp.GetBanks();
                ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME",0);
                GetMenus();
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult SetupRemita(FormCollection form, DataAccessLayerT.Classes.LoanApplication lp)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                ViewBag.nBanks = dp.GetBanks();
                var amount = form["amount"];
	            var refNumber = Convert.ToString(form["refNumber"]);
                var payerName = Convert.ToString(form["payerName"]);
                var payerPhone = Convert.ToString(form["payerPhone"]);
                var payerEmail = Convert.ToString(form["payerEmail"]);
                var payerBankCode = lp.BankCode; //Convert.ToString(form["payerBankCode"]);
                var payerAccount = Convert.ToString(form["payerAccount"]);
                var startDate = Convert.ToDateTime(form["startDate"]);
                var mandateType = ConfigurationManager.AppSettings["mandateType"];//Convert.ToString(form["mandateType"]);
                var maxNoOfDebits = Convert.ToInt32(form["maxNoOfDebits"]);
                dynamic obj = new JObject();
                obj.maxNoOfDebits = maxNoOfDebits;
                obj.payerName = payerName;
                obj.payerEmail = payerEmail;
                obj.payerPhone = payerPhone;
                obj.payerBankCode = payerBankCode;
                obj.refNumber = refNumber;
                obj.payerAccount = payerAccount;
                var TotalRepayment = amount;
                int LT = maxNoOfDebits;
                obj.amount = TotalRepayment.ToString();
                obj.startDate = startDate.AddDays(0).ToString("dd'/'MM'/'yyyy"); //MyUtility.getCurrentLocalDateTime().AddDays(1).ToString("dd'/'MM'/'yyyy");
                obj.endDate = startDate.AddMonths(LT).AddDays(6).ToString("dd'/'MM'/'yyyy");//MyUtility.getCurrentLocalDateTime().AddMonths(LT).AddDays(6).ToString("dd'/'MM'/'yyyy");
                obj.maxNoOfDebits = maxNoOfDebits; //form["maxNoOfdebit"];
                string json = obj.ToString();
                string mandateSetupurl = ConfigurationManager.AppSettings["mandateSetupurl"];
               //string mandateSetupurl = ConfigurationManager.AppSettings["mandateSetupurlTest"]; 
               var data = MyUtilities.DoRemitaPost(mandateSetupurl, json);
                ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", 0);
                dynamic resp = JObject.Parse(data);
                if(resp.respCode == "00")
                {
                    RemitaResponse(resp,appUser,obj);
                }
                //  var  respCode = SendRemitaLink(obj, appUser, TotalRepayment);
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public void RemitaResponse(dynamic resp,string userid,dynamic obj)
        {
            try
            {
                PatnerTransactLog Pt = new PatnerTransactLog();
                var resps = Convert.ToString(resp);
                Pt.PatnerReference = resp.respCode;
                Pt.PatnerResponse = Convert.ToString(resp);
                Pt.mandateID = resp.mandateID;
                Pt.PatnerResponse = resp.Remita;
                Pt.BankAcct = obj.payerAccount;
                Pt.BankCode = obj.payerBankCode;
                Pt.RefNum = obj.refNumber;
                Pt.PatnerReference = obj.requestID;
                Pt.PatnerCode = "Remita";
                //Pt.EmailAddress = obj.payerEmail;
                Pt.EmailAddress = userid;
                Pt.PatnerUrl = resp.mandateform;
                Pt.DateCreated = MyUtility.getCurrentLocalDateTime();
                Pt.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                Pt.valueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                Pt.PatnerResponse = Convert.ToString(resps);
                _DM.InsertPatnerTransactLog(Pt);
                
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
               
            }
        }

        public int InsertRemitaObj(dynamic RemObj,dynamic jObj, string email)
        {
            try
            {

                Partner patners = new Partner();
                PatnerTransactLog PatnerLog = new PatnerTransactLog();
                 
                    PatnerLog.ContactPerson = RemObj.Firstname;
                    PatnerLog.PatnerUrl = jObj.mandateform;
                    //PatnerLog.EmailAddress = RemObj.EmailAddress;
                    PatnerLog.EmailAddress = email;
                    PatnerLog.PhoneNumber = RemObj.PhoneNumber;
                    PatnerLog.DateCreated = MyUtility.getCurrentLocalDateTime();
                    PatnerLog.RefNum = RemObj.LoanRefNumber;
                    PatnerLog.BankAcct = RemObj.AccountNumber;
                    PatnerLog.PatnerResponse = Convert.ToString(jObj);
                   // PatnerLog.PatnerReference = RemObj.requestID;
                    PatnerLog.PatnerCode = "Remita";
                
                PatnerLog.PatnerReference = jObj.respCode;

                PatnerLog.mandateID = jObj.mandateID;

            
                PatnerLog.BankCode = RemObj.bank_codes;

                PatnerLog.PatnerReference = jObj.requestID;

              
                 PatnerLog.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                 PatnerLog.valueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
           
               

                /*patners.ContactPerson = RemObj.Firstname;
                  patners.EmailAddress = RemObj.EmailAddress;
                  patners.DateCreated = MyUtility.getCurrentLocalDateTime();
                  patners.PhoneNumber = RemObj.PhoneNumber;
                  patners.PartnerID = RemObj.AccountNumber;
                  patners.PartnerKey = RemObj.LoanRefNumber;
                patners.ContactAddress = link;*/
                //patners.TokenVal = RemObj.requestID;
                //patners.myTokenTime = RemObj.mandateID;
                var resp = _DM.insertRemitaObj(PatnerLog);
                return resp.ID;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

//        [HttpPost]
//public ActionResult Convert(FormCollection collection)
//        {
//            PdfConverter pdfConverter = new PdfConverter();
//            string url = collection["TxtUrl"];
//            byte[] pdf = pdfConverter.GetPdfBytesFromUrl(url);

        //            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
        //            fileResult.FileDownloadName = "RenderedPage.pdf";

        //            return fileResult;
        //        }

        public string InsertLoanLedger(LoanLedger LoanLedger)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                return "";
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult Restructure2()
        {
            try
            {
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult Restructure1()
        {
            try
            {
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult LoanRestructring(FormCollection form, LoanApplication LoanApp, DataAccessLayerT.DataManager.AppLoan AppLoan)
        {
            try
            {
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    double Amount = Convert.ToDouble(form["NewAmount"]);
                    var ReferenceNum = LoanApp.IdentficationNumber;
                    var LoanLedger = _DR.GetLoanLedger(ReferenceNum);
                    if (LoanLedger == null)
                    {
                        TempData["ErrMsg"] = "Error Try Again";
                        return View("Restructure1");
                    }
                    var LastRecord = LoanLedger.Distinct().Last();
                    if (LastRecord.orgCode == 2)
                    {
                        TempData["ErrMsg"] = "Loan Already Restructured";
                        TempData["code"] = "2";
                        ViewBag.LoansLedger = LoanLedger;
                        return View("Restructure1");
                    }
                    if (LastRecord.orgCode == 1)
                    {
                        TempData["ErrMsg"] = "Loan Repayment Complete";
                        TempData["code"] = "2";
                        ViewBag.LoansLedger = LoanLedger;
                        return View("Restructure1");
                    }
                    ViewBag.LoansLedger = LoanLedger;
                }
                return View("Restructure1");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult LoanRestructring2(FormCollection form, LoanApplication LoanApp, DataAccessLayerT.DataManager.AppLoan AppLoan)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    double Amount = Convert.ToDouble(form["NewAmount"]);
                    var ReferenceNum = LoanApp.IdentficationNumber;
                    var LoanLedger = _DR.GetLoanLedger(ReferenceNum);
                    if (LoanLedger == null)
                    {
                        TempData["ErrMsg"] = "Error Try Again";
                        return View("Restructure2");
                    }
                    var LastRecord = LoanLedger.Distinct().Last();
                    if (LastRecord.orgCode == 2)
                    {
                        TempData["ErrMsg"] = "Loan Already Restructured";
                        TempData["code"] = "2";
                        ViewBag.LoansLedger = LoanLedger;
                        return View("Restructure2");
                    }
                    if (LastRecord.orgCode == 1)
                    {
                        TempData["ErrMsg"] = "Loan Repayment Complete";
                        TempData["code"] = "2";
                        ViewBag.LoansLedger = LoanLedger;
                        return View("Restructure2");
                    }
                    ViewBag.LoansLedger = LoanLedger;
                }
                return View("Restructure2");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult Restructure1(FormCollection form, LoanApplication LoanApp, DataAccessLayerT.DataManager.AppLoan AppLoan)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                userid = dp.getUserID(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();
                    int RemainingTenure = Convert.ToInt16(form["LoanTenure"]);
                    double Amount = Convert.ToDouble(form["NewAmount"]);
                    var ReferenceNum = LoanApp.IdentficationNumber;
                    //var InstFK = _DR.getInstFK(ReferenceNum);
                    //var interesrate = _DR.getInterestRateByInst(InstFK,RemainingTenure);

                    var LoanLedger = _DR.GetLoanLedger(ReferenceNum);
                    if (LoanLedger == null || LoanLedger.Count == 0)
                    {
                        TempData["ErrMsg"] = "Invalid Reference Number";
                        return View();
                    }
                    var LastRecord = LoanLedger.Distinct().Last();
                    ViewBag.LoanAmount = LastRecord.LoanAmount;
                    ViewBag.LoanTenure = LastRecord.LoanTenure;
                    ViewBag.CreditSum = LoanLedger.Sum(x => x.Credit);
                    ViewBag.DebitSum = LoanLedger.Sum(x => x.Debit);
                    if (ViewBag.CreditSum == ViewBag.DebitSum)
                    {
                        TempData["ErrMsg"] = "Loan Already Closed";
                        return View();
                    }
                    ViewBag.Balance = ViewBag.DebitSum - ViewBag.CreditSum;
                    double NewAmount = (double)Amount;
                    if (Amount > ViewBag.Balance)
                    {
                        TempData["ErrMsg"] = "Invalid Repayment Amount";
                        return View();
                    }
                    double NewBalance = (double)(ViewBag.Balance - NewAmount);
                    ViewBag.AllCreditCount = LoanLedger.Select(x => x.Credit).Count();
                    ViewBag.CreditCount = LoanLedger.Where(q => q.Credit != 0).Select(x => x.Credit).Count();
                    // Old Tenure Claculation 
                    // int RemainingTenure = ViewBag.AllCreditCount - ViewBag.CreditCount;
                    // New Tenure Calculation

                    //var NewInterestRate = _DR.getInterestRate(RemainingTenure);
                    var NewInterestRate = _DR.getInterestRateByInstFk(RemainingTenure, LastRecord.ID);
                    if (NewInterestRate == 0)
                    {
                        TempData["ErrMsg"] = "Unable To Get Interest Rate";
                        return View();
                    }

                    var NewMonthlyRepayment = utilities.PayrollLoanCalc(NewBalance, RemainingTenure, NewInterestRate);
                    if (NewMonthlyRepayment == null || NewMonthlyRepayment == "")
                    {
                        TempData["ErrMsg"] = "Unable to Calculate Monthly Repayment";
                        return View();
                    }
                    double NewMonthlyPrincipal = (double)NewBalance / RemainingTenure;
                    double NewMonthlyInterest = (double)NewBalance * NewInterestRate;
                    AppStatFk = 6;
                    AppLoan = _DR.GetLoanApplication(ReferenceNum, AppStatFk);
                    // AppLoan.LoanAmount = NewAmount;
                    int id = 0;
                    AppLoan.LoanAmount = NewBalance;
                    AppLoan.LoanTenure = RemainingTenure;
                    AppLoan.interestRate = (float)NewInterestRate;
                    var refNum = InsertIntoLoanApplication(AppLoan, userid, out id);
                    var GuarId = InsertIntoGuarantor(AppLoan, userid, id);
                    AppLoan.LoanRefNumber = refNum;

                    var resp = LoansLedger(AppLoan);
                    if (resp != 0)
                    {
                        AppLoan.LoanAmount = NewAmount;
                        resp = updateLoanLedger(AppLoan, ReferenceNum);
                        TempData["Offer"] = "0";
                        TempData["ErrMsg"] = "Loan Restructuring Succesful";
                        UvlotApplication.Classes.TableObjects.LoanApplication Lp = new UvlotApplication.Classes.TableObjects.LoanApplication();
                        Lp.LoanRefNumber = refNum;

                        return View(Lp);
                    }
                    ViewBag.LoansLedger = LoanLedger;

                }

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult Restructure2(FormCollection form, LoanApplication LoanApp, DataAccessLayerT.DataManager.AppLoan AppLoan)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                userid = dp.getUserID(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();
                    int RemainingTenure = Convert.ToInt16(form["LoanTenure"]);
                    // double Amount = Convert.ToDouble(form["NewAmount"]);
                    var ReferenceNum = LoanApp.IdentficationNumber;
                    //var InstFK = _DR.getInstFK(ReferenceNum);
                    //var interesrate = _DR.getInterestRateByInst(InstFK,RemainingTenure);

                    var LoanLedger = _DR.GetLoanLedger(ReferenceNum);
                    if (LoanLedger == null || LoanLedger.Count == 0)
                    {
                        TempData["ErrMsg"] = "Invalid Reference Number";
                        return View();
                    }
                    var LastRecord = LoanLedger.Distinct().Last();
                    ViewBag.LoanAmount = LastRecord.LoanAmount;
                    ViewBag.LoanTenure = LastRecord.LoanTenure;
                    ViewBag.CreditSum = LoanLedger.Sum(x => x.Credit);
                    ViewBag.DebitSum = LoanLedger.Sum(x => x.Debit);
                    if (ViewBag.CreditSum == ViewBag.DebitSum)
                    {
                        TempData["ErrMsg"] = "Loan Already Closed";
                        return View();
                    }
                    ViewBag.Balance = ViewBag.DebitSum - ViewBag.CreditSum;
                    /* double NewAmount = (double)Amount;
                     if (Amount > ViewBag.Balance)
                     {
                         TempData["ErrMsg"] = "Invalid Repayment Amount";
                         return View();
                     }
                     double NewBalance = (double)(ViewBag.Balance - NewAmount);
                     */
                    double NewBalance = ViewBag.Balance;
                    ViewBag.AllCreditCount = LoanLedger.Select(x => x.Credit).Count();
                    ViewBag.CreditCount = LoanLedger.Where(q => q.Credit != 0).Select(x => x.Credit).Count();
                    // Old Tenure Claculation 
                    // int RemainingTenure = ViewBag.AllCreditCount - ViewBag.CreditCount;
                    // New Tenure Calculation

                    //var NewInterestRate = _DR.getInterestRate(RemainingTenure);
                    var NewInterestRate = _DR.getInterestRateByInstFk(RemainingTenure, LastRecord.ID);
                    if (NewInterestRate == 0)
                    {
                        TempData["ErrMsg"] = "Unable To Get Interest Rate";
                        return View();
                    }

                    var NewMonthlyRepayment = utilities.PayrollLoanCalc(NewBalance, RemainingTenure, NewInterestRate);
                    if (NewMonthlyRepayment == null || NewMonthlyRepayment == "")
                    {
                        TempData["ErrMsg"] = "Unable to Calculate Monthly Repayment";
                        return View();
                    }
                    double NewMonthlyPrincipal = (double)NewBalance / RemainingTenure;
                    double NewMonthlyInterest = (double)NewBalance * NewInterestRate;
                    AppStatFk = 6;
                    AppLoan = _DR.GetLoanApplication(ReferenceNum, AppStatFk);
                    //AppLoan.LoanAmount = NewAmount;
                    AppLoan.LoanAmount = NewBalance;
                    AppLoan.LoanTenure = RemainingTenure;
                    AppLoan.interestRate = (float)NewInterestRate;
                    int id = 0;
                    var refNum = InsertIntoLoanApplication(AppLoan, userid, out id);
                    var GuarId = InsertIntoGuarantor(AppLoan, userid, id);

                    AppLoan.LoanRefNumber = refNum;

                    var resp = LoansLedger(AppLoan);
                    if (resp != 0)
                    {
                        resp = updateLoanLedger(AppLoan, ReferenceNum);
                        TempData["Offer"] = "0";
                        TempData["ErrMsg"] = "Loan Restructuring Succesful";
                        UvlotApplication.Classes.TableObjects.LoanApplication Lp = new UvlotApplication.Classes.TableObjects.LoanApplication();
                        Lp.LoanRefNumber = refNum;

                        return View(Lp);
                    }
                    ViewBag.LoansLedger = LoanLedger;

                }

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult LoanApprovalUpdate()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string InsertIntoLoanApplication(DataAccessLayerT.DataManager.AppLoan lApObj, int user, out int ID)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                ID = 0;
                LoanApplication instObj = new DataAccessLayerT.DataManager.LoanApplication();
                instObj.Institution_FK = lApObj.institutionFk;
                instObj.MeansOfIDFilePath = lApObj.IdentficationNumberImage;
                instObj.AccomodationType_FK = lApObj.AccomodationType_FK;
                instObj.AccountName = lApObj.AccountName;
                instObj.AccountNumber = lApObj.AccountNumber;
                instObj.ApplicantID = lApObj.ApplicantID;
                instObj.ApplicationStatus_FK = 6;
                instObj.BankCode = Convert.ToString(lApObj.BankCode);
                instObj.BVN = lApObj.BVN;
                instObj.ClosestBusStop = lApObj.ClosestBusStop;
                instObj.ContactAddress = lApObj.ContactAddress;
                instObj.CreatedBy = Convert.ToString(user); //Change To User ID
                instObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                instObj.DateModified = MyUtility.getCurrentLocalDateTime();
                instObj.DateOfBirth = Convert.ToString(lApObj.DateOfBirth);
                instObj.EmailAddress = lApObj.EmailAddress;
                instObj.ExistingLoan = lApObj.ExistingLoan;
                instObj.ExistingLoan_NoOfMonthsLeft = lApObj.ExistingLoan_NoOfMonthsLeft;
                instObj.ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount;
                instObj.Firstname = lApObj.Firstname;
                instObj.Gender_FK = lApObj.Gender_FK;
                instObj.IdentficationNumber = lApObj.IdentficationNumber;
                instObj.Landmark = lApObj.Landmark;
                instObj.LGA_FK = lApObj.ID;
                instObj.StateofResidence_FK = lApObj.StateofResidence_FK;
                instObj.LoanAmount = Convert.ToDouble(lApObj.LoanAmount);
                instObj.LoanComment = "";
                instObj.LoanRefNumber = MyUtility.GenerateRefNo();
                instObj.LoanTenure = lApObj.LoanTenure;
                instObj.MaritalStatus_FK = lApObj.MaritalStatus_FK;
                instObj.MeansOfID_FK = lApObj.MeansOfID_FK;
                instObj.Organization = lApObj.Organization;
                instObj.Othernames = lApObj.Othernames;
                instObj.PhoneNumber = lApObj.PhoneNumber;
                instObj.RepaymentMethod_FK = lApObj.RepaymentMethod_FK;
                instObj.StateofResidence_FK = lApObj.StateofResidence_FK;
                instObj.Surname = lApObj.Surname;
                instObj.Title_FK = lApObj.Title_FK;
                instObj.IsVisible = 1;
                instObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                instObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                instObj.LoanProduct_FK = 2;
                instObj.NOK_EmailAddress = lApObj.NOK_EmailAddress;
                instObj.NOK_FullName = lApObj.NOK_FullName;
                instObj.NOK_HomeAddress = lApObj.NOK_HomeAddress;
                instObj.NOK_PhoneNumber = lApObj.NOK_PhoneNumber;
                instObj.NOK_Relationship = lApObj.NOK_Relationship;

                var resp = DataWriter.CreateLoanApplication(instObj);
                ID = resp.ID;
                return resp.LoanRefNumber;
            }
            catch (Exception ex)
            {
                ID = 0;
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public int InsertIntoGuarantor(DataAccessLayerT.DataManager.AppLoan lApObj, int user, int id)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                Guarantor GuaObj = new DataAccessLayerT.DataManager.Guarantor();
                GuaObj.LoanApplication_FK = id;
                GuaObj.Othernames = lApObj.GuarOthernames;
                GuaObj.PhoneNumber = lApObj.GuarPhone;
                GuaObj.Surname = lApObj.Surname;
                GuaObj.RelationShipWithApplicant = lApObj.GuarRelationship;
                GuaObj.EmailAddress = lApObj.GuarEmail;
                GuaObj.ContactAddress = lApObj.GuarContact;
                GuaObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");

                var resp = DataWriter.CreateGuarantor(GuaObj);
                return resp.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }




        [HttpGet]
        // public ActionResult EditUser(FormCollection form, int id)
        public ActionResult EditUser(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                int id = Convert.ToInt16(Request.QueryString["id"]);

                var users = _DR.getAllUser();
                if (id != 0)
                {
                    var currentuser = _DR.getUserProfile(id);
                    ViewBag.firstname = currentuser.Firstname;
                    ViewBag.lastname = currentuser.Lastname;
                    ViewBag.Email = currentuser.EmailAddress;
                    ViewBag.Phone = currentuser.PhoneNumber;
                    ViewBag.id = id;

                }
                ViewBag.Data = users;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public int LoansLedger(DataAccessLayerT.DataManager.AppLoan LoanApp)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                LoansController Ln = new LoansController();
                var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
                var salaryAmount = Convert.ToDouble(LoanApp.Salary);
                InstCode = LoanApp.InstitutionCode;
                var LoanLegder = Ln.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage, InstCode);
                int RemainingDays = 0;

                if (respMessage == "0")
                {

                    double InterestValue = 0;
                    double PoratedAmt = 0;
                    LoansLedger Ledger = new LoansLedger();
                    int i;
                    // var CurrentDate = MyUtility.getCurrentLocalDateTime();
                    var CurrentDate = LoanApp.DisburseDate;
                    var Dayval = CurrentDate.Day;
                    int lastday = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);
                   
                     RemainingDays = lastday - Dayval;

                    // for Last Days Of A Month 

                    DateTime today = MyUtility.getCurrentLocalDateTime();
                    DateTime endOfMonth = new DateTime(today.Year,
                                                       today.Month,
                          DateTime.DaysInMonth(today.Year, today.Month));


                    if (lastday == 31 && RemainingDays <= 16)
                    {
                        var InterestRate = _DR.getInterestRate(LoanApp.LoanTenure);
                        if (RemainingDays != 0)
                        {
                            InterestRate = InterestRate / 100;
                            InterestValue = InterestRate * LoanAmount;
                            PoratedAmt = (double)RemainingDays / lastday;
                            PoratedAmt = (double)PoratedAmt * InterestValue;
                            PoratedAmt = Math.Round(PoratedAmt, 3);
                        }

                        for (i = 1; i <= LoanApp.LoanTenure; i++)
                        {
                            Ledger.ApplicantID = LoanApp.ApplicantID;
                            Ledger.RefNumber = LoanApp.LoanRefNumber;
                            Ledger.IsVisible = 1;
                            Ledger.trnDate = endOfMonth.AddMonths(i - 1);
                            Ledger.Credit = 0;
                            if (i == 1)
                            {
                                Ledger.Debit = repayment + PoratedAmt;
                            }
                            if (i > 1)
                            {
                                Ledger.Debit = repayment;
                            }
                            Ledger.Institution_FK = LoanApp.institutionFk;
                            Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            _DM.InsertLoansLedger(Ledger);

                        }
                         return LoanApp.ID;
                    }

                    if (lastday == 31 && RemainingDays > 16)
                    {
                        PoratedAmt = 0;
                        for (i = 1; i <= LoanApp.LoanTenure; i++)
                        {
                            Ledger.ApplicantID = LoanApp.ApplicantID;
                            Ledger.RefNumber = LoanApp.LoanRefNumber;
                            Ledger.IsVisible = 1;
                            Ledger.trnDate = endOfMonth.AddMonths(i - 1);
                            Ledger.Credit = 0;
                            Ledger.Debit = repayment;
                            Ledger.Institution_FK = LoanApp.institutionFk;
                            Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            _DM.InsertLoansLedger(Ledger);

                        }
                        return LoanApp.ID;
                    }

                    // Its Ends Here
                    // if (RemainingDays > 10)
                    if (lastday == 30 && RemainingDays > 15)
                    {
                        PoratedAmt = 0;
                        for (i = 1; i <= LoanApp.LoanTenure; i++)
                        {
                            Ledger.ApplicantID = LoanApp.ApplicantID;
                            Ledger.RefNumber = LoanApp.LoanRefNumber;
                            Ledger.IsVisible = 1;
                            Ledger.trnDate = endOfMonth.AddMonths(i-1);
                            Ledger.Credit = 0;
                            Ledger.Debit = repayment;
                            Ledger.Institution_FK = LoanApp.institutionFk;
                            Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            _DM.InsertLoansLedger(Ledger);

                        }
                        return LoanApp.ID;
                    }
                   
                    // if (RemainingDays <= 10)
                    if (lastday == 30 && RemainingDays <= 15)
                    {
                        var InterestRate = _DR.getInterestRate(LoanApp.LoanTenure);
                        if (RemainingDays != 0)
                        {
                            InterestRate = InterestRate / 100;
                            InterestValue = InterestRate * LoanAmount;
                            PoratedAmt = (double)RemainingDays / lastday;
                            PoratedAmt = (double)PoratedAmt * InterestValue;
                            PoratedAmt = Math.Round(PoratedAmt, 3);
                        }

                        for (i = 1; i <= LoanApp.LoanTenure; i++)
                        {
                            Ledger.ApplicantID = LoanApp.ApplicantID;
                            Ledger.RefNumber = LoanApp.LoanRefNumber;
                            Ledger.IsVisible = 1;
                            Ledger.trnDate = endOfMonth.AddMonths(i-1);
                            Ledger.Credit = 0;
                            if (i == 1)
                            {
                                Ledger.Debit = repayment + PoratedAmt;
                            }
                            if (i > 1)
                            {
                                Ledger.Debit = repayment;
                            }
                            Ledger.Institution_FK = LoanApp.institutionFk;
                            Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            _DM.InsertLoansLedger(Ledger);

                        }
                        return LoanApp.ID;
                    }
                    
                    return LoanApp.ID;

                }




                return LoanApp.ID;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public int updateLoanLedger(DataAccessLayerT.DataManager.AppLoan LoanApp, string RefNumber)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                LoansController Ln = new LoansController();
                var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
                var salaryAmount = Convert.ToDouble(LoanApp.Salary);
                InstCode = LoanApp.InstitutionCode;
                var LoanLegder = Ln.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage, InstCode);


                if (respMessage == "0")
                {

                    double InterestValue = 0;
                    double PoratedAmt = 0;
                    LoanLedger Ledger = new LoanLedger();
                    int i;
                    var CurrentDate = MyUtility.getCurrentLocalDateTime();
                    var Dayval = CurrentDate.Day;
                    int lastday = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);
                    int RemainingDays = lastday - Dayval;

                    // Update The Loan To Be Closed

                    var resp = _DM.UpdateLoanLedger(RefNumber, LoanAmount);


                    //Udate Ends Here 
                    // for Last Days Of A Month 

                    DateTime today = MyUtility.getCurrentLocalDateTime();
                    DateTime endOfMonth = new DateTime(today.Year,
                                                       today.Month,
                          DateTime.DaysInMonth(today.Year, today.Month));

                    // Its Ends Here
                    if (RemainingDays > 10)
                    {
                        PoratedAmt = 0;
                    }
                    if (RemainingDays <= 10)

                    {
                        var InterestRate = _DR.getInterestRate(LoanApp.LoanTenure);
                        if (RemainingDays != 0)
                        {
                            InterestRate = InterestRate / 100;
                            InterestValue = InterestRate * LoanAmount;
                            PoratedAmt = (double)RemainingDays / lastday;
                            PoratedAmt = (double)PoratedAmt * InterestValue;
                            PoratedAmt = Math.Round(PoratedAmt, 3);
                        }
                    }
                    for (i = 1; i <= LoanApp.LoanTenure; i++)
                    {
                        Ledger.ApplicantID = LoanApp.ApplicantID;
                        Ledger.RefNumber = LoanApp.LoanRefNumber;
                        Ledger.IsVisible = 1;
                        Ledger.TranxDate = endOfMonth.AddMonths(i);
                        Ledger.Credit = 0;
                        if (i == 1)
                        {
                            Ledger.Debit = repayment + PoratedAmt;
                        }
                        if (i > 1)
                        {
                            Ledger.Debit = repayment;
                        }
                        Ledger.Institution_FK = LoanApp.institutionFk;
                        Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
                        Ledger.PaymentFlag_FK = 0;
                        _DM.InsertLoansLedger(Ledger);

                    }
                    return LoanApp.ID;

                }
                return LoanApp.ID;




            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }



        [HttpGet]
        public ActionResult Applications()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                AppStatFk = 1;
                userid = _DR.getUserID(user);
                ViewBag.Data = _DR.GetApplications();
                Session["AllTransaction"] = ViewBag.Data;

               
                //int instFk = Convert.ToInt16(User.PinNumber);
                //ViewBag.Data = _DR.GetAppStat(instFk);
                //Session["AllTransaction"] = ViewBag.Data;

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult InterestRate(int Tenure)
        {
            try
            {
                DataReader dr = new DataReader();

                var LoanTenure = dr.GetInterestRate(Tenure);

                var InterestRate = Convert.ToDouble(LoanTenure.InterestRate);

                return Json(new { InterestRate });
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }
        

             [HttpGet]
        public ActionResult KpiReports()
        {
            try
            {
                clearError();

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                AppStatFk = 6;
                userid = _DR.getUserID(user);
                var MyApplication  = _DR.GetDIsbursedApplcation(AppStatFk);
            
               
                ViewBag.Data = MyApplication;
                Session["AllTransaction"] = ViewBag.Data;

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

      

        public List<double> GetKpiInMinutes(int ID)
        {
            try
            {

                List<double> KpiMinutes = new List<double>();
                DateTime TD = DateTime.Now;
                double TimeDiffx = 0;
                double kpi = 0;
                //var Rec = (from a in db.NyscLoanApplications
                //           join b in db.NYSCLoanApprovals on a.ID equals b.LoanApplication_FK
                //           where a.ID == 158
                //           select new AppLoanss
                //           {
                //               LoanRefNumber = a.RefNumber,
                //               APplicationDate = (DateTime)a.DateCreated,
                //               ApplicationApprove = (DateTime)b.DateCreated

                //           }

                //           ).ToList();
                var Rec = _DR.GetKpi(ID);

                if (Rec.Count > 0)
                {
                    var Count = Rec.Count;
                    var AppDate = Rec.Select(x => x.ApplicationDate).FirstOrDefault();
                    for (var i = 0; i <= Rec.Count; i++)
                    {
                        var TimeDiff = Rec.Select(x => x.ApplicationApprove).ElementAt(i);
                        if (i + 1 == Count)
                        {
                            if (KpiMinutes.Count < 4)
                            {
                                kpi = 0;
                                KpiMinutes.Add(kpi);
                            }
                            ViewBag.Data = KpiMinutes;
                            return KpiMinutes;
                        }
                        else
                        {
                            if (i == 0)
                            {
                                TD = Rec.Select(x => x.ApplicationApprove).FirstOrDefault();
                                TimeDiffx = TD.Subtract(AppDate).TotalMinutes;
                                kpi = Convert.ToInt16(TimeDiffx);
                               // kpi = ConvertTime(kpi);
                                KpiMinutes.Add(kpi);
                            }
                            TD = Rec.Select(x => x.ApplicationApprove).ElementAt(i + 1);
                            TimeDiffx = TD.Subtract(TimeDiff).TotalMinutes;
                            kpi = Convert.ToInt16(TimeDiffx);
                           // kpi = ConvertTime(kpi);
                            KpiMinutes.Add(kpi);

                        }

                    }
                }
                ViewBag.Data = KpiMinutes;
                return KpiMinutes;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public double ConvertTime(double Mins)
        {
            try
            {
                TimeSpan spWorkMin = TimeSpan.FromMinutes(12534);
                string workHours = spWorkMin.ToString(@"hh\:mm");
                return spWorkMin.TotalHours;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        [HttpGet]
        public ActionResult KpiDetails(int Refid)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
               // var RefNum = _DR.GetRefNum(Refid);
                string RefNum = "15752941567981";
               if(Refid == null || Refid == 0)
                {

                }
                if(RefNum == null || RefNum == "")
                {
                    TempData["KpiLoanAppToRemitaGen"] = "N/A";
                }
                else
                {
                    // To Test Individual Borrower Report
                    CBIndividualBorower();
                    // Time from Remita Issuance to Remita Activation
                    var RemitaGenToActivation = GetRemitaGenToActivation(RefNum);
                    TempData["RemitaIssuanceToActivation"] = RemitaGenToActivation.Before("+");
                    TempData["RemitaActivationToDisbursement"] = RemitaGenToActivation.After("+");
                    // Ends
                    var KpiLoanAppToRemitaGen = _DR.KpiLoanAppToRemitaGeneration(RefNum);
                    if (KpiLoanAppToRemitaGen == null)
                    {
                        TempData["KpiLoanAppToRemitaGen"] = "N/A";
                    }
                    else
                    {
                        TempData["KpiLoanAppToRemitaGen"] = KpiLoanAppToRemitaGen;
                    }
                   
                }
                if (Refid != 0)
                {
                    var kpi = GetKpiInMinutes(Refid);
                    DataAccessLayerT.DataManager.AppLoan appLoan = new DataAccessLayerT.DataManager.AppLoan();
                    var KpiComment = _DR.GetKpi(Refid);
                    List<DataAccessLayerT.DataManager.AppLoan> appx = new List<DataAccessLayerT.DataManager.AppLoan>();
                    appx = KpiComment;
                    ViewBag.KpiComment = KpiComment; 
                    ViewBag.Data = kpi;
                    if(kpi == null)
                    {
                        TempData["KpiLoanAppToRemitaGen"] = "N/A";
                        TempData["TotalTime"] = "N/A";
                        TempData["AvgLoanProcessTime"] = "N/A";
                        return View();
                    }
                    if (kpi.Count != 0)
                    {
                        var CT = kpi.Sum(x => Convert.ToInt64(x));
                        var TimeCOnert = Convert.ToInt16(CT);
                        TempData["TotalTime"] = timeConvert(TimeCOnert);
                   
                    var TotalLoan = _DR.getCountOFLoans();
                    TempData["AvgLoanProcessTime"] = CT / TotalLoan;
                     
                    var AVrproTime = TempData["AvgLoanProcessTime"];
                    // AVrproTime = Convert.ToInt32(AVrproTime);
                    AVrproTime = timeConvert(Convert.ToInt32(AVrproTime));
                    TempData["AvgLoanProcessTime"] = AVrproTime;

                   
                   

                    }
                }
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

       
        public string GetRemitaGenToActivation(string RefNum)
        {
            try
            {
                dynamic jobj = new JObject();
                string diffDate = null; string DiffDate1 = "N/A";
                var LoanApp = _DR.GetLP(RefNum);
                var rec = _DR.getMandate(RefNum);
                if (rec.ID != 0 && rec != null)
                {
                  
                    jobj.mandateId = "160386148950";//rec.mandateID;//"160386148950";//Convert.ToString(rec.MandateId);
                    jobj.requestId = "1583765067792";//rec.PatnerReference;//"1583765067792";//Convert.ToString(rec.requestId);

                    var json = jobj.ToString();
                   
                    var url = ConfigurationManager.AppSettings["MandateStatusLive"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    if (data == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return null;
                    }
                    dynamic jObj = JObject.Parse(data);

                    //if (jObj.respCode != null || jObj.respCode != "" || jObj.respCode != "00")
                        if(jObj.respCode == "00")
                    {
                        diffDate = jObj.RegistrationDate.Value.Subtract(rec.DateCreated.Value);
                        var DiffDate = LoanAp.DateModified.Value.Subtract(rec.DateCreated.Value);
                        DiffDate1 = DiffDate.ToString();
                       if(diffDate == null)
                        {
                            diffDate = "N/A";
                        }
                        if (DiffDate1 == null)
                        {
                            DiffDate1 = "N/A";
                        }
                    }
                }
                
               
                
                return (diffDate.ToString() + "+" + DiffDate1.ToString());
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public String timeConvert(int time)
        {
            try
            {

                var Days = time / 24 / 60;
                var hours = time / 60 % 24;
                var Secs = time % 60;
                if (Days == 0)
                {
                    Days = 0;
                }
                if (hours == 0)
                {
                    hours = 0;
                }
                if (Secs == 0)
                {
                    Secs = 0;
                }
                return Days + " Days  " + hours + " Hours  " + Secs + " Seconds  ";
                // return time / 24 / 60 + ":" + time / 60 % 24 + ':' + time % 60;

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public ActionResult CBIndividualBorower()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                AppStatFk = 1;
                userid = _DR.getUserID(user);
                ViewBag.Data = _DR.GetIndividualBorrower();
                Session["AllTransaction"] = ViewBag.Data;

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public ActionResult MonthlyRepayment()
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult MonthlyRepayment(FormCollection form)
        {
            try
            {

                var Month = Convert.ToDateTime(form["mnth"]);
               
                 if(Month != null || Month != null) {

                    MonthlyRepayments(Month);
                }
             
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


       
        public string MonthlyRepayments(DateTime Date)
        {
            try
            {

                var TDate = Date;
                //int monthID = 0;
                //int.TryParse(Month, out monthID);
                var LoansDue = _DR.GetDuePaymentsByMonth(TDate);
                Classes.TableObjects.Remitaresp mObj = new TableObjects.Remitaresp();

                if (LoansDue != null)
                {
                    if (LoansDue.Count > 0)
                    {
                        for (var w = 0; w <= LoansDue.Count; w++)
                        {
                            double debitAmount = 0;
                            double.TryParse(LoansDue[w].Debit.ToString(), out debitAmount);

                            string mandateID = LoansDue[w].Company;
                            string bankCode = LoansDue[w].Location;
                            string accountNo = LoansDue[w].EmployeeID;

                            dynamic obj = new JObject();
                            obj.totalAmount = debitAmount.ToString();
                            obj.refNumber = LoansDue[w].ReferenceNum.ToString();
                            obj.mandateId = mandateID;
                            obj.fundingBankCode = bankCode;
                            obj.fundingAccount = accountNo;

                            string json = obj.ToString();
                            string RemitaDebitInstruction = ConfigurationManager.AppSettings["DirectDebitLive"];
                            var data = Helper.DoPost(RemitaDebitInstruction, "", "", "", "", "", "", json);

                            // Classes.TableObjects.Remitaresp mObj = new TableObjects.Remitaresp();
                            MonthlyRepayment _MR = new MonthlyRepayment();
                            mObj = new JavaScriptSerializer().Deserialize<TableObjects.Remitaresp>(data);
                          
                            if (mObj.respCode != null || mObj.respCode != "")
                            {
                                  
                                 InsertMonthlyRepaymentLog(mObj,_MR);
                            }
                        }
                    }


                }
                return mObj.respCode;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public void InsertMonthlyRepaymentLog(TableObjects.Remitaresp remitaresp, MonthlyRepayment _MR)
        {
            try
            {
                _MR.Amount = remitaresp.Amount;
                _MR.FundingAccount = remitaresp.fundingAccount;
                _MR.FundingBankCode = remitaresp.fundingBankCode;
                _MR.MandateID = remitaresp.mandateID;
                _MR.RefNumber = remitaresp.refNumber;
                _MR.RequestID = remitaresp.requestID;
                _MR.ResponseCode = remitaresp.respCode;
                _MR.RespDescription = remitaresp.respDescription;
                _MR.TransactDate = remitaresp.TransactDate;
                _MR.TransactionRef = remitaresp.transactionRef;
                _MR.RRR = remitaresp.RRR;

                _DM.InsertMonthlyRepaymentLog(_MR);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());

            }
        }

        public string MonthlyRepaymentStatus(DateTime Date)
        {
            try
            {

                var TDate = Date;
                var LoansDue = _DR.GetDuePaymentsByMonth(TDate);
                Classes.TableObjects.Remitaresp mObj = new TableObjects.Remitaresp();

                if (LoansDue != null)
                {
                    if (LoansDue.Count > 0)
                    {
                        for (var w = 0; w <= LoansDue.Count; w++)
                        {
                            dynamic obj = new JObject();
                            obj.mandateId = LoansDue[w].MandateId;//Convert.ToString(rec.MandateId);
                            obj.requestId = LoansDue[w].requestId; //Convert.ToString(rec.requestId);
                            
                           
                           
                            var json = obj.ToString();
                            //var url = ConfigurationManager.AppSettings["DebitStatusTest"];
                            var url = ConfigurationManager.AppSettings["DebitStatusLive"];

                            string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                            if (data == null)
                            {
                                TempData["ErrMsg"] = "Please Try Again";
                                continue;
                            }
                            dynamic jObj = JObject.Parse(data);

                            if (jObj.respCode != "" || jObj.respCode != null)
                            {
                                if (jObj.respCode != "00")
                                {
                                   // updateRepaymentLoansLedger();
                                }
                                //TempData["SucMsg"] = jObj.respDescription;
                                //TempData["value1"] = jObj.amount;
                                //TempData["value2"] = jObj.RRR;
                                //TempData["status"] = jObj.description;
                                //TempData["value5"] = jObj.respDescription;
                            }
                           
                          
                        }
                    }


                }
                return mObj.respCode;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult MonthlyRepaymentStatus(FormCollection form)
        {
            try
            {

              

                var Month = Convert.ToDateTime(form["mnth"]);

                if (Month != null || Month != null)
                {

                    MonthlyRepaymentStatus(Month);
                }


                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        [HttpGet]
        public ActionResult MyApplications()
        {
            try
            {
                clearError();

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                AppStatFk = 1;
                userid = _DR.getUserID(user);
                //var MyApplication  = _DR.GetMyApplications(Convert.ToString(userid));
                var MyApplication = _DR.GetMyApplicationsWithComment(Convert.ToString(userid));
              //List<int> MyApp = MyApplication.Select(v => v.ID).ToList();
                ViewBag.Data = MyApplication;
               // var LoanComment = _DR.GetAllLoanComments(MyApp);
                Session["AllTransaction"] = ViewBag.Data;

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public void clearError()
        {
            if (TempData["SucMsg"] == null)
            {
                TempData["SucMsg"] = "";
            }
            if (TempData["ErrMsg"] == null)
            {
                TempData["ErrMsg"] = "";
            }
            if (TempData["Error"] == null)
            {
                TempData["Error"] = "";
            }
        }

        [HttpGet]
        public ActionResult ApplicationsStat()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
               // var rec = _DR.getUser(user);
               
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                AppStatFk = 1;
                int instFk = Convert.ToInt16(User.PinNumber);
                ViewBag.Data = _DR.GetAppStat(instFk);
               Session["AllTransaction"] = ViewBag.Data;

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult DocStat()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                // var rec = _DR.getUser(user);

                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                AppStatFk = 1;
                int instFk = Convert.ToInt16(User.PinNumber);
                // ViewBag.Data = _DR.GetAppStat(AppStatFk);
                ViewBag.Data = _DR.GetDocAppStat(AppStatFk);
                Session["AllTransaction"] = ViewBag.Data;

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        [HttpGet]
        public ActionResult MyApplicationsStatus()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                AppStatFk = 1;
                userid = _DR.getUserID(user);
                ViewBag.Data = _DR.GetMyApplicationsStatus(Convert.ToString(userid));
     
                // ViewBag.Data = MyAPplicationStat;
                Session["AllTransaction"] = ViewBag.Data;

                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetApplication(string Refid)
        {
            try
            {
                //TempData["ErrMsg"] = "";
                if (TempData["SucMsg"] == null)
                {
                    TempData["SucMsg"] = "";
                }
                if (TempData["ErrMsg"] == null)
                {
                    TempData["ErrMsg"] = "";
                }
                if (TempData["Error"] == null)
                {
                    TempData["Error"] = "";
                }
                

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                // var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                LoansController Ln = new LoansController();
                DataAccessLayerT.Classes.LoanApplication LoanApps = new DataAccessLayerT.Classes.LoanApplication();
                LoanApps = _DR.GetLoanApplicationByRefNum(Refid);
                var InstCode = _DR.getInstitutionById(LoanApps.institutionFk);
                LoanApps.instCode = InstCode.InstitutionCode;
                //LoanApps.MeanOFIDFIlePath = Ln.saveImage(LoanApps.MeanOFIDFIlePath);
                GetMenus();
                LoansController ln = new LoansController();
                ViewBag.nBanks = dp.GetBanks();
                ViewBag.nRepmtMethods = dp.GetRepaymentMethods();
                ViewBag.nStates = dp.GetNigerianStates();
                ViewBag.nMeansOfIDs = dp.GetMeansOfIdentifications();
                ViewBag.nAccomodationTypes = dp.GetAccomodationTypes();
                ViewBag.nLGAs = dp.GetAllLGAs();
                ViewBag.nTitles = dp.GetTitles();
                ViewBag.nMarital = dp.GetMaritalStatus();
                int val = 0;
                ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", LoanApps.Title_FK);
                ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", LoanApps.MaritalStatus_FK);
                // ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", LoanApps.LGAs);

                ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", LoanApps.AccomodationType_FK);
                ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", LoanApps.MeansOfID_FK);
                ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", LoanApps.StateofResidence_FK);
                ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", LoanApps.BankCode);
                ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", LoanApps.RepaymentMethod_FK);
                ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", LoanApps.LoanTenure);
                ViewData["nGender"] = new SelectList(ln.GetAllGender(), "Value", "Text", LoanApps.Gender_FK);
                ViewData["nemploymentStatus"] = new SelectList(ln.GetAppStatus(), "Value", "Text", LoanApps.EmployeeStatus);
                return View(LoanApps);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        //[HttpPost]
        //public ActionResult UpdateApplication()
        //{
        //    try
        //    {
        //        return View();
        //    }
        //    catch(Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        [HttpGet]
        public ActionResult EditInstitution(FormCollection form, int id)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                //id = Convert.ToInt16(Request.QueryString["id"]);
                ViewBag.listInstitution = _DR.getAllInstitutionType();
                var users = _DR.getAllInstitution();
                if (id != 0)
                {
                    var currentuser = _DR.getInstitutionProfile(id);
                    ViewBag.Name = currentuser.Name;
                    ViewBag.InstCode = currentuser.InstitutionCode;
                    ViewBag.InstAdd = currentuser.InstitutionAddress;
                    ViewBag.InstEmail = currentuser.InstitutionEmailAddress;
                    ViewBag.InstContPhone = currentuser.ContactPhoneNo;
                    ViewBag.InstPhone = currentuser.InstitutionPhoneNo;
                    ViewBag.InstContEmail = currentuser.ContactEmailAddress;
                    ViewBag.InstHeadOfInst = currentuser.HeadOfInstition;
                    ViewBag.id = id;

                }
                ViewBag.Data = users;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult CreatePayrollLoan(FormCollection form, TableObjects.LoanApplication lApObj)
        {
            //form["institutionName"]
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                int userid = LoggedInuser.LoggedInUserID(user);
                var Marital = _DR.GetMaritalStatus(Convert.ToString(form["Marital"]));
                var LGA = _DR.GetLocalGovt(Convert.ToString(form["lgaList"]));
                var MeansOfID_FK = Convert.ToInt32(form["meansOfID"]);
                var Gender_FK = Convert.ToInt32(form["selectGender"]);
                var AccomodationType_FK = Convert.ToInt32(form["AccomodationTypes"]);

                var DtDisbursed = Convert.ToString(form["DateDisbursed"]);

                var DtCreated = Convert.ToString(form["DateCreated"]);
                string DOB = lApObj.DateOfBirth.ToString();
                DataAccessLayerT.DataManager.LoanApplication inst = new DataAccessLayerT.DataManager.LoanApplication
                {
                    AccomodationType_FK = Convert.ToInt32(form["AccomodationTypes"]), //Pending status
                    AccountName = lApObj.AccountName,
                    AccountNumber = lApObj.AccountNumber,
                    ApplicantID = lApObj.ApplicantID,
                    ApplicationStatus_FK = DtDisbursed.Length == 0 ? 3 : 6,  //3, //Pending status, Gender = a.Gender_FK == 1 ? "Male" : "Female",
                    BankCode = Convert.ToString(form["Bank"]),
                    BVN = lApObj.BVN,
                    ClosestBusStop = lApObj.ClosestBusStop,
                    ContactAddress = lApObj.ContactAddress,
                    CreatedBy = userid.ToString(), //Change To User ID
                    //DateCreated = MyUtility.getCurrentLocalDateTime(),
                    //DateModified = MyUtility.getCurrentLocalDateTime(),
                    DateCreated = Convert.ToDateTime(DtCreated),
                    DateModified = Convert.ToDateTime(DtDisbursed),
                    DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
                    EmailAddress = lApObj.EmailAddress,
                    ExistingLoan = lApObj.ExistingLoan,
                    ExistingLoan_NoOfMonthsLeft = lApObj.ExistingLoan_NoOfMonthsLeft,
                    ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
                    Firstname = lApObj.Firstname,
                    Gender_FK = Convert.ToInt32(form["selectGender"]),
                    IdentficationNumber = lApObj.IdentficationNumber,
                    Landmark = lApObj.Landmark,
                    LGA_FK = Convert.ToInt16(form["lgaList"]),
                    LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
                    LoanComment = "",
                    LoanRefNumber = MyUtility.GenerateRefNo(),
                    LoanTenure = lApObj.LoanTenure,
                    MaritalStatus_FK = Convert.ToInt16(form["Marital"]),
                    MeansOfID_FK = Convert.ToInt32(form["meansOfID"]),
                    NOK_EmailAddress = lApObj.NOK_EmailAddress,
                    NOK_FullName = lApObj.NOK_FullName,
                    NOK_HomeAddress = lApObj.NOK_HomeAddress,
                    NOK_PhoneNumber = lApObj.NOK_PhoneNumber,
                    NOK_Relationship = lApObj.NOK_Relationship,
                    Organization = lApObj.Organization,
                    Othernames = lApObj.Othernames,
                    PhoneNumber = lApObj.PhoneNumber,
                    RepaymentMethod_FK = Convert.ToInt32(form["RepmtMethod"]),
                    StateofResidence_FK = Convert.ToInt32(form["States"]),
                    Surname = lApObj.Surname,
                    Title_FK = Convert.ToInt32(form["Titles"]),
                    IsVisible = 1,
                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
                    LoanProduct_FK = 2

                };
                //string mario = form["institutionName"];
                DataWriter.CreateLoanApplication(inst);

                if (inst.ID > 0)
                {
                    LoansController _Lc = new LoansController();
                    _Lc.CreateGuarantor(lApObj, inst);
                    EmployerLoanDetail empObj = new EmployerLoanDetail();
                    empObj.ClosestBusStop = "";
                    empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                    empObj.DateModified = MyUtility.getCurrentLocalDateTime();
                    empObj.Department = Convert.ToString(form["department"]);
                    empObj.Designation = Convert.ToString(form["designation"]);
                    empObj.EmployerID = inst.ApplicantID;
                    empObj.EmploymentStatus_FK = Convert.ToInt32(form["employment_status"]);
                    empObj.IsVisible = 1;
                    empObj.LandMark = "";
                    var loss = form["Los"].ToString();
                    int los = MyUtility.isNumeric(loss) == true ? Convert.ToInt16(loss) : 0;
                    empObj.LengthOfServiceInMth = los;
                    empObj.LGA_FK = "0";
                    string Amount = form["netMonthlyIncome"].ToString();
                    empObj.NetMonthlyIncome = MyUtility.isFloat(Amount) == true ? Convert.ToDouble(Amount) : 0;
                    empObj.Occupation = Convert.ToString(form["occupation"]);
                    empObj.OfficialEmailAddress = Convert.ToString(form["officeEmail"]);
                    empObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                    empObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                    empObj.LoanApplication_FK = inst.ID;

                    DataWriter.CreateEmployerLoanDetails(empObj);
                    return View("Acknowledgement");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            return View("Acknowledgement");
        }

        [HttpPost]
        public ActionResult UpdateUser(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                var userx = _DR.getAllUser();
                User users = new User();
                users.Firstname = Convert.ToString(form["fname"]);
                users.Lastname = Convert.ToString(form["lname"]);
                users.PhoneNumber = Convert.ToString(form["Phone"]);
                users.EmailAddress = Convert.ToString(form["Email"]).Trim();
                users.ID = Convert.ToInt16(form["id"]);
                var id = _DR.UpdateProfiles(users);
                if (id != null)
                {
                    TempData["ErrMsg"] = "User Updated Succesfully";
                    return RedirectToAction("CreateUser");
                }
                if (id == null)
                {
                    TempData["ErrMsg"] = "Error Updating Data ! Please Try Again";
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult UpdateInstitution(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                var userx = _DR.getAllInstitution();
                Institution inst = new Institution();
                inst.Name = Convert.ToString(form["Name"]);
                inst.InstitutionType_FK = Convert.ToInt16(form["ID"]);
                inst.ContactEmailAddress = Convert.ToString(form["InstContEmail"]);
                inst.ContactPhoneNo = Convert.ToString(form["InstContPhone"]).Trim();
                inst.DateCreated = MyUtility.getCurrentLocalDateTime();
                inst.HeadOfInstition = Convert.ToString(form["InstHeadOfInst"]);
                inst.InstitutionPhoneNo = Convert.ToString(form["InstPhone"]);
                inst.InstitutionEmailAddress = Convert.ToString(form["InstEmail"]);
                inst.InstitutionCode = Convert.ToString(form["InstCode"]);
                inst.InstitutionAddress = Convert.ToString(form["InstAdd"]);
                inst.ID = Convert.ToInt16(form["ids"]);
                var id = _DR.UpdateInstitution(inst);
                if (id != null)
                {
                    TempData["ErrMsg"] = "Institution Updated Succesfully";
                    return RedirectToAction("CreateInstitution");
                }
                if (id == null)
                {
                    TempData["ErrMsg"] = "Error Updating Data ! Please Try Again";
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult InterestRateSetup()
        {
            try
            {
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                ViewBag.listInstitution = _DR.getAllInstitution();
                ViewBag.listInterestRate = _DR.GetAllTenure();
                ViewBag.Data = _DR.getAllLoanRate();
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult InterestRateSetup(FormCollection form, LoanInterestRate LIR)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                //LoanInterestRate LIR = new LoanInterestRate();
                var response = _DR.Validate(LIR);
                if (response == false)
                {

                    var res = _DM.CreateLoanInterestrate(LIR);
                    TempData["SucMsg"] = "Record Added";
                }
                else if (response == true)
                {
                    TempData["ErrMsg"] = "Record Already Exist";
                }
                ViewBag.listInstitution = _DR.getAllInstitution();
                ViewBag.listInterestRate = _DR.GetAllTenure();
                ViewBag.Data = _DR.getAllLoanRate();
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult AllUser()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult AllUser(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult CreateUser()
        {


            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                var users = _DR.getAllUser();

                ViewBag.Data = users;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult CreatePage(Classes.Page page, FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                DataAccessLayerT.DataManager.Page pages = new DataAccessLayerT.DataManager.Page();
                if (page != null)
                {
                    bool valid = _DR.ValidatePage(page.PageName);
                    if (valid == true)
                    {
                        TempData["ErrMsg"] = "Page Already Exist";

                        return RedirectToAction("CreatePage");
                    }
                    else if (valid == false)
                    {
                        pages.PageName = page.PageName;
                        pages.PageDescription = page.PageDescription;
                        pages.PageUrl = page.PageUrl;
                        int PageHeader = Convert.ToInt16(form["ids"]);
                        pages.PageHeader = PageHeader;
                        //pages.ValueDate = DateTime.Now("yyyy/mm/dd");
                        pages.IsVisible = 1;

                        int id = _DM.CreatePages(pages);
                        TempData["SucMsg"] = "Page Created !";
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Pages Not Succesful Please Try Again!";
                    }
                }
                return Redirect("CreatePage");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult LoanApplication()
        {
            try {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var rec = _DR.getUser(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                /*   ViewBag.nBanks = dp.GetBanks();
                ViewBag.nRepmtMethods = dp.GetRepaymentMethods();
                ViewBag.nStates = dp.GetNigerianStates();
                ViewBag.nMeansOfIDs = dp.GetMeansOfIdentifications();
                ViewBag.nAccomodationTypes = dp.GetAccomodationTypes();
                ViewBag.nLGAs = dp.GetAllLGAs();
                ViewBag.nTitles = dp.GetTitles();
                ViewBag.nmaritalStatus = dp.GetMaritalStatus();*/
                LoansController Lc = new LoansController();
                ViewBag.nBanks = dp.GetBanks();
                ViewBag.nRepmtMethods = dp.GetRepaymentMethods();
                ViewBag.nStates = dp.GetNigerianStates();
                ViewBag.nMeansOfIDs = dp.GetMeansOfIdentifications();
                ViewBag.nAccomodationTypes = dp.GetAccomodationTypes();
                ViewBag.nLGAs = dp.GetAllLGAs();
                ViewBag.nTitles = dp.GetTitles();
                ViewBag.nMarital = dp.GetMaritalStatus();
                int val = 0;
                ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", val);
                ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", val);
                ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", val);

                ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", val);
                ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", val);
                ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", val);
                ViewData["nBanks"] = new SelectList(dp.GetBanks(), "FlutterWaveBankCode", "NAME", val);
                ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", val);
                ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", val);
                ViewData["nGender"] = new SelectList(Lc.GetAllGender(), "Value", "Text", val);
                ViewData["nemploymentStatus"] = new SelectList(Lc.GetAppStatus(), "Value", "Text", val);
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult CreatePage()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var pag = _DR.getAllPage();
                ViewBag.listPageHeader = _DR.getAllPageHeader();
                ViewBag.Data = pag;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult RecommendLoan()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                //  TempData["ErrMsg"] = "";
                //TempData["Error"] = "";
                // TempData["SucMsg"] = "";
                /*  if (TempData["SucMsg"] == null)
                  {
                      TempData["SucMsg"] = "";
                  }
                  if (TempData["ErrMsg"] == null)
                  {
                      TempData["ErrMsg"] = "";
                  }
                  if (TempData["Error"] == null)
                  {
                      TempData["Error"] = "";
                  }*/
                clearError();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var rec = _DR.getUser(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                int instFk = Convert.ToInt16(rec.PinNumber);
                ViewBag.Data = _DR.GetLoans(instFk);
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult RecommendLoans()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
              
                clearError();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var rec = _DR.getUser(user);
                var appUser = user;
                int AppStatFk = 3;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                // int instFk = Convert.ToInt16(rec.PinNumber);
                // ViewBag.Data = _DR.GetLoans(instFk);
                ViewBag.Data = _DR.GetAllLoans(AppStatFk);
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult StuApproveLoan()

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                AppStatFk = 3;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                ViewBag.Data = _DR.ApproveStudentLoans(AppStatFk);
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult ApproveLoan()
        // public ActionResult RecommendLoanSecondLevel()
        {
            try
            {
                // TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                clearError();
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
              //  TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                AppStatFk = 2;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                ViewBag.Data = _DR.ApproveLoans(AppStatFk);
                
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult DisburseLoan()
       {
            try
            {
                //TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                clearError();
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                //AppStatFk = 1;
                AppStatFk = 7;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                ViewBag.Data = _DR.ApproveLoans(AppStatFk);
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult Disburse(string Refid, TableObjects.LoanApplication LoanApp, AppLoan Apploan)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                // AppStatFk = 1;
                AppStatFk = 7;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("DisburseLoan");
                }

                var LoanApps = _DR.GetLoanApplication(Refid, AppStatFk);
                if (LoanApps == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("DisburseLoan");
                }
                /* Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                 Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                 Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                 Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                 Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                 Apploan.ContactAddress = string.IsNullOrEmpty(LoanApps.ContactAddress) ? "none" : LoanApps.ContactAddress;
                 Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;

                 Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                 Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                 Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                 Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                 Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                 Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                 Apploan.Repayment = string.IsNullOrEmpty(LoanApps.Repayment) ? "none" : LoanApps.Repayment;
                 Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                 Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                 Apploan.Organization =
                     string.IsNullOrEmpty(Apploan.Organization) ? "none" : Apploan.Organization;
                 Apploan.NOK_Relationship =
                     string.IsNullOrEmpty(LoanApps.NOK_Relationship) ? "none" : LoanApps.NOK_Relationship;
                 Apploan.NOK_PhoneNumber = string.IsNullOrEmpty(LoanApps.NOK_PhoneNumber) ? "none" : LoanApps.NOK_PhoneNumber;
                 Apploan.NOK_HomeAddress = string.IsNullOrEmpty(LoanApps.NOK_HomeAddress) ? "none" : LoanApps.NOK_HomeAddress;
                 Apploan.NOK_FullName = string.IsNullOrEmpty(LoanApps.NOK_FullName) ? "none" : LoanApps.NOK_FullName;
                 Apploan.NOK_EmailAddress =
                     string.IsNullOrEmpty(LoanApps.NOK_EmailAddress) ? "none" : LoanApps.NOK_EmailAddress;
                 Apploan.MeansOfIdentifications =
                     string.IsNullOrEmpty(LoanApps.MeansOfIdentifications) ? "none" : LoanApps.MeansOfIdentifications;
                 Apploan.MaritalStatus =
                     string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                 Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                 Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
                 Apploan.LoanAmount = LoanApps.LoanAmount;
                 string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                 Apploan.ConvertedAmount = utilities.ConvertToCurrency(LoanAmt);
                 Apploan.Landmark =
                     string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                 Apploan.IdentficationNumber = string.IsNullOrEmpty(LoanApps.IdentficationNumber) ? "none" : LoanApps.IdentficationNumber;
                 Apploan.ExistingLoan = LoanApps.ExistingLoan;
                 Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                 Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                 Apploan.Firstname =
                     string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                 Apploan.ID = LoanApps.ID;
                 Apploan.LoanRefNumber =
                     string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                 Apploan.ClosestBusStop =
                string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                 Apploan.Department =
                 string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                 Apploan.Occupation =
                string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                 Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                 Apploan.EmployeeStatus = string.IsNullOrEmpty(LoanApps.EmployeeStatus) ? "none" : LoanApps.EmployeeStatus;
                 Apploan.Salary = LoanApps.Salary;
                 string SalAmt = Convert.ToString(Apploan.Salary);
                 Apploan.ConvertedSalary = utilities.ConvertToCurrency(SalAmt);*/
                GetApplicantInfo(Refid, LoanApp, Apploan, LoanApps);
                TempData["Username"] = User.Firstname;
                var LoanComments = _DR.GetLoanComment(LoanApps.ID);
                if (LoanComments.Count > 0)
                {
                    GetComment(LoanComments);
                }
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void GetComment(List<AppLoan> LoanComments)
        {
            try
            {
                if (LoanComments.Count > 0)
                {

                    int i = 0;
                    for (i = 0; i <= LoanComments.Count; i++)
                    {
                        if (i == 0)
                        {
                            ViewBag.FirstCommentBy = LoanComments[i].Firstname;
                            //var fuser = _DR.getUserByID(ViewBag.FirstCommentBy);
                            /// ViewBag.FirstCommentBy = fuser.Firstname;
                            ViewBag.FirstLoanComment = LoanComments[i].LoanComment;
                        }
                        if (i == 1)
                        {
                            ViewBag.SecondCommentBy = LoanComments[i].Firstname;
                            // var suser = _DR.getUserByID(ViewBag.SecondCommentBy);
                            //ViewBag.SecondCommentBy = suser.Firstname;

                            ViewBag.SecondLoanComment = LoanComments[i].LoanComment;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());

            }
        }
        [HttpPost]
        public ActionResult Disburse(FormCollection form, TableObjects.LoanApplication LoanApps, DataAccessLayerT.DataManager.AppLoan LoanApp)
        {
            try
            {
                // TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                EmptyTemp();
                double InterestValue = 0;
                double PoratedAmt = 0;
                LoanLedger Ledger = new LoanLedger();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                LoanApp = (DataAccessLayerT.DataManager.AppLoan)TempData["LoanObj"];

                LoanAp = UpdateLoanApproval(LoanApp, form, out LoanApproval);

                /* DataAccessLayerT.DataManager.LoanApplication LoanAp = new DataAccessLayerT.DataManager.LoanApplication();
                  DataAccessLayerT.DataManager.LoanApproval LoanApproval = new DataAccessLayerT.DataManager.LoanApproval();
                  LoanApproval.Comment = Convert.ToString(form["DisburseLoanComment"]);
                  LoanApproval.CommentBy = Convert.ToInt16(userid);
                  var ido = Convert.ToString(form["Accept"]);
                  LoanApproval.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                  LoanApproval.DateCreated = MyUtility.getCurrentLocalDateTime();
                  LoanApproval.ValueDate = LoanApp.ValueDate;
                  LoanApproval.ValueTime = LoanApp.ValueTime;
                  LoanApproval.LoanApplication_FK = LoanApp.ID;
                  LoanApproval.IsVisible = 1;
                  LoanAp.LoanRefNumber = LoanApp.LoanRefNumber;
                  LoanAp.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                  LoanAp.LoanComment = Convert.ToString(form["comment"]);
                  LoanAp.DateModified = MyUtility.getCurrentLocalDateTime();
                  LoanApp.LoanComment = LoanAp.LoanComment;
                  */
                if (LoanApproval.ApplicationStatus_FK == 5)
                {
                    _DM.UpdateLoanApplication(LoanAp);
                    TempData["ErrMsg"] = LoanAp.LoanComment;
                    LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                    return RedirectToAction("Disburse");
                }
                LoansController Ln = new LoansController();
                InstCode = LoanApp.InstitutionCode;
                var salary = _DR.GetSalary(LoanApp.ID);
                if (salary == null)
                {
                    TempData["ErrMsg"] = "Please Check The Salary";
                    RedirectToAction("Disburse");
                }
                var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
                var salaryAmount = Convert.ToDouble(salary.NetMonthlyIncome);
                var LoanLegder = Ln.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage, InstCode);
                var LedgerRec = _DR.getLedgerRecord(LoanApp.LoanRefNumber);

                double TotalRepayment = Convert.ToDouble(repayment) * LoanApp.LoanTenure;
                TotalRepayment = Math.Round(TotalRepayment + Convert.ToDouble(repayment));
                LoanApp.TotalRepayment = TotalRepayment;

                //int i = 0;
                if (respMessage == "0")
                {
                    /*  for (i = 0; i < LoanApp.LoanTenure; i++)
                   {
                         for (i = 0; i < LedgerRec.Count; i++)
                         {

                             Ledger.ApplicantID = LedgerRec[i].ApplicantID;
                             Ledger.RefNumber = LedgerRec[i].RefNumber;
                             Ledger.IsVisible = 1;
                             Ledger.TranxDate = LedgerRec[i].trnDate;
                             Ledger.Credit = LedgerRec[i].Credit;
                             Ledger.Debit = LedgerRec[i].Debit;
                             Ledger.Institution_FK = LedgerRec[i].Institution_FK;
                             Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
                             _DM.InsertLoanLedger(Ledger);
                         }

                     }
 */
                    // Disburse To Customers
                     var resp = DisburseCash(LoanApp);
                    //var resp = "0";
                    if (resp != null)
                    {
                        //Inserting Disbursement
                        insertintoTransactLog(resp,LoanApp,appUser);

                        //Ends Here
                        dynamic result = JObject.Parse(resp);
                        if (result?.status == "true")
                        {
                            updateLoansLedgerr(LoanAp.LoanRefNumber);
                            var recommends = LoansLedger(LoanApp);
                            var recommend = FinallDisbursement(LedgerRec, Ledger, LoanApp, LoanAp);
                           
                            //  var recommend = _DM.UpdateLoanApplication(LoanAp);
                            if (recommend > 0)
                            {
                                //LoanApproval = _DM.UpdateLoanApproval(LoanApproval,LoanAp);
                                LoanApproval.CommentBy = userid;
                                LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                                if (LoanApproval.ID > 0)
                                {
                                    //For Sending Email to customers
                                    // LoanApp.Contract = "http://uvlot.com/Loans/OfferLetter?Refid=15764171114767";
                                    LoggedinInst = LoggedInuser.LoggedInInstitution();
                                    LoanApp.InstitutionEmailAddress = LoggedinInst;
                                    ///  LoanApp.Contract = "http://localhost:28957/Loans/DisburseOfferLetter?Refid=" + LoanApp.LoanRefNumber;
                                    LoanApp.Contract = ConfigurationManager.AppSettings["DisburseOfferLetter"]+ LoanApp.LoanRefNumber;
                                    LoanApp.AppStat = 6;
                                    lc.SendEmail(LoanApp);

                                    //TempData["Offer"] = "1";
                                    TempData["Offer"] = "6";
                                    return RedirectToAction("DisburseLoan");
                                    //return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });
                                }
                                else
                                {
                                    TempData["Error"] =  "Loan Already Disbursed ! But Error Updating Application Status";
                                    //return RedirectToAction("Disburse", new { @Refid = LoanApp.LoanRefNumber });
                                    return RedirectToAction("DisburseLoan");
                                }
                            }
                        }
                        else
                        {
                            TempData["Error"] = result?.message;
                            return RedirectToAction("Disburse", new { @Refid = LoanApp.LoanRefNumber });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Error Disbursing Loan! Try Again";
                        return RedirectToAction("DisburseLoan", new { @Refid = LoanApp.LoanRefNumber });
                    }
                }
                else if (respMessage != "0")
                {

                    TempData["Error"] = respMessage;
                    return RedirectToAction("Disburse", new { @Refid = LoanApp.LoanRefNumber });

                }

                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


       public void insertintoTransactLog(dynamic resp,dynamic LoanAp,string user)
        {
            try
            {

                PatnerTransactLog PatnerLog = new PatnerTransactLog();
                //PatnerLog.ContactPerson = resp.Firstname;
                PatnerLog.PatnerResponse = resp;
                PatnerLog.EmailAddress = user;
                //PatnerLog.PhoneNumber = resp.PhoneNumber;
                PatnerLog.DateCreated = resp.MyUtility.getCurrentLocalDateTime();
                PatnerLog.RefNum =LoanAp.LoanRefNumber;
               PatnerLog.BankAcct = LoanAp.AccountNumber;
               //PatnerLog.PatnerResponse = resp;
                PatnerLog.PatnerReference = resp.transactionid;
                PatnerLog.PatnerCode = "Disbusrsement";
                
             
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
               
            }
        }

        //[HttpPost]
        /* public JsonResult GetApplicantName(string Prefix)
        {
             try
             {
                 //Searching records from list using LINQ query  
                 var CityList = _DR.GetApplicantName(Prefix);

                 return Json(CityList , JsonRequestBehavior.AllowGet);
             }
             catch(Exception ex)
             {
                 WebLog.Log(ex.Message.ToString());
                 return null;
             }
         }
         */
        [HttpPost]
        public JsonResult GetApplicantName(string prefix)
        {

            var customer = _DR.GetApplicantName(prefix);

            return Json(customer);

          
        }



        [HttpGet]
        public ActionResult DocUploads(string Refid)
        {
          
                try
                {
                TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                LoanLedger Ledger = new LoanLedger();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                string RefNum = Request.QueryString["RefNum"];
                if (RefNum != null)
                {
                    Refid = RefNum;
                }
                if (Refid == null)
                {
                    GetMenus();
                    return View();
                };
                var signat = dp.getSignature(Refid);
                if (signat != null)
                {
                    var Lastsignat = signat.FirstOrDefault();
                    if (Lastsignat.signature != "none")
                    {
                        ViewBag.SignedOffer = signat;
                    }
                }
                if (signat != null || signat == null)
                {
                    string IdentImage = "";
                    var DocUpload = _DR.GetDocument(userid, Refid);
                    foreach (var q in DocUpload)
                    {
                        if (q.IdentficationNumberImage.StartsWith("/images"))
                        {
                            // IdentImage = "C:\\Users\\Reliance Limited\\Documents\\Visual Studio 2015\\Projects\\UvlotApplication\\UvlotApplication\\" + q.DocImagePath;
                            // IdentImage = IdentImage.After("C:\\Users\\Reliance Limited\\Documents\\Visual Studio 2015\\Projects\\UvlotApplication\\UvlotApplication\\").Replace("\\", "/");
                            // string slash = "/";
                            // q.DocImagePath = string.IsNullOrEmpty(IdentImage) ? "none" :  IdentImage;
                            // /images/assign.png
                        }

                        if (q.IdentficationNumberImage.StartsWith("h"))
                        {
                            // IdentImage = q.IdentficationNumberImage.After("C:\\Users\\Reliance Limited\\Documents\\Visual Studio 2015\\Projects\\UvlotApplication\\UvlotApplication\\").Replace("\\", "/");
                            IdentImage = q.IdentficationNumberImage.After("h:\\root\\home\\paelyt-001\\www\\Uvlot\\").Replace("\\", "/");
                            string slash = "/";
                            q.IdentficationNumberImage = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                        }


                    }
                    ViewBag.Data = DocUpload;
                    ViewBag.RefID = Refid;
                    GetMenus();
                }
                return View();
                }
             
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult DocUploads(FormCollection form,HttpPostedFileBase[] file)
        {
            try
            {
                TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                LoanLedger Ledger = new LoanLedger();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
               
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var ServerSavePath = ""; var fileName = "";
                var RefNum = Convert.ToString(form["refNum"]);
                string respMsg;
                var Validate = _DR.CheckAppStatusWithRef(RefNum,out respMsg);
                if(Validate == false)
                {
                    GetMenus();
                    TempData["ErrMsg"] = respMsg;
                    return RedirectToAction("DocUploads");
                }
                int count = _DR.CountDocsForReview(RefNum, out respMsg);
                if (count == 3 )
                {
                    GetMenus();
                    TempData["ErrMsg"] = respMsg;
                    //   return RedirectToAction("DocUploads");
                    return RedirectToAction("DocUploads", new { @RefNum = RefNum });
                }
                
                if (RefNum == null || RefNum == "")
                {
                    GetMenus();
                    TempData["ErrMsg"] = "Please Provide Reference Number";
                    return RedirectToAction("DocUploads");
                  //  return View();
                }
                foreach (string files in Request.Files)
                {
                var fileContent = Request.Files[files];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                        // get a stream
                        var fileCount = files.Count();
                        for(var i = 0; i<= fileCount; i++)
                        {
                            var imgname = Convert.ToString(form["FileName"]+i);
                        }
               var stream = fileContent.InputStream;
                   // and optionally write the file to disk
               fileName = Path.GetFileName(fileContent.FileName);
                   string fileExt = Path.GetExtension(fileName);
               if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG")
               {
               ServerSavePath = Path.Combine(Server.MapPath("~/images/") + fileName);
                   //Save file to server folder  
                fileContent.SaveAs(ServerSavePath);
                //assigning file uploaded status to ViewBag for showing message to user.  
                //ViewBag.UploadStatus = file.Count().ToString() + " files uploaded successfully.";
                 TempData["SucMsg"] = "Image Succesfully Uploaded";
               }
                        DocUpload docuploads = new DocUpload();
                        docuploads.DocName = fileName;
                        docuploads.ReferenceNum = RefNum;
                        docuploads.Cols1 = MyUtility.getCurrentLocalDateTime().ToString();
                        docuploads.DocImagePath = ServerSavePath;
                        docuploads.User_FK = userid;
                        var resp = _DM.insertDocument(docuploads);

               }
                   
              // var resp = InsertUploads();
               }
     
                GetMenus();
                return RedirectToAction("DocUploads", new { @RefNum = RefNum });
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult ApproveDocUpload(FormCollection form, TableObjects.LoanApplication LoanApps, DataAccessLayerT.DataManager.AppLoan LoanApp)
        {
            try
            {
                // TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                EmptyTemp();
                GetMenus();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                LoanApp = (DataAccessLayerT.DataManager.AppLoan)TempData["LoanObj"];

                LoanApproval.Comment = Convert.ToString(form["comment"]).Replace(",", "").Trim();
                LoanApproval.CommentBy = Convert.ToInt16(userid);
                var ido = Convert.ToString(form["Accept"]);
                LoanApproval.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanApproval.DateCreated = MyUtility.getCurrentLocalDateTime();
                LoanApproval.ValueDate = LoanApp.ValueDate;
                LoanApproval.ValueTime = LoanApp.ValueTime;
                LoanApproval.LoanApplication_FK = LoanApp.ID;
                LoanApproval.IsVisible = 1;
                LoanAp.LoanRefNumber = LoanApp.LoanRefNumber;
                LoanAp.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanAp.LoanComment = Convert.ToString(form["comment"]).Replace(",", "").Trim();
                LoanAp.DateModified = MyUtility.getCurrentLocalDateTime();
               
                LoansController lc = new LoansController();
                // call this guy from here .

                if (LoanApproval.ApplicationStatus_FK == 10)
                {
                    _DM.UpdateLoanApp(LoanAp);
                    TempData["ErrMsg"] = LoanAp.LoanComment;
                    LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                    return RedirectToAction("ApproveDocUpload");
                }
                var salary = _DR.GetSalary(LoanApp.ID);
                if (salary == null)
                {
                    TempData["ErrMsg"] = "Please Check The Salary";
                    RedirectToAction("ApproveDocUpload");
                }
                InstCode = LoanApp.InstitutionCode;
                var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
                var salaryAmount = Convert.ToDouble(salary.NetMonthlyIncome);
                var LoanLegder = lc.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage, InstCode);

                if (respMessage == "0")
                {

                    var recommend = _DM.UpdateLoanApp(LoanAp);
                    if (recommend > 0)
                    {
                        LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                        if (LoanApproval.ID > 0)
                        {
                            // Send Email to Users After Recommending
                            LoggedinInst = LoggedInuser.LoggedInInstitution();
                            LoanApp.InstitutionEmailAddress = LoggedinInst;
                            lc.SendEmail(LoanApp);
                            // Send Mail to The Next Level
                            // var Roleid = 3;
                            int Roleid = Convert.ToInt16(ConfigurationManager.AppSettings["SecondApproveRole"]);

                            var EmailList = _DR.GetNextLevelUser(Roleid);
                            if (EmailList.Count > 1)
                            {
                                var i = 0;
                                for (i = 0; i < EmailList.Count; i++)
                                {
                                    user = EmailList[i];
                                    SendNextLevelEmail(user, LoggedinInst);
                                }
                            }
                            if (EmailList.Count == 1)
                            {
                                SendNextLevelEmail(EmailList[0], LoggedinInst);
                            }

                          
                            TempData["Offer"] = "1";

                         
                            TempData["SucMsg"] = "Loan Approval Stage 2 Complete!";
                            //  return RedirectToAction("ApproveDocUpload", new { @Refid = LoanApp.LoanRefNumber });
                            return RedirectToAction("ApproveDocUpload");
                        }
                        else
                        {
                            TempData["Error"] = "Error Appoving Loan! Try Again";
                            //return RedirectToAction("ApproveDocUpload", new { @Refid = LoanApp.LoanRefNumber });
                            return RedirectToAction("ViewDocuments", new { @Refid = LoanApp.LoanRefNumber });
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Error Approving Loan! Try Again";
                        // return RedirectToAction("ApproveDocUpload", new { @Refid = LoanApp.LoanRefNumber });
                        return RedirectToAction("ViewDocuments", new { @Refid = LoanApp.LoanRefNumber });
                    }
                }
                else
                {
                    TempData["Error"] = respMessage;
                    return RedirectToAction("ViewDocuments", new { @Refid = LoanApp.LoanRefNumber });
                }



            }

            catch (Exception ex)
            {
                if (LoanApp == null)
                    return RedirectToAction("RecommendLoan");
                return null;
            }

        }




       [HttpGet]
        public ActionResult ApproveDocUpload()
        {
            try
            {
                // TempData["Error"] = ""; TempData["SucMsg"] = "";
                clearError();
                LoanLedger Ledger = new LoanLedger();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
               // var UploadDoc = _DR.GetDocForApproval();
                // Get Just One Record 
                 
                var LastRecord = _DR.GetDocForApprovals();
                var LastRecords = _DR.GetSignedofferLetters();
              //  if(LastRecords.Count == 2)
              //  {
                    ViewBag.LastRecords = LastRecords;
               // }
                ViewBag.LastRecord = LastRecord;
                // For One Record
               // var DocList = UploadDoc.ToList().Distinct().GroupBy(k => (k.Firstname + "" + k.Othernames + "" + k.Organization)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());

               // ViewBag.Data = UploadDoc;
                //ViewBag.Docs = DocList;
                //var DocList = _DR.GetDocForApprovals();
                //ViewBag.Data = DocList;
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
        [HttpGet]
        
        public ActionResult ViewDocuments(string Refid, TableObjects.LoanApplication LoanApp, AppLoan Apploan)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("RecommendLoan");
                }
                AppStatFk = 1;
                GetMenus();
                var LoanApps = _DR.GetLoanApplication(Refid, AppStatFk);


               var UploadDoc = _DR.GetDocForApproval(Refid);
                foreach (var q in UploadDoc)
                {
                  
                    //string IdentImage = q.DocumentPath.After("C:\\Users\\Reliance Limited\\Documents\\Visual Studio 2015\\Projects\\UvlotApplication\\UvlotApplication\\").Replace("\\", "/");
                    string IdentImage = q.DocumentPath.After("h:\\root\\home\\paelyt-001\\www\\Uvlot\\").Replace("\\", "/");
                    string slash = "/";
                    q.DocumentPath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                }
                var DocList = UploadDoc.ToList().Distinct().GroupBy(k => (k.Firstname + "" + k.Othernames + "" + k.Organization)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());

                ViewBag.Data = UploadDoc;
                ViewBag.Docs = DocList;

               if (LoanApps == null)
                {
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("RecommendLoan");
                }

               
                GetApplicantInfo(Refid, LoanApp, Apploan, LoanApps);
                //  TempData["LoanObj"] = LoanApp;
                if (Apploan.ApplicationStatus == "2")
                {
                    TempData["Msg"] = "Loan Application Already Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "1")
                {
                    TempData["Msg"] = "Loan Application Already Approved By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "7")
                {
                    TempData["Msg"] = "Loan Application Not Recommended By" + User.Firstname;
                }
                if (Apploan.ApplicationStatus == "5")
                {
                    TempData["Msg"] = "Loan Application Declined By" + User.Firstname;
                }
                TempData["LoanObj"] = Apploan;
                GetMenus();
               
                return View(Apploan);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       /* [HttpPost]
        public ActionResult ApproveDocUpload(string code, FormCollection form,string Refid)
        {
            try
            {
                //code:'" + LoanRef + '&' + LoanFk + '&&' + AppStatus +
                //code: '" + LoanRef + ' & ' + LoanFk + '}' + AppStatus + "'}&"
                Refid = code.Before("&").ToString();
                var LoanFk = code.After("&").Before("?");
                int AppStatus = Convert.ToInt16(code.After("?"));
                bool isValid = false;
                DataAccessLayerT.DataManager.LoanApplication LoanAp = new DataAccessLayerT.DataManager.LoanApplication();
                DataAccessLayerT.DataManager.LoanApproval LoanApproval = new DataAccessLayerT.DataManager.LoanApproval();
                LoanApproval.Comment = "Checked";
                LoanApproval.CommentBy = Convert.ToInt16(userid);
               // var ido = 7;
                LoanApproval.ApplicationStatus_FK = AppStatus;
                var TimeDate = MyUtility.getCurrentLocalDateTime();
                LoanApproval.DateCreated = TimeDate;
                LoanApproval.ValueDate = TimeDate.Date.ToString();
                LoanApproval.ValueTime = TimeDate.TimeOfDay.ToString();
                LoanApproval.LoanApplication_FK = Convert.ToInt16(LoanFk);
                LoanApproval.IsVisible = 1;
                LoanAp.LoanRefNumber = Refid;
                LoanAp.ApplicationStatus_FK = AppStatus;
                LoanAp.LoanComment = "checked";
                LoanAp.DateModified = TimeDate;

                if(AppStatus == 10)
                {
                    LoanApproval.Comment = "Rejected";
                    var recommends = _DM.UpdateLoanApplication(LoanAp);
                    return RedirectToAction("ApproveDocUpload");
                }
                if (AppStatus == 7)
                {
                    LoanApproval.Comment = "Checked";
                    var recommend = _DM.UpdateLoanApplication(LoanAp);
                    if (recommend > 0)
                    {
                        LoanApproval = _DM.CreateLoanApproval(LoanApproval);
                        isValid = true;
                    }
                }
                return Json(isValid);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        */


            public void updateLoansLedgerr(string RefNum)
           {
            try
            {
                var UpdateLoansLeger = _DM.UpdateLoanLedgers(RefNum); 
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                
            }
        }
        public int FinallDisbursement(List<LoansLedger> LedgerRec, LoanLedger Ledger, DataAccessLayerT.DataManager.AppLoan LoanApp, LoanApplication LoanAp)
        {
            try
            {
              
                
                var recommend = 0;
                int i = 0;
                if (respMessage == "0")
                {
                    for (i = 0; i < LoanApp.LoanTenure; i++)
                    {
                        for (i = 0; i < LedgerRec.Count; i++)
                        {
                            Ledger.ApplicantID = LedgerRec[i].ApplicantID;
                            Ledger.RefNumber = LedgerRec[i].RefNumber;
                            Ledger.IsVisible = 1;
                            Ledger.TranxDate = LedgerRec[i].trnDate;
                            Ledger.Credit = LedgerRec[i].Credit;
                            Ledger.Debit = LedgerRec[i].Debit;
                            Ledger.Institution_FK = LedgerRec[i].Institution_FK;
                            Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            Ledger.PaymentFlag_FK = 0;
                            _DM.InsertLoanLedger(Ledger);
                        }

                    }
                    recommend = _DM.UpdateLoanApplication(LoanAp);
                }

                return recommend;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;

            }
        }

        public LoanApplication UpdateLoanApproval(DataAccessLayerT.DataManager.AppLoan LoanApp, FormCollection form, out LoanApproval LoanApprovals)
        {
            try
            {

                DataAccessLayerT.DataManager.LoanApplication LoanAp = new DataAccessLayerT.DataManager.LoanApplication();
                DataAccessLayerT.DataManager.LoanApproval LoanApproval = new DataAccessLayerT.DataManager.LoanApproval();
                LoanApproval.Comment = Convert.ToString(form["DisburseLoanComment"]);
                LoanApproval.CommentBy = Convert.ToInt16(userid);
                var ido = Convert.ToString(form["Accept"]);
                LoanApproval.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanApproval.DateCreated = MyUtility.getCurrentLocalDateTime();
                LoanApproval.ValueDate = LoanApp.ValueDate;
                LoanApproval.ValueTime = LoanApp.ValueTime;
                LoanApproval.LoanApplication_FK = LoanApp.ID;
                LoanApproval.IsVisible = 1;
                LoanAp.LoanRefNumber = LoanApp.LoanRefNumber;
                LoanAp.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
                LoanAp.LoanComment = Convert.ToString(form["comment"]);
                LoanAp.DateModified = MyUtility.getCurrentLocalDateTime();
                LoanApp.LoanComment = LoanAp.LoanComment;
                LoanApprovals = LoanApproval;

                return LoanAp;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                LoanApprovals = LoanApproval;
                return null;
            }
        }


        [HttpGet]
        public ActionResult Test()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                dynamic dat = _DR.LoanApRep();
                ViewBag.Data = dat;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult EditPage(FormCollection form, int id)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var role = _DR.getAllPage();
                if (id != 0)
                {
                    var currentuser = _DR.getPage(id);
                    DataAccessLayerT.DataManager.Page pages = new DataAccessLayerT.DataManager.Page();
                    ViewBag.Pagename = currentuser.PageName;
                    ViewBag.Pageurl = currentuser.PageUrl;
                    ViewBag.PageDescription = currentuser.PageDescription;
                    ViewBag.id = id;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult UpdatePage(FormCollection form)
        {
            try
            {
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var page = _DR.getAllPage();
                DataAccessLayerT.DataManager.Page pages = new DataAccessLayerT.DataManager.Page();
                pages.PageID = Convert.ToInt16(form["id"]);
                pages.PageName = Convert.ToString(form["pagename"]);
                pages.PageUrl = Convert.ToString(form["pageurl"]);
                pages.PageDescription = Convert.ToString(form["Pagedescription"]);
                var id = _DR.UpdatePages(pages);
                if (id != null)
                {
                    TempData["ErrMsg"] = "Pages Updated Succesfully";
                    return RedirectToAction("CreatePage");
                }
                if (id == null)
                {
                    TempData["ErrMsg"] = "Error Updating Data ! Please Try Again";
                    GetMenus();
                    return View("EditPage");
                }

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult CreateRole(Classes.Role Role)
        {
            try
            {
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                DataAccessLayerT.DataManager.Role role = new DataAccessLayerT.DataManager.Role();
                if (Role != null)
                {
                    string value = Role.RoleName;


                    bool val = _DR.ValidateRole(value);
                    if (val == true)
                    {
                        TempData["ErrMsg"] = "Role Already Exist";

                        return RedirectToAction("CreateRole");
                    }
                    else if (val == false)
                    {

                        role.ValueDate = DateTime.Now;
                        role.IsVisible = 1;
                        role.RoleName = value;
                        role.RoleDescription = Role.RoleDescription;
                        int id = _DM.CreateRole(role);
                        TempData["SucMsg"] = "Role Added Succesful !";
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Roles Not Succesful Please Try Again!";
                    }
                }

                return Redirect("CreateRole");

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string EmptyVal(string value)
        {
            try
            {
                return value = null;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }




        [HttpGet]
        public ActionResult EditRole(FormCollection form, int id)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var role = _DR.getAllRole();
                if (id != 0)
                {
                    var currentuser = _DR.getRole(id);
                    User users = new User();
                    ViewBag.Rolename = currentuser.RoleName;
                    ViewBag.RoleDescription = currentuser.RoleDescription;
                    ViewBag.id = id;
                }
                // ViewBag.Data = user;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult UpdateRole(FormCollection form)
        {
            try
            {
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var role = _DR.getAllRole();
                DataAccessLayerT.DataManager.Role roles = new DataAccessLayerT.DataManager.Role();
                roles.RoleId = Convert.ToInt16(form["id"]);
                roles.RoleName = Convert.ToString(form["Rolename"]);
                roles.RoleDescription = Convert.ToString(form["RoleDescription"]);
                var id = _DM.UpdateRole(roles);
                if (id != null)
                {
                    TempData["ErrMsg"] = "Roles Updated Succesfully";
                    return RedirectToAction("CreateRole");
                }
                if (id == null)
                {
                    TempData["ErrMsg"] = "Error Updating Data ! Please Try Again";
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult CreateRole()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var users = _DR.getAllRole();

                ViewBag.Data = users;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult CreateUserRole()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult UsernRoles(LoanViewModel lvm, FormCollection form)
        {
            try
            {

                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                int rolval = Convert.ToInt16(TempData["userid"]);
                if (rolval == 0)
                {

                    TempData["ErrMsg"] = "Please Select A User";

                    return RedirectToAction("UsernRoles");

                }
                DataAccessLayerT.DataManager.UserRole userroles = new DataAccessLayerT.DataManager.UserRole();
                int roleid = Convert.ToInt16(form["roleid"]);
                int userid = Convert.ToInt16(form["id"]);
                userroles.Role_FK = roleid;
                //userroles.UserId = userid;
                userroles.User_FK = rolval;
                userroles.Role_FK = _DR.selectRolesByName(userroles);
                if (userroles.Role_FK != 0)
                {
                    TempData["ErrMsg"] = "User Already Have This Role";
                    //ViewBag.listUser = DataReaders.getAllUser();
                    return RedirectToAction("UsernRoles");
                    //return View("UsernRoles");
                }
                else
                {
                    userroles.Role_FK = roleid;
                    //userroles.UserId = userid;
                    userroles.User_FK = rolval;
                    userroles.IsVisible = 1;
                    userroles.ValueDate = DateTime.Now;

                    _DM.InsertUserRoles(userroles);
                    TempData["SucMsg"] = "User n Roles Created Succesfully";
                    lvm.getAllUserAndRoless = _DR.getAllUsersAndRoles();

                }
                //return View("");
                return RedirectToAction("UsernRoles");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }
        [HttpGet]
        public ActionResult CreateInstitution()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                ViewBag.listInstitution = _DR.getAllInstitutionType();
                var users = _DR.getAllInstitution();
                // users.Add(new DropDownList { ID = "0",  = "SELECT ONE" });
                ViewBag.Data = users;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            //  ViewBag.listUser = _DR.getAllUser();
        }

        [HttpGet]
        public ActionResult UsernRoles()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                DataAccessLayerT.DataManager.Role rol = new DataAccessLayerT.DataManager.Role();

                string userid = Request.QueryString["value"];
                TempData["userid"] = userid;
                if (userid == "" || userid == null)
                {

                    userid = "2016";
                }
                User users = new DataAccessLayerT.DataManager.User();

                var value = userid;
                users.ID = Convert.ToInt16(value);

                ViewBag.listUser = _DR.getAllUser();

                var result = _DR.getUserRoles(users.ID);
                var userroles = _DR.buildNamesList(users);
                var roles = _DR.buildAllRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = _DR.getPageUrl(newList);


                var model = new LoanViewModel
                {
                    GetAssignRoless = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };
                //  LoanViewModel lvm = new LoanViewModel();
                ViewBag.Data = _DR.getAllUsersAndRoles();

                GetMenus();
                return View(model);
            }


            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        //[HttpGet]
        //public ActionResult PagenRoles()
        //{
        //    try
        //    {
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }
        //        UvlotEntities uvdb = new UvlotEntities();
        //        List<SelectListItem> selectitem = new List<SelectListItem>();
        //        var listPage = _DR.getAllPage();
        //        foreach (var pagelis in listPage)
        //        {
        //            SelectListItem selectListitem = new SelectListItem
        //            {
        //                Text = pagelis.PageName,
        //                Value = Convert.ToString(pagelis.PageID)

        //            };

        //           selectitem.Add(selectListitem);
        //        }
        //        ViewBag.listPage = selectitem;

        //        // ViewBag.listPage = _DR.getAllPage();

        //        //value = Pageid;
        //        // pages.PageID = Convert.ToInt16(value);
        //        //var pag = "";
        //        //var selectedItem  = _DR.getSelectedPage(pages.PageID);
        //        //if (selectedItem != null)
        //        //{
        //        //    pag = selectedItem;
        //        //    pages.PageName = pag;
        //        //}
        //        pages.PageID = Convert.ToInt16(TempData["id"]);
        //        if (pages.PageID == 0)
        //        {
        //            pages.PageID = 1008;
        //        }
        //        var Pags = _DR.getPage(pages.PageID);
        //        Pags.PageID = Pags.PageID;
        //        var result = _DR.getPageRoles(pages.PageID);
        //        var pageroles = _DR.buildPagesList(Pags);
        //        var roles = _DR.buildAllRoleList();
        //        List<int> pagerole = pageroles;
        //        List<int> role = roles;
        //        var newList = roles.Except(pageroles);

        //        var pageurl = _DR.getPageUrl(newList);
        //        var model = new LoanViewModel
        //        {
        //            GetAssignPagess = result.ToList(),
        //            UnGetAssignRoless = pageurl.ToList(),
        //        };

        //        ViewBag.Data = _DR.getAllPagesAndRoles();
        //        GetMenus();
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        [HttpGet]
        public ActionResult PagenRoles()
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var value = "";
                string Pageid = Request.QueryString["value"];
                TempData["Pageid"] = Pageid;
                if (Pageid == "" || Pageid == null)
                {

                    Pageid = "2016";
                }

                ViewBag.listPage = _DR.getAllPage();

                //getAllPagesAndRoles();
                //var value = 2016;
                value = Pageid;
                pages.PageID = Convert.ToInt16(value);
                var pag = "";
                var selectedItem = _DR.getSelectedPage(pages.PageID);
                if (selectedItem != null)
                {
                    pag = selectedItem;
                    pages.PageName = pag;
                }
                var result = _DR.getPageRoles(pages.PageID);
                var pageroles = _DR.buildPagesList(pages);
                var roles = _DR.buildAllRoleList();
                List<int> pagerole = pageroles;
                List<int> role = roles;
                var newList = roles.Except(pageroles);

                var pageurl = _DR.getPageUrl(newList);
                var model = new LoanViewModel
                {
                    GetAssignPagess = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                ViewBag.Data = _DR.getAllPagesAndRoles();
                GetMenus();
                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult PagenRoles(LoanViewModel lvm, FormCollection form)
        {
            try
            {
                //int pagval = Convert.ToInt16(Request.QueryString["value"]);
                //string Pageid = Request.QueryString["value"];
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                int pagval = Convert.ToInt16(TempData["PageID"]);
                if (pagval == 0)
                {

                    TempData["ErrMsg"] = "Please Select A Page";

                    return RedirectToAction("PagenRoles");

                }
                var pag = "";
                int roleid = Convert.ToInt16(form["RoleId"]);
                int pageid = Convert.ToInt16(form["PageID"]);
                // var selectedItem = DataReaders.getSelectedPage(pageid);
                var selectedItem = _DR.getSelectedPage(pagval);
                if (selectedItem != null)
                {
                    pag = selectedItem;

                }
                _pa.Role_FK = roleid;
                _pa.PageName = pag;
                _pa.Role_FK = _DR.selectPageRolesByName(_pa);
                if (_pa.Role_FK != 0)
                {
                    TempData["ErrMsg"] = "Page Already Have This Role";
                    return RedirectToAction("PagenRoles");
                }
                else
                {
                    _pa.Role_FK = roleid;
                    _pa.PageName = pag;
                    _pa.IsVisible = 1;


                    _DM.InsertPageRoles(_pa);
                    TempData["SucMsg"] = "Page Role Created Succesfully";
                    return RedirectToAction("PagenRoles");
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        //[HttpPost]
        //public ActionResult PagenRoles(LoanViewModel lvm, FormCollection form)
        //{
        //    try
        //    {

        //        EmptyVal(Convert.ToString(TempData["ErrMsg"]));
        //       // int pagval = Convert.ToInt16(TempData["PageID"]);

        //        var pag = "";
        //        int roleid = Convert.ToInt16(form["RoleId"]);
        //        int pageid = Convert.ToInt16(form["Value"]);
        //        int vals = Convert.ToInt16(form["Value"]);
        //        TempData["id"] = vals;
        //        if (pageid == 0)
        //        {

        //            TempData["ErrMsg"] = "Please Select A Page";
        //            return RedirectToAction("PagenRoles");

        //        }
        //        var selectitems = _DR.getPag(pageid);
        //        ViewBag.listPage = selectitems;
        //        List<SelectListItem> selectitem = new List<SelectListItem>();
        //        var PageList = _DR.getAllPage();
        //        var newList = PageList.Concat(selectitems);
        //        foreach (var sel in selectitems)
        //        {
        //            SelectListItem selectListitem = new SelectListItem
        //            {

        //                Text = sel.PageName,
        //                Value = Convert.ToString(sel.PageID)

        //            };
        //            selectitem.Add(selectListitem);
        //        }

        //        ViewBag.listPage = selectitem;



        //        //var selectedItem = _DR.getSelectedPage(pagval);
        //        //if (selectedItem != null)
        //        //{
        //        //    pag = selectedItem;

        //        //}
        //        var veri = 1;
        //        if (veri == 0)
        //        {
        //            _pa.Role_FK = roleid;
        //            _pa.PageName = pag;
        //            _pa.Role_FK = _DR.selectPageRolesByName(_pa);
        //            if (_pa.Role_FK != 0)
        //            {
        //                TempData["ErrMsg"] = "Page Already Have This Role";
        //                return RedirectToAction("PagenRoles");
        //            }
        //            else
        //            {
        //                _pa.Role_FK = roleid;
        //                _pa.PageName = pag;
        //                _pa.IsVisible = 1;


        //                _DM.InsertPageRoles(_pa);
        //                TempData["ErrMsg"] = "Page Role Created Succesfully";
        //                return RedirectToAction("PagenRoles");
        //            }
        //        }
        //        return RedirectToAction("PagenRoles");
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}



        public List<Menus> GetAllMenu()
        {
            try
            {
                var Pages = _DR.GetAllMenu();
                return ViewBag.AllMenu = Pages.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Boolean validatePage()
        {
            try
            {
                HttpRequest request = System.Web.HttpContext.Current.Request;
                string url = request.Url.ToString();

                string[] dspilt = url.ToString().Trim().Split('/');
                var rawurl = dspilt[dspilt.Length - 1];
                StringBuilder b = new StringBuilder(rawurl.ToString());
                b.Replace(".", "}");

                ViewBag.Men = GetMenus();

                return false;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                //return ViewBag.Menus;
                return false;
            }
        }

        [HttpGet]
        public ActionResult TestUrl()
        {
            try
            {
                GetMenus();
                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult Reject()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public string GetMenus()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    // return null;
                    RedirectToAction("HomePage", "Home");
                }

                var mc = _DR.getUserID(appUser);
                //the line does the same thing

                var ids = _DR.getUserRols(mc);

                var roles = _DR.getUserRoles(ids.Cast<int>().ToList());
                foreach (var item in roles)
                { rol.Add(item.RoleName); }



                var results = _DR.getResults(rol);
                // This is For the Url Blocker 

                MenuList = results;
                //Yeah This one .
                var Menus = results.ToList().Distinct().GroupBy(k => (k.pageheader)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());

                // i added this 7May2019
                HttpRequest request = System.Web.HttpContext.Current.Request;
                string url = request.Url.ToString();

                string[] dspilt = url.ToString().Trim().Split('/');
                var rawurl = "/" + dspilt[dspilt.Length - 1];

                /* This is the real One to use
                var pagrol = _DR.ValidateRole(results, rawurl);
                  if (pagrol == true)
                  {
                      ViewBag.Menus = Menus;
                      return "";
                  }
                  if (pagrol == false)
                  {
                      if (rawurl == "/index" || rawurl == "/EditPassword" || rawurl == "/EditProfile" )
                      {
                          ViewBag.Menus = Menus;
                          return "";
                      }
                      if(rawurl.Contains("/Acknowledgement"))
                      {
                          ViewBag.Menus = Menus;
                          return "";
                      }
                  }*/
                //    var showPage = results.ToList().Where(x=> x.pageurl == rawurl).FirstOrDefault();

                //if(showPage == null )
                //{
                //    return "";
                //}

                /*   if (dspilt.Count() == 5 && rawurl.Contains("?Refid"))
                   {
                       var value = dspilt[4].ToString();
                       rawurl = "/" + value.Before("?").Trim();
                       var QueryVal = value.After("?").Trim();
                   }
                   ViewBag.Menus = results;

                  var pagrol = _DR.ValidateRole(results, rawurl);
                  if (pagrol == true)
                  {
                      ViewBag.Menus = Menus;
                      return "";
                  }
                  if (pagrol == false)
                   {
                       if (rawurl == "/index" || rawurl == "/EditPassword" || rawurl == "/EditProfile")
                       {
                           ViewBag.Menus = Menus;
                           return "";
                       }
                      // if(rawurl == "/Acknowledgement")
                      //{

                      //}
                       //string myParam = Request.QueryString["id"];
                       //if (myParam != null)
                       //{
                       //    ViewBag.Menus = Menus;
                       //    return "";
                       //}

                       var defaulturl = ConfigurationManager.AppSettings["DefaultUrl"];
                       ViewBag.Menus = Menus;
                       Response.Redirect("/");

                   }
                   */
                // return ViewBag.Menus = results;

                return ViewBag.Menus = Menus;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditProfile()
        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Session["id"] == null)
                {
                    return RedirectToAction("/", "Home");
                }

                // id = Convert.ToString(Session["userid"]);
                var userid = _DR.getUserID(user);
                userid = Convert.ToInt16(userid);
                ViewBag.Data = _DR.getUserByEmail(userid);

                GetMenus();
                return View(ViewBag.Data);
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditProfile(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                User users = new User();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                users.ID = Convert.ToInt16(form["id"]);
                users.Firstname = Convert.ToString(form["fname"]);
                users.Lastname = Convert.ToString(form["lname"]);
                users.EmailAddress = Convert.ToString(form["email"]);
                users.PhoneNumber = Convert.ToString(form["phone"]);
                users.PaswordVal = Convert.ToString(form["pasword"]);

                var Result = _DM.UpdateProfile(users);
                if (Result != "0")
                {
                    TempData["ErrMsg"] = "Profile Updated";
                    return RedirectToAction("EditProfile", "Admin");

                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult EditPassword()
        {
            try
            {
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditPassword(FormCollection form)
        {
            try
            {
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                string password = "";
                var confirmPassword = "";
                string EncrypPassword = "";
                User users = new User();
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    password = Convert.ToString(form["Password"]);
                    users.PaswordVal = Convert.ToString(form["newPassword"]);
                    confirmPassword = Convert.ToString(form["confirmPassword"]);
                    EncrypPassword = new CryptographyManager().ComputeHash(password, HashName.SHA256);
                }
                if (confirmPassword != users.PaswordVal)
                {
                    TempData["ErrMsg"] = "New Password and Confirm Password Must Match";

                    return View();
                }
                else
                {
                    var NewEncrypPassword = new CryptographyManager().ComputeHash(users.PaswordVal, HashName.SHA256);
                    users.PaswordVal = NewEncrypPassword;
                    users.EmailAddress = Session["id"].ToString();
                    //var valid = _DR.loggedIn(users.EmailAddress, EncrypPassword);
                    int? value = 0;
                    var valid = _DR.loggedIn(users.EmailAddress, EncrypPassword,out value);
                    users.PaswordVal = NewEncrypPassword;

                    string Email = users.EmailAddress;
                    User id = _DR.getUser(Email);
                    users.ID = id.ID;
                    if (valid == true)
                    {

                        _DM.UpdatePassword(users);
                        TempData["ErrMsg"] = "Password Succesfully Updated";
                        GetMenus();
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                    }

                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        //public ActionResult Report()
        public ActionResult PageRole()
        {
            try
            {
                /* getPagesx pg;
                 pg = new getPagesx
                 {
                     comp =_DR.getAllPag(),
                 };*/
                // ViewBag.Department = GetSelectListItems();
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                UvlotEntities uvdb = new UvlotEntities();
                var abc = uvdb.Pages.ToList();
                var adc = _DR.getAllPag();
                // ViewBag.Page = new SelectList(abc,"PageID","PageName");
                ViewBag.Page = abc;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var value = "";
                string Pageid = Request.QueryString["value"];
                TempData["Pageid"] = Pageid;
                if (Pageid == "" || Pageid == null)
                {

                    Pageid = "30021";
                }

                ViewBag.listPage = _DR.getAllPage();

                //getAllPagesAndRoles();
                //var value = 2016;
                value = Pageid;
                pages.PageID = Convert.ToInt16(value);
                var pag = "";
                var selectedItem = _DR.getSelectedPage(pages.PageID);
                if (selectedItem != null)
                {
                    pag = selectedItem;
                    pages.PageName = pag;
                }
                var result = _DR.getPageRoles(pages.PageID);
                var pageroles = _DR.buildPagesList(pages);
                var roles = _DR.buildAllRoleList();
                List<int> pagerole = pageroles;
                List<int> role = roles;
                var newList = roles.Except(pageroles);

                var pageurl = _DR.getPageUrl(newList);
                var model = new LoanViewModel
                {
                    GetAssignPagess = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                ViewBag.Data = _DR.getAllPagesAndRoles();
                GetMenus();
                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        //public ActionResult Report(FormCollection form, getPagesx pg)
        public ActionResult PageRole(FormCollection form, getPagesx pg)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                UvlotEntities uvdb = new UvlotEntities();
                var abc = uvdb.Pages.ToList();
                var adc = _DR.getAllPag();
                ViewBag.Page = abc;
                var pag = "";
                var selectedItem = "";
                int SelectID = pg.PageID;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                /* This id For Inserting New Roles */
                var AsnBtn = Convert.ToString(form["AssignBtn"]);
                if (AsnBtn != null)
                {
                    // int pagval = Convert.ToInt16(TempData["PageID"]);
                    int pagval = Convert.ToInt16(form["PageID"]);

                    if (pagval == 0)
                    {

                        TempData["ErrMsg"] = "Please Select A Page";

                        //  return RedirectToAction("PagenRoles");
                        return RedirectToAction("PagenRole");

                    }

                    int roleid = Convert.ToInt16(form["RoleId"]);
                    int pageid = Convert.ToInt16(form["PageID"]);
                    // var selectedItem = DataReaders.getSelectedPage(pageid);
                    selectedItem = _DR.getSelectedPage(pagval);
                    if (selectedItem != null)
                    {
                        pag = selectedItem;

                    }

                    _pa.Role_FK = roleid;
                    _pa.PageName = pag;
                    _pa.Role_FK = _DR.selectPageRolesByName(_pa);
                    if (_pa.Role_FK != 0)
                    {
                        TempData["ErrMsg"] = "Page Already Have This Role";
                        return RedirectToAction("PageRole");
                    }
                    else
                    {
                        _pa.Role_FK = roleid;
                        _pa.PageName = pag;
                        _pa.IsVisible = 1;


                        _DM.InsertPageRoles(_pa);
                        TempData["SucMsg"] = "Page Role Created Succesfully";
                        return RedirectToAction("PageRole");
                    }
                }

                /* This is For Inserting New Roles */
                var value = "";
                string Pageid = Convert.ToString(form["txt"]);
                var Pagem = _DR.getPageByName(Pageid);
                Pageid = Convert.ToString(Pagem.PageID);

                TempData["Pageid"] = Pageid;
                if (Pageid == "" || Pageid == null)
                {
                    Pageid = "2016";
                }


                ViewBag.listPage = _DR.getAllPage();


                value = Pageid;
                pages.PageID = Convert.ToInt16(value);
                pag = "";
                selectedItem = _DR.getSelectedPage(pages.PageID);
                if (selectedItem != null)
                {
                    pag = selectedItem;
                    pages.PageName = pag;
                }
                var result = _DR.getPageRoles(pages.PageID);
                var pageroles = _DR.buildPagesList(pages);
                var roles = _DR.buildAllRoleList();
                List<int> pagerole = pageroles;
                List<int> role = roles;
                var newList = roles.Except(pageroles);

                var pageurl = _DR.getPageUrl(newList);
                var model = new LoanViewModel
                {
                    GetAssignPagess = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                ViewBag.Data = _DR.getAllPagesAndRoles();
                GetMenus();

                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpGet]
        //  public ActionResult Reports(FormCollection form)
        public ActionResult UserRole(FormCollection form)
        {
            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                UvlotEntities uvdb = new UvlotEntities();
                var abc = uvdb.Users.ToList();
                ViewBag.listUser = abc;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                DataAccessLayerT.DataManager.Role rol = new DataAccessLayerT.DataManager.Role();

                string userid = Request.QueryString["value"];
                TempData["userid"] = userid;
                if (userid == "" || userid == null)
                {

                    userid = "2016";
                }
                User users = new DataAccessLayerT.DataManager.User();

                //var value = userid;

                var value = "2016";
                users.ID = Convert.ToInt16(value);

                ViewBag.listUser = _DR.getAllUser();

                users.ID = Convert.ToInt16(value);
                var pag = "";
                var selectedItem = _DR.getSelectedEmail(users.ID);
                if (selectedItem != null)
                {
                    pag = selectedItem;
                    users.EmailAddress = pag;
                }
                var result = _DR.getUserRoles(users.ID);
                var userroles = _DR.buildNamesList(users);
                var roles = _DR.buildAllRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = _DR.getPageUrl(newList);
                var model = new LoanViewModel
                {
                    GetAssignRoless = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                ViewBag.Data = _DR.getAllUsersAndRoles();

                GetMenus();
                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult DisapprovedLoans()
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

        public void ClearError()
        {
            try
            {
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());

            }
        }

        public bool ValidateUrl()
        {
            try
            {
                /*  var LoggedInuser = new LogginHelper();
                  user = LoggedInuser.LoggedInUser();

                  var appUser = user;
                  if (appUser == null)
                  {
                      // return null;
                      RedirectToAction("HomePage", "Home");
                  }

                  var mc = _DR.getUserID(appUser);
                  //the line does the same thing

                  var ids = _DR.getUserRols(mc);

                  var roles = _DR.getUserRoles(ids.Cast<int>().ToList());
                  foreach (var item in roles)
                  { rol.Add(item.RoleName); }
                  var results = _DR.getResults(rol);
                  var Menus = results.ToList().Distinct().GroupBy(k => (k.pageheader)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());*/

                // i added this 7May2019
                HttpRequest request = System.Web.HttpContext.Current.Request;
                string url = request.Url.ToString();
                GetMenus();
                string[] dspilt = url.ToString().Trim().Split('/');
                var rawurl = "/" + dspilt[dspilt.Length - 1];
                if (rawurl.Contains("?"))
                {
                    rawurl = rawurl.Before("?");
                }
                var pagrol = _DR.ValidateRole(MenuList, rawurl);
                if (pagrol == true)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }
        [HttpPost]
        // public ActionResult Reports(FormCollection form, LoanViewModel lvm)
        public ActionResult UserRole(FormCollection form, LoanViewModel lvm)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                DataAccessLayerT.DataManager.UserRole userrolz = new DataAccessLayerT.DataManager.UserRole();
                int userid = 0;
                var email = lvm.PageModel.PageName;
                var id = _DR.getUserID(email);
                TempData["userid"] = id;
                EmptyVal(Convert.ToString(TempData["ErrMsg"]));
                int rolval = Convert.ToInt16(TempData["userid"]);
                if (rolval == 0)
                {
                    TempData["ErrMsg"] = "Please Select A User";
                    return RedirectToAction("UserRole");
                }
                var AsnBtn = Convert.ToString(form["AssignBtn"]);
                if (AsnBtn != null)
                {

                    int roleid = Convert.ToInt16(form["roleid"]);
                    userid = Convert.ToInt16(form["id"]);
                    userrolz.Role_FK = roleid;
                    //userroles.UserId = userid;
                    userrolz.User_FK = rolval;
                    userrolz.Role_FK = _DR.selectRolesByName(userrolz);
                    if (userrolz.Role_FK != 0)
                    {
                        TempData["ErrMsg"] = "User Already Have This Role";

                        // return RedirectToAction("UsernRoles");
                        return RedirectToAction("UsernRole");

                    }
                    else
                    {
                        userrolz.Role_FK = roleid;
                        userrolz.User_FK = rolval;
                        userrolz.IsVisible = 1;
                        userrolz.ValueDate = DateTime.Now;

                        _DM.InsertUserRoles(userrolz);
                        TempData["ErrMsg"] = "User n Roles Created Succesfully";
                        lvm.getAllUserAndRoless = _DR.getAllUsersAndRoles();

                    }
                    return RedirectToAction("UserRole");
                }

                DataAccessLayerT.DataManager.Role rol = new DataAccessLayerT.DataManager.Role();

                TempData["userid"] = userid;

                User users = new DataAccessLayerT.DataManager.User();
                userid = rolval;
                var value = userid;
                users.ID = Convert.ToInt16(value);

                ViewBag.listUser = _DR.getAllUser();

                var result = _DR.getUserRoles(users.ID);
                var userroles = _DR.buildNamesList(users);
                var roles = _DR.buildAllRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = _DR.getPageUrl(newList);


                var model = new LoanViewModel
                {
                    GetAssignRoless = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                ViewBag.Data = _DR.getAllUsersAndRoles();

                GetMenus();
                return View(model);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public IEnumerable<SelectListItem> GetSelectListItems()
        {

            UvlotEntities uvdb = new UvlotEntities();
            List<SelectListItem> selectitem = new List<SelectListItem>();
            var PageList = _DR.getAllPages();
            foreach (var pagelis in PageList)
            {
                SelectListItem selectListitem = new SelectListItem
                {
                    Text = pagelis.PageName,
                    Value = pagelis.PageName,
                    //Value = Convert.ToString(pagelis.PageID)

                };
                selectitem.Add(selectListitem);
            }
            return selectitem;
        }

        public void SendNextLevelEmail(string user,string instEmail)
        {
            try
            {

                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/NextLevelEmailNotification.html"));
                //bodyTxt = bodyTxt.Replace("$UserName",user).Replace("$MerchantName", $"{lvm.Firstname} {lvm.Surname}").Replace("$LoanComment", $"{lvm.LoanComment}");
                bodyTxt = bodyTxt.Replace("$UserName", user);

                var msgHeader = $"Welcome to CashNowNow";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, user, instEmail, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }

        [HttpGet]
        public ActionResult RejectedApplications()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult RejectedApplications(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                //ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                int AppStatFk = Convert.ToUInt16(form["ApplicationStatus"]);
                //AppStatFk = 1;
                ViewBag.Data = _DR.ApprovedloanReport(AppStatFk);

                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult ApprovedApplications()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult ApprovedApplications(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                //ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                int AppStatFk = Convert.ToUInt16(form["ApplicationStatus"]);
                //AppStatFk = 1;
                ViewBag.Data = _DR.ApprovedloanReport(AppStatFk);

                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        //Amaka Region

        [HttpGet]
        public ActionResult AllApprovedLoans()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                AppStatFk = 1;
                ViewBag.Data = _DR.ApprovedloanReport(AppStatFk);

                Session["AllTransaction"] = ViewBag.Data;

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult AllApprovedLoans(FormCollection form)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            LoanApplication lonobj = new LoanApplication();
            try

            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                    AppStatFk = Convert.ToInt32(form["ApplicationStatus"]);
                    ViewBag.Data = _DR.ApprovedloanReport(AppStatFk);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult CheckPayrollLoan()
        {
            // GetMenus();
            // ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);
            return View();
        }


        [HttpPost]
        public ActionResult CheckPayrollLoan(FormCollection form, LoanViewModel lvm)
        {
            LoanApplication lonobj = new LoanApplication();
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                string msg = "";
                var ApplicationFk = Convert.ToString(form["RefNumber"]);
                ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);


                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult LoanTransactionbyDate()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {

                    var to = DateTime.Today;
                    var From = DateTime.Today.AddDays(-100);

                    ViewBag.Data = _DR.LoanTransactionbyDate(to, From);
                    Session["AllTransaction"] = ViewBag.Data;

                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult LoanTransactionbyDate(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    var to = DateTime.Today;
                    var From = DateTime.Today.AddDays(-10);

                    ViewBag.Data = _DR.LoanTransactionbyDate(to, From);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        [HttpGet]
        public ActionResult LoanReport()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();
                    ViewBag.Data = _DR.LoanAppReport();
                    Session["AllTransaction"] = ViewBag.Data;
                }
                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }



        }


        [HttpPost]
        public ActionResult LoanReport(LoanApplication loanApp)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.Data = _DR.LoanAppReport();
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult LoanRepayment()
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    var to = DateTime.Today;
                    var From = DateTime.Today.AddDays(-100);

                    ViewBag.Data = _DR.LoanRepayment(to, From);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }

        // This is For a recommender to get all Recommended and Approved Loan
        [HttpPost]
        public ActionResult MyLoans(FormCollection form)
        {
            LoanApplication lonobj = new LoanApplication();
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    userid = _DR.getUserID(user);
                    ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                    AppStatFk = Convert.ToInt32(form["ApplicationStatus"]);
                    ViewBag.Data = _DR.GetMyLoans(AppStatFk, userid);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult MyLoans()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                AppStatFk = 1;
                userid = _DR.getUserID(user);
                ViewBag.Data = _DR.GetMyLoans(AppStatFk, userid);
                Session["AllTransaction"] = ViewBag.Data;

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult LoanRepayment(FormCollection form)
        {
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    var to = DateTime.Today;
                    var From = DateTime.Today.AddDays(-10);

                    ViewBag.Data = _DR.LoanRepayment(to, From);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }
        [HttpGet]
        public ActionResult ImportExcel1()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                // LoansController lc = new LoansController();
                // lc.PayrollLoanCalculation();
                // ViewBag.LoanCal = Helper.PayrollLoanCalc(112500, 11, 4.50).ToString();
                // ViewBag.mySum = Helper.CalculateSum(23, 32).ToString();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                
                    GetMenus();
                
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [ActionName("ImportExcel1")]
        [HttpPost]
        public ActionResult ImportExcel1(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                if (Request.Files["FileUpload1"].ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();
                    // string query = null;
                    string connString = "";

                    Session["myDtVal"] = null;


                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };

                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads"), Request.Files["FileUpload1"].FileName);
                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads"));
                    }
                    if (validFileTypes.Contains(extension))
                    {
                        if (System.IO.File.Exists(path1))
                        {
                            System.IO.File.Delete(path1);
                        }
                        Request.Files["FileUpload1"].SaveAs(path1);
                        if (extension == ".csv")
                        {
                            DataTable dt = Utility.ConvertCSVtoDataTable(path1);
                            ViewBag.Data = dt;
                            Session["myDtVal"] = dt;
                        }
                        //Connection String to Excel Workbook  
                        else if (extension.Trim() == ".xls")
                        {
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            ViewBag.Data = dt;
                            Session["myDtVal"] = dt;
                        }
                        else if (extension.Trim() == ".xlsx")
                        {
                            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            ViewBag.Data = dt;
                            Session["myDtVal"] = dt;
                        }

                    }
                    else
                    {
                        ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";
                        GetMenus();
                    }
                    GetMenus();
                }

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult ImportStudentRecord()
        {

            try
            {
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [ActionName("ImportStudentRecord")]
        [HttpPost]
        public ActionResult ImportStudentRecord(FormCollection form)
        {
            try
            {
                if (Request.Files["FileUpload1"].ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();
                    // string query = null;
                    string connString = "";

                    Session["myDtVal"] = null;


                    string[] validFileTypes = { ".xls", ".xlsx", ".csv" };

                    string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads"), Request.Files["FileUpload1"].FileName);
                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads"));
                    }
                    if (validFileTypes.Contains(extension))
                    {
                        if (System.IO.File.Exists(path1))
                        {
                            System.IO.File.Delete(path1);
                        }
                        Request.Files["FileUpload1"].SaveAs(path1);
                        if (extension == ".csv")
                        {
                            DataTable dt = Utility.ConvertCSVtoDataTable(path1);
                            ViewBag.Data = dt;
                            Session["myDtVal"] = dt;
                        }
                        //Connection String to Excel Workbook  
                        else if (extension.Trim() == ".xls")
                        {
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            ViewBag.Data = dt;
                            Session["myDtVal"] = dt;
                        }
                        else if (extension.Trim() == ".xlsx")
                        {
                            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                            ViewBag.Data = dt;
                            Session["myDtVal"] = dt;
                        }

                    }
                    else
                    {
                        ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";

                    }
                    GetMenus();
                }

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string InsertUsers()
        {
            try
            {
                return "";
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult uploaddata(FormCollection form)
        {
            try
            {
                TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                float PorationValue = 0;
                UvlotEntities db = new UvlotEntities();
                //string refNumber = MyUtility.GenerateRefNo();
                DataTable dt = (DataTable)Session["myDtVal"];
                DataReader dr = new DataReader();
                // GetMenus();
                foreach (DataRow row in dt.Rows)
                    // row.Table.Rows.Count;

                    try
                    {
                        var sname = row["SURNAME"].ToString();
                        var fname = row["FIRST NAME"].ToString();
                       
                        //  LoanApplication inst = new LoanApplication();
                        if (row["SURNAME"] != null   && row["FIRST NAME"] != null )
                        {
                            if ( sname != "" || fname != "" )
                            {
                                var ddat = Convert.ToDateTime(row["DISBURSEMENT DATE"]);
                                var CheckIfExist = _DR.ValidateRec(sname, fname, ddat);
                                if (CheckIfExist == true)
                                {
                                    TempData["ErrMsg"] = "Record Already Exist !";
                                    //break;
                                    continue;
                                }
                            }
                            if (row["SURNAME"].ToString().Length < 2 && row["FIRST NAME"].ToString().Length < 2 )
                            {
                                // break;
                                continue;
                            }
                        }
                        User u = new User();
                        u.Firstname = row["FIRST NAME"].ToString();
                        u.EmailAddress = row["PERSONAL EMAIL"].ToString();
                        u.EmploymentStatus_FK = 1;
                        u.ReferralLevel = 1;
                        u.PhoneNumber = row["MOBILE NUMBER"].ToString();
                        var pasw = "password";
                        var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(pasw, HashName.SHA256);


                        bool val = _DR.Validate(u.EmailAddress, u.PhoneNumber);
                        if (val == true)
                        {
                            TempData["ErrMsg"] = "User Already Exist ! Loan Application Submitted";
                            var uid = _DR.getUser(u.EmailAddress);
                            userid = Convert.ToInt16(uid.ID);
                           // return View("Signup");
                        }
                        else if (val == false)
                        {

                            var Userid = _DM.InsertUser(u);
                            if (Userid != 0)
                            {

                                UserRole UserRoles = new UserRole();
                                UserRoles.User_FK = Userid;
                                UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["Applicant"]);
                                UserRoles.IsVisible = 1;
                                _DM.InsertUserRoles(UserRoles);

                            }
                        }
                        // var userid = _DM.InsertUser(u);
                        
                        LoanApplication instObj = new LoanApplication();
                        instObj.Institution_FK = dr.GetInstitution(row["INSTITUTION"].ToString());
                        instObj.LoanTenure = Convert.ToInt16(row["Tenure"]);
                        int Tenure = Convert.ToInt16(instObj.LoanTenure);
                        //var LoanTenure = _DR.GetInterestRate(Tenure);
                        int insfk = Convert.ToInt16(instObj.Institution_FK);
                        var LoanTenure = _DR.getInterestRateByInstFk(Tenure, insfk);
                        if (LoanTenure == 0)
                        {
                            GetMenus();
                            TempData["ErrMsg"] = "Please Check Institution";
                            return View("Importexcel1");
                        }
                       
                        instObj.AccomodationType_FK = dr.GetAccomType(row["ACCOMMODATION TYPE"].ToString());
                        instObj.AccountName = row["ACCOUNT NAME"].ToString();
                        instObj.AccountNumber = row["ACCOUNT NUMBER"].ToString();

                        instObj.ApplicantID = row["EMPLOYEE ID"].ToString();
                        instObj.EmailAddress = row["PERSONAL EMAIL"].ToString();
                        //Pending status,
                        instObj.BVN = row["BANK VERIFICATION NUMBER"].ToString();
                        instObj.ClosestBusStop = row["CLOSEST BUSTOP"].ToString();

                        var DOB = Convert.ToDateTime(row["DATE OF BIRTH"]);
                        WebLog.Log("DOB"+ DOB);
                        instObj.ContactAddress = row["HOME ADDRESS"].ToString();
                        instObj.CreatedBy = Convert.ToString(userid);//""; //Change To User ID
                        instObj.RepaymentMethod_FK = _DR.getRepaymentByName(row["REPAYMENT METHOD"].ToString());
                        // instObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                        //instObj.DateCreated = row["PERSONAL EMAIL"].ToString()
                        var datetimes = row["DISBURSEMENT DATE"].ToString();
                        WebLog.Log("datetime"+ datetimes);
                        //datetime = Convert.ToDateTime(datetime);
                        instObj.DateCreated = Convert.ToDateTime(row["Date Applied"]);
                       /* var dttin = Convert.ToString((row["Date Applied"]));

                        instObj.DateCreated = Convert.ToDateTime(dttin); //DateTime.ParseExact(dttin, "yyyy/MM/dd", new CultureInfo("en-US"));*/
                        WebLog.Log("instObj.DateCreated" + instObj.DateCreated);
                        // instObj.DateModified = Convert.ToDateTime(datetime);
                        instObj.DateModified = Convert.ToDateTime(row["DISBURSEMENT DATE"]); //Convert.ToDateTime(datetimes);//DateTime.ParseExact(datetimes, "yyyy/MM/dd", new CultureInfo("en-US"));
                        WebLog.Log("instObj.DateModified" + instObj.DateModified);
                        instObj.DateOfBirth = DOB.ToString("yyyy/MM/dd");// Helper.isDate(DOB) == true ? Convert.ToDateTime(DOB) : MyUtility.getCurrentLocalDateTime();
                        instObj.EmailAddress = row["PERSONAL EMAIL"].ToString();
                        instObj.ExistingLoan = row["ANY EXISTING LOAN"].ToString().ToUpper() == "YES" ? true : false;

                        instObj.ExistingLoan_NoOfMonthsLeft =Convert.ToInt16(row["EXISTING LOAN TENURE LEFT"]);

                        instObj.ExistingLoan_OutstandingAmount = Convert.ToDouble(row["EXISTING LOAN AMOUNT"]);
                        instObj.Firstname = row["FIRST NAME"].ToString();
                        instObj.Gender_FK = row["GENDER"].ToString().ToUpper() == "M" ? 1 : 2;

                        instObj.IdentficationNumber = row["IDENTIFICATION NUMBER"].ToString();
                        instObj.Landmark = row["LANDMARK"].ToString();
                        instObj.LGA_FK = dr.GetLocalGovt(row["LGA"].ToString());
                        instObj.LoanAmount = Convert.ToDouble(row["LOAN AMOUNT"]);
                        //instObj.LoanTenure = Convert.ToInt16(row["Tenure"]);
                        instObj.LoanComment = "";
                        //instObj.LoanRefNumber = MyUtility.GenerateRefNo();
                        instObj.MaritalStatus_FK = dr.GetStatus(row["MARITAL STATUS"].ToString());
                        instObj.BankCode = Convert.ToString(dr.GetBankCode(row["BANK NAME"].ToString().Trim()));
                        instObj.MeansOfID_FK = dr.GetMeansofIdbyname(row["MEANS OF ID"].ToString());


                        instObj.NOK_EmailAddress = row["NOK_EMAIL ADDRESS"].ToString();
                        instObj.NOK_FullName = row["NOK_NAME"].ToString();
                        instObj.NOK_HomeAddress = row["NOK_HOME ADDRESS"].ToString();
                        instObj.NOK_PhoneNumber = row["NOK_PHONE"].ToString();

                        instObj.NOK_Relationship = row["NOK_RELATIONSHIP"].ToString();
                        instObj.Organization = row["EMPLOYER"].ToString();
                        var instution = row["INSTITUTION"].ToString();
                        instObj.LoanRefNumber = instution + "/" + "PY" + "/" + DateTime.Now.Year.ToString().Substring(2) + "/" + MyUtility.GenerateRefNo();
                        instObj.Othernames = "";
                        instObj.PhoneNumber = row["MOBILE NUMBER"].ToString();
                        instObj.ApprovedInterest = 1;
                      //instObj.Institution_FK = dr.GetInstitution(row["INSTITUTION"].ToString());
                      //instObj.RepaymentMethod_FK = 0;
                        instObj.StateofResidence_FK = dr.GetState(row["STATE OF RESIDENCE"].ToString());
                        instObj.Surname = row["SURNAME"].ToString();
                        instObj.Title_FK = dr.GetTitleIDByName(row["TITLE"].ToString());
                        instObj.ApplicationStatus_FK = _DR.getApplicationStatus(row["App Status"].ToString());

                        instObj.IsVisible = 1;
                        instObj.ValueDate = instObj.DateCreated.Value.ToString("yyyy/MM/dd"); //MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        instObj.ValueTime = instObj.DateCreated.Value.ToString("yyyy/MM/dd");
                        //MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                        instObj.LoanProduct_FK = 2;
                        db.LoanApplications.Add(instObj);
                        db.SaveChanges();


                        EmployerLoanDetail empObj = new EmployerLoanDetail();
                        empObj.ClosestBusStop = row["ORGANISATION CLOSEST BUSTOP"].ToString();
                        //empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                        //empObj.DateModified = MyUtility.getCurrentLocalDateTime();

                        empObj.DateCreated = Convert.ToDateTime(row["Date Applied"]);
                        empObj.DateModified = Convert.ToDateTime(row["DISBURSEMENT DATE"]);

                      /*  var Dct = (row["Date Applied"]).ToString();
                       empObj.DateCreated = Convert.ToDateTime(Dct); //DateTime.ParseExact(Dct, "yyyy/MM/dd", new CultureInfo("en-US"));
                        
                        var Dct1 = (row["DISBURSEMENT DATE"]).ToString();
                        empObj.DateModified = Convert.ToDateTime(Dct1);//DateTime.ParseExact(Dct1, "yyyy/MM/dd", new CultureInfo("en-US"));
                        */

                        empObj.Department = row["DEPARTMENT"].ToString();
                        empObj.Designation = row["DESIGNATION"].ToString();
                        empObj.EmployerID = row["EMPLOYEE ID"].ToString();
                        empObj.EmploymentStatus_FK = dr.GetEmpoStatus(row["EMPLOYMENT STATUS"].ToString());

                        empObj.IsVisible = 1;
                        empObj.LandMark = row["ORGANISATION LANDMARK"].ToString();

                        var losy = row["LENGTH OF SERVICE YEARS"].ToString();
                        int los = Helper.isNumeric(losy) == true ? Convert.ToInt16(losy) : 0;

                        empObj.LengthOfServiceInYrs = los;


                        var losm = row["LENGTH OF SERVICE MONTHS"].ToString();
                        int lom = Helper.isNumeric(losm) == true ? Convert.ToInt16(losm) : 0;

                        empObj.LengthOfServiceInMth = lom;
                        var LGAFK = _DR.GetLocalGovt(row["LGA"].ToString()); 
                        empObj.LGA_FK = Convert.ToString(LGAFK);
                        empObj.NetMonthlyIncome = Convert.ToDouble(row["NET MONTHLY INCOME"]);
                        empObj.Occupation = row["OCCUPATION"].ToString();

                        empObj.OfficialEmailAddress = row["EMAIL"].ToString();
                        empObj.ValueDate = instObj.DateCreated.Value.ToLongDateString();
                        //MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        empObj.ValueTime = instObj.DateCreated.Value.ToLongTimeString();
                        //MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                        empObj.LoanApplication_FK = instObj.ID;

                        db.EmployerLoanDetails.Add(empObj);
                        db.SaveChanges();
                        Guarantor gua = new Guarantor();
                        gua.LoanApplication_FK = instObj.ID;
                        gua.ContactAddress = row["GUARANTOR CONTACT ADDRESS"].ToString();
                        gua.EmailAddress = row["GUARANTOR EMAIL ADDRESS"].ToString();
                        gua.Othernames = row["GUARANTOR OTHERNAMES"].ToString();
                        gua.PhoneNumber = row["GUARANTOR PHONE NUMBER"].ToString();
                        gua.RelationShipWithApplicant = row["GUARANTOR RELATIONSHIP"].ToString();
                        gua.Surname = row["GUARANTOR SURNAME"].ToString();
                        db.Guarantors.Add(gua);
                        db.SaveChanges();

                       /* int Tenure = Convert.ToInt16(instObj.LoanTenure);
                        //var LoanTenure = _DR.GetInterestRate(Tenure);
                        int insfk =Convert.ToInt16(instObj.Institution_FK);
                        var LoanTenure = _DR.getInterestRateByInstFk(Tenure,insfk);
                        if (LoanTenure == 0)
                            return null;
*/
                        var LoanAmount =(double)instObj.LoanAmount;
                        var LoanTenur = (int)instObj.LoanTenure;
                        var InterestRate = Convert.ToDouble(LoanTenure);


                       // DateTime today = MyUtility.getCurrentLocalDateTime();
                       // DateTime endOfMonth = new DateTime(today.Year,
                                                    //       today.Month,
                              //DateTime.DaysInMonth(today.Year, today.Month));


                        var disburseDate = instObj.DateModified.Value.Date;
                        DateTime endOfMonth = new DateTime(disburseDate.Year,
                                                           disburseDate.Month,
                              DateTime.DaysInMonth(disburseDate.Year, disburseDate.Month));
                        //var disbursementDate = instObj.DateCreated;
                        var disbursementDate = instObj.DateModified.Value.Date;
                        int currentDay = disbursementDate.Day;
                        int LastDayOfTheMonth = DateTime.DaysInMonth(disbursementDate.Year, disbursementDate.Month);
                        if(currentDay == 30 || currentDay == 31)
                        {
                            currentDay = currentDay - 4;
                        }
                        int DaysLeft = LastDayOfTheMonth - currentDay;
                        var Repayment = utilities.PayrollLoanCalc(LoanAmount, LoanTenur, InterestRate);
                        var repay = Convert.ToDouble(Repayment);
                        if (DaysLeft > 15 || DaysLeft == 0)
                        {
                            Repayment = "0";
                            PorationValue = 0;
                            // left to you
                        }
                        int i = 0;
                        if (DaysLeft <= 15 )
                        {
                            var MonthlyInt = (repay * (InterestRate * 0.01));
                            PorationValue = (float)DaysLeft / LastDayOfTheMonth;
                            PorationValue = (float)MonthlyInt * PorationValue;
                            PorationValue = (float)Math.Round(PorationValue, 3);
                        }

                      

                        for (i = 1; i <= LoanTenur; i++)
                        {
                            LoanLedger lonObj = new LoanLedger();
                            LoansLedger lonObjs = new LoansLedger();
                            if (instution == "PYE")
                            {
                                lonObj.ApplicantID = row["IDENTIFICATION NUMBER"].ToString();
                                lonObjs.ApplicantID = row["IDENTIFICATION NUMBER"].ToString();
                            }
                            else
                            {
                                lonObj.ApplicantID = row["EMPLOYEE ID"].ToString();
                                lonObjs.ApplicantID = row["EMPLOYEE ID"].ToString();

                            }
                           
                            lonObj.Credit = 0;
                            lonObjs.Credit = 0;
                            if (i == 1)
                            {
                                lonObjs.Debit  = repay + PorationValue;
                                lonObj.Debit = repay + PorationValue;

                            }
                            if (i > 1)
                            {

                                lonObj.Debit = repay;
                                lonObjs.Debit = repay;

                            }
                            lonObj.Institution_FK = instObj.Institution_FK;
                            lonObjs.Institution_FK = instObj.Institution_FK;
                            lonObj.IsVisible = 1;
                            lonObjs.IsVisible = 1;
                            lonObj.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            lonObjs.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            lonObj.RefNumber = instObj.LoanRefNumber;
                            lonObjs.RefNumber = instObj.LoanRefNumber;
                           // lonObj.ApplicantID = row["IDENTIFICATION NUMBER"].ToString();
                            //lonObjs.ApplicantID = row["IDENTIFICATION NUMBER"].ToString();
                            lonObj.PartnerRefNumber = "";

                            // lonObj.TranxDate = instObj.DateCreated.Value.AddMonths(i-1);
                            //lonObjs.trnDate = instObj.DateCreated.Value.AddMonths(i - 1);
                             lonObj.TranxDate = endOfMonth.AddMonths(i-1);
                            lonObjs.trnDate = endOfMonth.AddMonths(i - 1);
                            lonObj.ValueDate = instObj.DateCreated.Value.Date.ToString("yyyy/MM/dd");
                         
                            //MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                            lonObj.ValueTime = instObj.DateCreated.Value.ToString("yyyy/MM/dd");
                            //MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                            lonObjs.IsVisible = 1;
                            lonObj.PaymentFlag_FK = 0;
                            db.LoansLedgers.Add(lonObjs);
                            db.SaveChanges();
                            db.LoanLedgers.Add(lonObj);
                            db.SaveChanges();
                        }

                        ////DataWriter.CreateLoanApplication(instObj);
                    }

                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                      
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                                //raise a new exception inserting the current one as the InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        WebLog.Log(raise);
                    }
            }


            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        //raise a new exception inserting the current one as the InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                WebLog.Log(raise);
            }
            GetMenus();
            return View("ImportExcel1");

        }


        [HttpPost]
        public ActionResult uploadStudentdata(FormCollection form)
        {
            try
            {

                UvlotEntities db = new UvlotEntities();
                //string refNumber = MyUtility.GenerateRefNo();
                DataTable dt = (DataTable)Session["myDtVal"];
                DataReader dr = new DataReader();
                // GetMenus();
                foreach (DataRow row in dt.Rows)
                    // row.Table.Rows.Count;

                    try
                    {
                        //  LoanApplication inst = new LoanApplication();
                        if (row["SURNAME"] != null)
                        {
                            if (row["SURNAME"].ToString().Length < 2 && row["FIRST NAME"].ToString().Length < 2)
                            {
                                break;
                            }
                        }

                        LoanApplication instObj = new LoanApplication();

                        instObj.AccomodationType_FK = dr.GetAccomType(row["ACCOMMODATION TYPE"].ToString());
                        instObj.AccountName = row["ACCOUNT NAME"].ToString();
                        instObj.AccountNumber = row["ACCOUNT NUMBER"].ToString();

                        instObj.ApplicantID = row["MEANS OF ID"].ToString();
                        instObj.EmailAddress = row["PERSONAL EMAIL"].ToString();
                        //Pending status,
                        instObj.BVN = row["BANK VERIFICATION NUMBER"].ToString();
                        instObj.ClosestBusStop = row["CLOSEST BUSTOP"].ToString();

                        string DOB = row["DATE OF BIRTH"].ToString();
                        instObj.ContactAddress = row["HOME ADDRESS"].ToString();
                        instObj.CreatedBy = ""; //Change To User ID
                        instObj.RepaymentMethod_FK = _DR.getRepaymentByName(row["REPAYMENT METHOD"].ToString());
                        // instObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                        //instObj.DateCreated = row["PERSONAL EMAIL"].ToString()
                        var datetime = row["DISBURSEMENT DATE"];
                        //datetime = Convert.ToDateTime(datetime);
                        instObj.DateCreated = Convert.ToDateTime(datetime);
                        instObj.DateModified = MyUtility.getCurrentLocalDateTime();
                        instObj.DateOfBirth = DOB;// Helper.isDate(DOB) == true ? Convert.ToDateTime(DOB) : MyUtility.getCurrentLocalDateTime();
                        instObj.EmailAddress = row["PERSONAL EMAIL"].ToString();
                        instObj.ExistingLoan = row["ANY EXISTING LOAN"].ToString().ToUpper() == "YES" ? true : false;

                        instObj.ExistingLoan_NoOfMonthsLeft = 0;

                        instObj.ExistingLoan_OutstandingAmount = 0;
                        instObj.Firstname = row["FIRST NAME"].ToString();
                        instObj.Gender_FK = row["GENDER"].ToString().ToUpper() == "MALE" ? 1 : 2;

                        instObj.IdentficationNumber = row["IDENTIFICATION NUMBER"].ToString();
                        instObj.Landmark = row["LANDMARK"].ToString();
                        instObj.LGA_FK = dr.GetLocalGovt(row["LGA"].ToString());
                        instObj.LoanAmount = Convert.ToDouble(row["LOAN AMOUNT"]);
                        instObj.LoanTenure = Convert.ToInt16(row["Tenure"]);
                        instObj.LoanComment = "";
                        instObj.LoanRefNumber = MyUtility.GenerateRefNo();
                        instObj.MaritalStatus_FK = dr.GetStatus(row["MARITAL STATUS"].ToString());
                        instObj.BankCode = Convert.ToString(dr.GetBankCode(row["BANK NAME"].ToString().Trim()));
                        instObj.MeansOfID_FK = dr.GetMeansofIdbyname(row["MEANS OF ID"].ToString());


                        instObj.NOK_EmailAddress = row["NOK_EMAIL ADDRESS"].ToString();
                        instObj.NOK_FullName = row["NOK_NAME"].ToString();
                        instObj.NOK_HomeAddress = row["NOK_HOME ADDRESS"].ToString();
                        instObj.NOK_PhoneNumber = row["NOK_PHONE"].ToString();


                        instObj.NOK_Relationship = row["NOK_RELATIONSHIP"].ToString();
                        instObj.Organization = row["INSTITUTION"].ToString();
                        instObj.Othernames = "";
                        instObj.PhoneNumber = row["MOBILE NUMBER"].ToString();

                        instObj.Institution_FK = dr.GetInstitution(row["INSTITUTION"].ToString());
                        // instObj.RepaymentMethod_FK = 0;
                        instObj.StateofResidence_FK = dr.GetState(row["STATE OF RESIDENCE"].ToString());
                        instObj.Surname = row["SURNAME"].ToString();
                        instObj.Title_FK = dr.GetTitleIDByName(row["TITLE"].ToString());
                        instObj.ApplicationStatus_FK = _DR.getApplicationStatus(row["STATUS"].ToString());

                        instObj.IsVisible = 1;
                        instObj.ValueDate = instObj.DateCreated.Value.ToLongDateString(); //MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        instObj.ValueTime = instObj.DateCreated.Value.ToLongTimeString();

                        //MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                        instObj.LoanProduct_FK = 2;
                        db.LoanApplications.Add(instObj);
                        db.SaveChanges();


                        StudentLoanDetail empObj = new StudentLoanDetail();
                        empObj.CourseOfStudy = row["Course Of Study"].ToString();
                        empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                        empObj.DateModified = MyUtility.getCurrentLocalDateTime();

                        empObj.Department = row["DEPARTMENT"].ToString();
                        empObj.CurrentLevel_FK = Convert.ToInt16(row["Current Level"]);
                        empObj.CurrentSemester_FK = Convert.ToInt16(row["Current Semester"]);
                        empObj.Faculty = Convert.ToString(row["FACULTY"]);

                        empObj.IsVisible = 1;
                        empObj.Institution = row["LANDMARK"].ToString();


                        empObj.InstitutionAddress = Convert.ToString(row["Institution Address"]); ;
                        empObj.LoanApplication_FK = instObj.ID;
                        empObj.MatricNumber = Convert.ToString(row["Matric Number"]);
                        empObj.YearOfAdmission = row["Admission Year"].ToString();

                        empObj.YearOfExpectedCompletion = row["Year Of Expected Completion"].ToString();
                        empObj.ValueDate = instObj.DateCreated.Value.ToLongDateString();
                        //MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        empObj.ValueTime = instObj.DateCreated.Value.ToLongTimeString();
                        //MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                        empObj.LoanApplication_FK = instObj.ID;

                        db.StudentLoanDetails.Add(empObj);
                        db.SaveChanges();

                    }

                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                                //raise a new exception inserting the current one as the InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        WebLog.Log(raise);
                    }
            }


            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        //raise a new exception inserting the current one as the InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                WebLog.Log(raise);
            }
            GetMenus();
            return View("ImportExcel1");

        }

        [HttpGet]
        public ActionResult CheckStudentLoanStatus()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                }
                return View();
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return
                    null;
            }

        }



        [HttpPost]
        public ActionResult ConfirmPay(FormCollection form, string ItemList)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                var LoanAps = new AppLoan();
                AppLoan Apploan = new AppLoan();
                LoanApplication LoanAp = new LoanApplication();
                DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();
                DataAccessLayerT.DataManager.LoanLedger LoanLdgers = new LoanLedger();
                data dt = new data();
                int LoanApxs = 0; //int ID = 0;
                string[] arr = ItemList.Split(',');
                IEnumerable<int> ID = arr.Select(int.Parse);
                string IDv = "";
                if (arr.Length > 1)
                {
                    for (var m = 0; m < arr.Length; m++)
                    {

                        IDv = arr[m];
                        // = IDv;
                        LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                    }

                    dt.respMSg = true;

                    LoanLdgers = _DR.getDateTime(IDv);
                    dt.InstiD = Convert.ToInt16(LoanLdgers.Institution_FK);
                    dt.DateTimes = Convert.ToString(LoanLdgers.TranxDate);
                    //  return RedirectToAction("invoice");
                    return Json(new { data = dt });
                }
                if (arr.Length == 1)
                {
                    IDv = arr[0];

                    LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                    dt.respMSg = true;
                    LoanLdgers = _DR.getDateTime(IDv);
                    dt.InstiD = Convert.ToInt16(LoanLdgers.Institution_FK);
                    dt.DateTimes = Convert.ToString(LoanLdgers.TranxDate);
                    // return RedirectToAction("invoice");
                    return Json(new { data = dt });
                }
                return View("invoice");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        /*    [HttpPost]
            public ActionResult ConfirmPays(FormCollection form, string ItemList)
            {
                try
                {
                    TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                    TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                    var LoanAps = new AppLoan();
                    AppLoan Apploan = new AppLoan();
                    LoanApplication LoanAp = new LoanApplication();
                    DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();
                    DataAccessLayerT.DataManager.LoanLedger LoanLdgers = new LoanLedger();
                    data dt = new data();
                    int LoanApxs = 0; //int ID = 0;
                    string[] arr = ItemList.Split(',');
                    IEnumerable<int> ID = arr.Select(int.Parse);
                    string IDv = "";
                    if (arr.Length > 1)
                    {
                        for (var m = 0; m < arr.Length; m++)
                        {

                            IDv = arr[m];
                            // = IDv;
                            LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                        }

                        dt.respMSg = true;

                        LoanLdgers = _DR.getDateTime(IDv);
                        dt.InstiD = Convert.ToInt16(LoanLdgers.Institution_FK);
                        dt.DateTimes = Convert.ToString(LoanLdgers.TranxDate);
                        //  return RedirectToAction("invoice");
                        return Json(new { data = dt });
                    }
                    if (arr.Length == 1)
                    {
                        IDv = arr[0];

                        LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                        dt.respMSg = true;
                        LoanLdgers = _DR.getDateTime(IDv);
                        dt.InstiD = Convert.ToInt16(LoanLdgers.Institution_FK);
                        dt.DateTimes = Convert.ToString(LoanLdgers.TranxDate);
                        // return RedirectToAction("invoice");
                        return Json(new { data = dt });
                    }
                    return View("invoice");
                }
                catch (Exception ex)
                {
                    WebLog.Log(ex.Message.ToString());
                    return null;
                }
            }
            */


        [HttpPost]
        public ActionResult ConfirmPays(UvlotApplication.Classes.TableObjects.LoanApplication LP,FormCollection form, string[] ItemList, float[] AmtPaid)
        {
            try
            {
                GetMenus();
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                LedgerTransact Lt = new LedgerTransact();
                TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                var LoanAps = new AppLoan();
                AppLoan Apploan = new AppLoan();
                LoanApplication LoanAp = new LoanApplication();
                var instFk = LP.institutionFk;
                instFk = Convert.ToInt16(TempData["InstitutionFk"]);
                DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();
                DataAccessLayerT.DataManager.LoanLedger LoanLdgers = new LoanLedger();
                data dt = new data();
                int LoanApxs = 0; //int ID = 0;
                double Ledgertransact = 0; int vals = 0;
                if (ItemList == null)
                {
                    if (Session["Inv"] != null)
                    {
                        ViewBag.Invoice = Session["Inv"];
                    }
                    GetMenus();
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", vals);
                    TempData["ErrMsg"] = "Please Select Checkbox";
                    return View("ConfirmPayments");
                }
                //  string[] arr = ItemList.Split(',');
                //  IEnumerable<int> ID = arr.Select(int.Parse);
                IEnumerable<int> ID = ItemList.Select(int.Parse);
               // IEnumerable<float> AmtPd = AmtPaid.Select(float.Parse);
                string IDv = "";
                float Ampd = 0;
               
                 if (ItemList.Length > 0)
                 {
                     for (var m = 0; m < ItemList.Length; m++)
                     {

                      IDv = ItemList[m];
                      if (AmtPaid.Length > 0)
                      {
                         Ampd = AmtPaid[m];
                      }

                        
                        var LoanLedg = _DM.InsertLedgerTransact(IDv, LoanLdger,Lt,Ampd,instFk);
                        var LoanID = Convert.ToInt16(IDv);
                        var paidAmt = _DR.GetAmountPaid(LoanID);
                        // if(Ledgertransact > Ampd)
                        if (Ampd == 0)
                        // if (paidAmt >= Ledgertransact)
                        {
                            LoanLedg.PaymentFlag_FK = 1;
                            LoanLedg.Credit = LoanLedg.Debit;
                            LoanApxs = _DM.UpdateLedger(IDv, LoanLedg);
                           
                        }
                        if (LoanLedg.Debit > paidAmt && Ampd !=0)
                        {
                            LoanLedg.PaymentFlag_FK = 4;
                            if(LoanLedg.Credit == null)
                            {
                                LoanLedg.Credit = 0;
                            }
                            LoanLedg.Credit += Ampd;
                            // LoanApxs = _DM.UpdateLedgers(IDv, LoanLdger);
                            LoanApxs = _DM.UpdateLedger(IDv, LoanLedg);
                        }
                        if (paidAmt >= LoanLedg.Debit && Ampd != 0)
                        {
                            LoanLedg.PaymentFlag_FK = 1;
                            LoanLedg.Credit = LoanLdgers.Debit;
                            LoanApxs = _DM.UpdateLedger(IDv, LoanLedg);
                        }
                       
                      
                        //LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                        //LoanApxs = _DM.LedgerTransact(IDv, LoanLdger);
                    }

                    GetMenus();
                    var startDate = Convert.ToDateTime(TempData["startDate"]);
                    var InstitutionFk = Convert.ToInt16(TempData["InstitutionFk"]);
                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]);
                    if (startDate == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return RedirectToAction("invoice");
                    }

                    var EndDates = Convert.ToString(startDate);

                    var invoice = dp.getInvoices(startDate, EndDate, InstitutionFk);

                    // ViewBag.Invoice = invoice;
                    var vav = GetPastInvoice(invoice, startDate);

                    ViewBag.Invoice = vav;
                }
               
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", vals);
                //return RedirectToAction("Confirmpayments");
                return View("ConfirmPayments");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                //   return RedirectToAction("Confirmpayments");
                return null;
            }
        }


        [HttpPost]
        public ActionResult UpdateOrganisation(UvlotApplication.Classes.TableObjects.LoanApplication LP, FormCollection form, string[] ItemList, float[] AmtPaid)
        {
            try
            {
                GetMenus();
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                //var instFk = LP.institutionFk;
                var instfk = form["NewInst"];
                data dt = new data();
                int LoanApxs = 0; 
                int vals = 0;
                if (ItemList == null)
                {
                    if (Session["Invoice"] != null)
                    {
                        ViewBag.Invoice = Session["Invoice"];
                    }
                    GetMenus();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);


                    ViewData["Institutions"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.IsVisible);
                    TempData["ErrMsg"] = "Please Select Checkbox";
                    return View("UpdateEmployees");
                }
              
                IEnumerable<int> ID = ItemList.Select(int.Parse);
             
                string IDv = "";
                float Ampd = 0;

                if (ItemList.Length > 0)
                {
                    for (var m = 0; m < ItemList.Length; m++)
                    {

                            int NewOrg = Convert.ToInt16(instfk); //(Int16)LoanAp.IsVisible;
                            LoanApxs = _DM.UpdateOrganisation(ItemList[m], NewOrg);
                    }
                    
               }

                    GetMenus();

                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);


                ViewData["Institutions"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.IsVisible);
                //Session["invoice"] = ViewBag.Invoice;


                return View("UpdateEmployees");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
              
                return null;
            }
        }



        [HttpPost]
        public ActionResult UpdateLedgers(UvlotApplication.Classes.TableObjects.LoanApplication LP, FormCollection form, string[] ItemList, float[] AmtPaid)
        {
            try
            {
                GetMenus();
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                LedgerTransact Lt = new LedgerTransact();
                TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                var LoanAps = new AppLoan();
                AppLoan Apploan = new AppLoan();
                LoanApplication LoanAp = new LoanApplication();
                var instFk = LP.institutionFk;
                instFk = Convert.ToInt16(TempData["InstitutionFk"]);
                DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();
                DataAccessLayerT.DataManager.LoanLedger LoanLdgers = new LoanLedger();
                data dt = new data();
                int LoanApxs = 0; //int ID = 0;
                double Ledgertransact = 0; int vals = 0;
                if (ItemList == null)
                {
                    if (Session["Inv"] != null)
                    {
                        ViewBag.Invoice = Session["Inv"];
                    }
                    GetMenus();
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", vals);
                    TempData["ErrMsg"] = "Please Select Checkbox";
                    return View("UpdateLedger");
                }
                //  string[] arr = ItemList.Split(',');
                //  IEnumerable<int> ID = arr.Select(int.Parse);
                IEnumerable<int> ID = ItemList.Select(int.Parse);
                // IEnumerable<float> AmtPd = AmtPaid.Select(float.Parse);
                string IDv = "";
                float Ampd = 0;

                if (ItemList.Length > 0)
                {
                    for (var m = 0; m < ItemList.Length; m++)
                    {

                        IDv = ItemList[m];
                        if (AmtPaid.Length > 0)
                        {
                            Ampd = AmtPaid[m];
                        }


                        var LoanLedg = _DM.InsertLedgerTransact(IDv, LoanLdger, Lt, Ampd, instFk);
                        var LoanID = Convert.ToInt16(IDv);
                        var paidAmt = _DR.GetAmountPaid(LoanID);
                        // if(Ledgertransact > Ampd)
                        if (Ampd == 0)
                        // if (paidAmt >= Ledgertransact)
                        {
                            LoanLedg.PaymentFlag_FK = 1;
                            LoanLedg.Credit = LoanLedg.Debit;
                            LoanApxs = _DM.UpdateLedger(IDv, LoanLedg);

                        }
                        if (LoanLedg.Debit > paidAmt && Ampd != 0)
                        {
                            LoanLedg.PaymentFlag_FK = 4;
                            if (LoanLedg.Credit == null)
                            {
                                LoanLedg.Credit = 0;
                            }
                            LoanLedg.Credit += Ampd;
                            // LoanApxs = _DM.UpdateLedgers(IDv, LoanLdger);
                            LoanApxs = _DM.UpdateLedger(IDv, LoanLedg);
                        }
                        if (paidAmt >= LoanLedg.Debit && Ampd != 0)
                        {
                            LoanLedg.PaymentFlag_FK = 1;
                            LoanLedg.Credit = LoanLdgers.Debit;
                            LoanApxs = _DM.UpdateLedger(IDv, LoanLedg);
                        }


                        //LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                        //LoanApxs = _DM.LedgerTransact(IDv, LoanLdger);
                    }

                    GetMenus();
                    var startDate = Convert.ToDateTime(TempData["startDate"]);
                    var InstitutionFk = Convert.ToInt16(TempData["InstitutionFk"]);
                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]);
                    if (startDate == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return RedirectToAction("invoice");
                    }

                    var EndDates = Convert.ToString(startDate);

                    var invoice = dp.getInvoices(startDate, EndDate, InstitutionFk);

                    // ViewBag.Invoice = invoice;
                    var vav = GetPastInvoice(invoice, startDate);

                    ViewBag.Invoice = vav;
                }

                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", vals);
                //return RedirectToAction("Confirmpayments");
                return View("UpdateLedger");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                //   return RedirectToAction("Confirmpayments");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GenerateInvoice(FormCollection form, string ItemList, TableObjects.LoanApplication LoanAP)

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                var LoanAps = new AppLoan();
                AppLoan Apploan = new AppLoan();
                LoanApplication LoanAp = new LoanApplication();
                DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();
                Institution Inst = new Institution();

                // Get The First Value On The ItemList

                //Ends Here//
                int LoanApxs = 0;
                string[] arr = ItemList.Split(',');
                if (arr.Length == 1)
                {
                    Inst.ID = _DR.getInstitutionByLedgerID(arr[0]);
                }
                else if (arr.Length > 1)
                {
                    Inst.ID = _DR.getInstitutionByLedgerID(arr[0]);
                }
                IEnumerable<int> ID = arr.Select(int.Parse);
                string IDv = "";
                double TotalAmount = 0;
                double Amount = 0;
                var TodaysDate = MyUtility.getCurrentLocalDateTime();
                TempData["Date"] = TodaysDate;
                TempData["DateDue"] = TodaysDate.AddDays(10);
                TempData["VAT"] = 0;
                //if (Inst.ID != 0)
                //{
                //    Inst = _DR.getInstitutionById(Inst.ID);
                //}
                if (arr.Length > 1)
                {
                    for (var m = 0; m < arr.Length; m++)
                    {
                        IDv = arr[m];
                        Amount = _DR.GetAmount(IDv);
                        TotalAmount = Math.Ceiling((double)(Amount + TotalAmount));
                        TempData["TotalAmount"] = TotalAmount;
                    }

                    data dt = new data();
                    dt.InstiD = Inst.ID;
                    dt.TotalAmounyt = Convert.ToDouble(TempData["TotalAmount"]);
                    // return Json(new { data = Inst.ID,datas = TempData["TotalAmount"] });
                    return Json(new { data = dt });
                    // return RedirectToAction("invoices", new { @instid = Inst.ID });
                }
                if (arr.Length == 1)
                {
                    IDv = arr[0];
                    Amount = _DR.GetAmount(IDv);
                    TotalAmount = (double)(Amount + TotalAmount);
                    TempData["TotalAmount"] = TotalAmount;
                    data dt = new data();
                    dt.InstiD = Inst.ID;
                    dt.TotalAmounyt = Convert.ToDouble(TempData["TotalAmount"]);
                    return Json(new { data = dt });
                }
                return View("invoices");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        [HttpPost]
        public ActionResult UpdateComp(FormCollection form, string ItemList, TableObjects.LoanApplication LoanAP)

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                var LoanAps = new AppLoan();
                AppLoan Apploan = new AppLoan();
                LoanApplication LoanAp = new LoanApplication();
                DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();
                Institution Inst = new Institution();
                var RefNum = "";
                int LoanApxs = 0;
                string[] arr = ItemList.Split(',');
               
                 if (arr.Length > 0)
                {
                     RefNum = _DM.UpdateEmp(arr[0]);

                    data dt = new data();
                    dt.InstiD = 7;
                    dt.TotalAmounyt = Convert.ToDouble(RefNum);

                    return Json(new { data = dt });

                }

                return View("UpdateEmployee");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult EarlyPayOff()
        {
            try
            {
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult EarlyPayOff(FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();
                    //var ReferenceNum = Convert.ToString(form["RefNumber"]);
                    var ReferenceNum = LoanAP.IdentficationNumber;
                    var LoanLedger = _DR.GetLoanLedger(ReferenceNum);
                    if (LoanLedger == null || LoanLedger.Count == 0)
                    {
                        TempData["ErrMsg"] = "Invalid Reference Number";
                        return View();
                    }
                    var LastRecord = LoanLedger.Distinct().Last();
                    if (LastRecord.orgCode == 2)
                    {
                        TempData["ErrMsg"] = "Loan Already Restructured";
                        TempData["code"] = "2";
                        ViewBag.LoansLedger = LoanLedger;
                        return View("EarlyPayOff");
                    }
                    if (LastRecord.orgCode == 1)
                    {
                        TempData["ErrMsg"] = "Loan Repayment Complete";
                        TempData["code"] = "2";
                        ViewBag.LoansLedger = LoanLedger;
                        return View("EarlyPayOff");
                    }

                    ViewBag.LoanAmount = LastRecord.LoanAmount;
                    ViewBag.LoanTenure = LastRecord.LoanTenure;
                    ViewBag.CreditSum = LoanLedger.Sum(x => x.Credit);
                    ViewBag.CreditCount = LoanLedger.Where(q => q.Credit != 0).Select(x => x.Credit).Count();
                    ViewBag.DebitSum = LoanLedger.Sum(x => x.Debit);
                    ViewBag.Balance = ViewBag.DebitSum - ViewBag.CreditSum;
                    string balance = Convert.ToString(ViewBag.Balance);
                    ViewBag.LoansLedger = LoanLedger;
                    var PayOff = CalPayOff(ViewBag.LoanAmount, ViewBag.LoanTenure, ViewBag.CreditCount);
                    string PayOffs = Convert.ToString(PayOff);
                    ViewBag.BalanceDue = PayOff;
                    ViewBag.Balance = utilities.ConvertToCurrency(balance);
                    ViewBag.BalanceDue = utilities.ConvertToCurrency(PayOffs);
                    if (ViewBag.CreditCount == ViewBag.LoanTenure)
                    {
                        TempData["flag"] = "0";
                    }
                    else if (ViewBag.CreditCount < ViewBag.LoanTenure)
                    {
                        TempData["flag"] = "1";
                    }
                }
                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult UpdateLedger()
        {
            try
            {
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        //[HttpGet]
        //public ActionResult UpdateLedgerNew()
        //{
        //    try
        //    {
        //        TempData["ErrMsg"] = "";
        //        TempData["SucMsg"] = "";
        //        bool valid = ValidateUrl();
        //        if (valid == false)
        //        {
        //            return RedirectToAction("Index", "Admin");
        //        }
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }
        //        else
        //        {
        //            GetMenus();

        //            return View();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}


        [HttpPost]
        public ActionResult UpdateLedger(FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();
                  
                    var ReferenceNum = LoanAP.IdentficationNumber;
                    var LoanLedger = _DR.GetLoanLedger(ReferenceNum);
                    if (LoanLedger == null || LoanLedger.Count == 0)
                    {
                        TempData["ErrMsg"] = "Invalid Reference Number";
                        return View();
                    }
                    ViewBag.LoansLedger = LoanLedger;
                }
                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult Confirmpayments()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    if (Session["invoice"] != null)
                    {
                        ViewBag.Invoice = Session["invoice"];
                    }
                    int val = 0;
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult RemitaTrans()
        {
            try
            {

                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }

                clearError();
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var rec = _DR.getUser(user);
                var appUser = user;
                // int AppStatFk = 3;
                int AppStatFk = 1;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                //ViewBag.Data = _DR.GetAllLoans(AppStatFk);
                ViewBag.Data = _DR.GetAllRemitaTrans(AppStatFk);
                Session["recs"] = ViewBag.Data;
                GetMenus();
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }



        [HttpGet]
        public ActionResult Confirmpayment()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    if (Session["invoice"] != null)
                    {
                        ViewBag.Invoice = Session["invoice"];
                    }
                    int val = 0;
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult PaymentLog()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                  
                    int val = 0;
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    return View();
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }





        [HttpPost]
        public ActionResult PaymentLog(string Month, FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                Session["AllTransaction"] = null;
                if (Month == null)
                {
                    {
                        GetMenus();
                        var dttime = Convert.ToString(form["date"]);
                        var dttimes = Convert.ToString(form["Enddate"]);
                        var startDate = DateTime.ParseExact(dttime, "dd/MM/yyyy", new CultureInfo("en-US"));
                        var EndDate = DateTime.ParseExact(dttimes, "dd/MM/yyyy", new CultureInfo("en-US"));
                        // DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));
                        //DateTime EndDate = DateTime.ParseExact(dttimes, "MM/dd/yyyy", new CultureInfo("en-US"));
                        TempData["startDate"] = startDate;
                        if (startDate == null)
                        {
                            TempData["ErrMsg"] = "Please Try Again";
                            return RedirectToAction("invoice");
                        }
                      
                        var InstitutionFk = LoanAP.institutionFk;
                       
                        ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);

                        var PayLog = dp.getPaymentLog(startDate, EndDate, InstitutionFk);

                        ViewBag.PayLog = PayLog;
                        Session["Pay"] = ViewBag.PayLog;
                        if (PayLog == null)
                        {
                            TempData["ErrMsg"] = "No Record Found ! ";
                        }


                        Session["AllTransaction"] = ViewBag.PayLog;


                    }

                }
                



                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Please Try Again";
                int val = 0;
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                return View();
            }
        }




        [HttpGet]
        public ActionResult UpdateEmployees(Classes.TableObjects.LoanApplication LoanApp)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                //bool valid = ValidateUrl();
                //if (valid == false)
                //{
                //    return RedirectToAction("Index", "Admin");
                //}
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    if (Session["invoice"] != null)
                    {

                         ViewBag.Invoice = Session["invoice"];
                    }
                    int val = 0;
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);

                    ViewBag.nInstitutionss = dp.getAllInstitution();
                    ViewData["Institutions"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        [HttpGet]
        public ActionResult UpdateEmployee(Classes.TableObjects.LoanApplication LoanApp)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    if (Session["invoice"] != null)
                    {
                        
                        var getEmployees = dp.getEmployees(LoanApp.institutionFk);

                        ViewBag.Invoice = getEmployees;
                       // ViewBag.Invoice = Session["invoice"];
                    }
                    int val = 0;
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    ViewBag.nInstitutionss = dp.getAllInstitution();
                    ViewData["Institutions"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult UpdateEmployee(string Month, FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Month == null)
                {
                    {
                        GetMenus();

                        var InstitutionFk = LoanAP.institutionFk;


                        if (InstitutionFk == 0)
                        {
                            TempData["ErrMsg"] = "Please Try Again";
                            return RedirectToAction("UpdateEmployee");
                        }

                        ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);
                     
                        // var invoice = dp.getInvoice(startDate, EndDate, InstitutionFk);
                        var getEmployees = dp.getEmployees(InstitutionFk);

                        ViewBag.Invoice = getEmployees;
                        Session["invoice"] = ViewBag.Invoice;
                        if (getEmployees == null)
                        {
                            TempData["ErrMsg"] = "No Record Found ! ";
                        }


                        Session["AllTransaction"] = ViewBag.getEmployees;


                    }

                }

              


                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Please Try Again";
                int val = 0;
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                return View();
            }
        }


        [HttpPost]
        public ActionResult UpdateEmployees(string Month, FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Month == null)
                {
                    {
                        GetMenus();
                      
                        var InstitutionFk = LoanAP.institutionFk;

                        var InstitutionFks = LoanAP.IsVisible;
                        if (InstitutionFk == 0)
                        {
                            TempData["ErrMsg"] = "Please Try Again";
                            return RedirectToAction("UpdateEmployee");
                        }

                        ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);

                        TempData["InstTO"] = InstitutionFks;
                      ViewData["Institutions"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.IsVisible);

                     
                        var getEmployees = dp.getEmployees(InstitutionFk);
                        
                        ViewBag.Invoice = getEmployees;
                        Session["invoice"] = ViewBag.Invoice;
                        if (getEmployees == null)
                        {
                            TempData["ErrMsg"] = "No Record Found ! ";
                        }


                        Session["AllTransaction"] = ViewBag.getEmployees;


                    }

                }
             

                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Please Try Again";
                int val = 0;
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                return View();
            }
        }



        [HttpGet]
        public ActionResult invoice()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    if (Session["invoice"] != null)
                    {
                        ViewBag.Invoice = Session["invoice"];
                    }
                    int val = 0;
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult invoiceR()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    if (Session["invoice"] != null)
                    {
                        ViewBag.Invoice = Session["invoice"];
                    }
                    int val = 0;
                    ViewBag.nInstitutions = dp.getAllInstitution();
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                    return View();
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Invoice> GetPastInvoice(List<Invoice> invoice,DateTime startDate)
        {
            try
            {
                var InvoiceRecalculated = new List<Invoice>();
               
                foreach (var i in invoice)
                {
                    var LedgRec = _DR.getAllLedRecords(i.ReferenceNum, startDate);
                    var Lr = LedgRec.GroupBy(x => x.ReferenceNum).Select(g => new Invoice
                    {
                        ReferenceNum = g.Key,
                        Credit = g.Sum(x=>x.Debit) - g.Sum(x=>x.Credit),
                        Fullname = g.Select(x=>x.Fullname).FirstOrDefault(),
                        LID = g.Select(x => x.LID).FirstOrDefault(),
                        Company = g.Select(x => x.Company).FirstOrDefault(),
                        Location = g.Select(x => x.Location).FirstOrDefault(),
                        DisburseDate = g.Select(x => x.DisburseDate).FirstOrDefault(),
                        RepaymentDate = g.Select(x => x.RepaymentDate).FirstOrDefault(),
                        Interestrate = 0,
                        Tenure = g.Select(x => x.Tenure).FirstOrDefault(),
                        InterestPricipalDue = g.Select(x => x.InterestPricipalDue).FirstOrDefault(),
                       // ReferenceNum = g.Select(x => x.ReferenceNum).FirstOrDefault(),
                       EmployeeID = g.Select(x => x.EmployeeID).FirstOrDefault(),
                       AmountPaid = (from x in uvdb.LoanLedgers where x.RefNumber == i.ReferenceNum select x.Credit ).Sum().Value,
                    }).FirstOrDefault();
                    InvoiceRecalculated.Add(Lr);
                    // invoice = Lr.ToList();
                    invoice = InvoiceRecalculated.ToList();
                  
                }

                return invoice;
               
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult invoice(string Month, FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Month == null)
                {
                    {
                        GetMenus();
                        var dttime = Convert.ToString(form["date"]);
                        //DateTime startDate = Convert.ToDateTime(form["date"]);// LoanAP.DateCreated;
                        DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));
                        //DateTime startDate = Convert.ToDateTime(LoanAP.DateCreated);
                        if (startDate == null)
                        {
                            TempData["ErrMsg"] = "Please Try Again";
                            return RedirectToAction("invoice");
                        }
                        var ids = Convert.ToString(LoanAP.DateCreated);
                        DateTime EndDate = Convert.ToDateTime(form["EndDate"]); //(DateTime)LoanAP.DateModified;
                        var startDates = Convert.ToString("30/01/2020");
                        var EndDates = Convert.ToString(startDate);//Convert.ToString("30/12/2019");
                        var InstitutionFk = LoanAP.institutionFk;
                        //   startDate = DateTime.ParseExact(startDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        //EndDate = DateTime.ParseExact(EndDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);

                        var invoice = dp.getInvoice(startDate, EndDate, InstitutionFk);
                        var vav = GetPastInvoice(invoice, startDate);
                        // Check Past Debt
                        //ViewBag.Invoice = invoice;
                        ViewBag.Invoice = vav;
                        if (invoice == null)
                        {
                            TempData["ErrMsg"] = "No Record Found ! ";
                        }


                        Session["AllTransaction"] = ViewBag.Invoice;


                    }

                }
                else
                {
                    GetMenus();

                    string dttimes = Month.After("&").Before("?");
                    dttimes = dttimes.Substring(1, dttimes.Length - 5);
                    DateTime startDate = DateTime.Parse(dttimes);
                    var InstitutionFks = Month.After("?");
                    int InstitutionFk = Convert.ToInt16(InstitutionFks);
                    var Flag = Month.After("?");

                    //  DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));

                    if (startDate == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return RedirectToAction("invoice");
                    }

                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]);

                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);
                    var invoice = dp.getInvoice(startDate, EndDate, InstitutionFk);
                    var vav = GetPastInvoice(invoice, startDate);
                    //ViewBag.Invoice = invoice;
                    ViewBag.Invoice = vav;
                    if (invoice == null)
                    {
                        TempData["ErrMsg"] = "No Record Found ! ";
                    }


                    Session["invoice"] = ViewBag.Invoice;

                    data dt = new data();
                    dt.respMSg = true;

                    return Json(new { data = dt });

                }

              

                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Please Try Again";
                int val = 0;
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                return View();
            }
        }


        [HttpPost]
        public ActionResult invoiceR(string Month, FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Month == null)
                {
                    {
                        GetMenus();
                        var dttime = Convert.ToString(form["date"]);
                        //DateTime startDate = Convert.ToDateTime(form["date"]);// LoanAP.DateCreated;
                        DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));
                        //DateTime startDate = Convert.ToDateTime(LoanAP.DateCreated);
                        if (startDate == null)
                        {
                            TempData["ErrMsg"] = "Please Try Again";
                            return RedirectToAction("invoice");
                        }
                        var ids = Convert.ToString(LoanAP.DateCreated);
                        DateTime EndDate = Convert.ToDateTime(form["EndDate"]); //(DateTime)LoanAP.DateModified;
                        var startDates = Convert.ToString("30/01/2020");
                        var EndDates = Convert.ToString(startDate);//Convert.ToString("30/12/2019");
                        var InstitutionFk = LoanAP.institutionFk;
                        //   startDate = DateTime.ParseExact(startDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        //EndDate = DateTime.ParseExact(EndDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);

                        var invoice = dp.getInvoiceR(startDate, EndDate, InstitutionFk);
                        var vav = GetPastInvoice(invoice, startDate);
                        // Check Past Debt
                        ViewBag.Invoice = invoice;
                        //ViewBag.Invoice = vav;
                        if (invoice == null)
                        {
                            TempData["ErrMsg"] = "No Record Found ! ";
                        }


                        Session["AllTransaction"] = ViewBag.Invoice;


                    }

                }
                else
                {
                    GetMenus();

                    string dttimes = Month.After("&").Before("?");
                    dttimes = dttimes.Substring(1, dttimes.Length - 5);
                    DateTime startDate = DateTime.Parse(dttimes);
                    var InstitutionFks = Month.After("?");
                    int InstitutionFk = Convert.ToInt16(InstitutionFks);
                    var Flag = Month.After("?");

                    //  DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));

                    if (startDate == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return RedirectToAction("invoice");
                    }

                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]);

                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);
                    var invoice = dp.getInvoiceR(startDate, EndDate, InstitutionFk);
                    var vav = GetPastInvoice(invoice, startDate);
                    //ViewBag.Invoice = invoice;
                    ViewBag.Invoice = vav;
                    if (invoice == null)
                    {
                        TempData["ErrMsg"] = "No Record Found ! ";
                    }


                    Session["invoice"] = ViewBag.Invoice;

                    data dt = new data();
                    dt.respMSg = true;

                    return Json(new { data = dt });

                }



                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Please Try Again";
                int val = 0;
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                return View();
            }
        }

        [HttpPost]
        public ActionResult Confirmpayments(string Month, FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                Session["AllTransaction"] = null;
                if (Month == null)
                {
                    {
                        GetMenus();
                        var dttime = Convert.ToString(form["date"]);
                        //DateTime startDate = Convert.ToDateTime(form["date"]);// LoanAP.DateCreated;
                        DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));
                        TempData["startDate"] = startDate;
                        //DateTime startDate = Convert.ToDateTime(LoanAP.DateCreated);
                        if (startDate == null)
                        {
                            TempData["ErrMsg"] = "Please Try Again";
                            return RedirectToAction("invoice");
                        }
                        var ids = Convert.ToString(LoanAP.DateCreated);
                        DateTime EndDate = Convert.ToDateTime(form["EndDate"]); //(DateTime)LoanAP.DateModified;
                        var startDates = Convert.ToString("30/01/2020");
                        var EndDates = Convert.ToString(startDate);//Convert.ToString("30/12/2019");
                        var InstitutionFk = LoanAP.institutionFk;
                        TempData["InstitutionFk"] = InstitutionFk;
                        //startDate = DateTime.ParseExact(startDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        //EndDate = DateTime.ParseExact(EndDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);

                        var invoice = dp.getInvoices(startDate, EndDate, InstitutionFk);

                        // ViewBag.Invoice = invoice;

                        var vav = GetPastInvoice(invoice, startDate);
                       
                        ViewBag.Invoice = vav;
                        Session["Inv"] = ViewBag.Invoice;
                        if (invoice == null)
                        {
                            TempData["ErrMsg"] = "No Record Found ! ";
                        }


                        Session["AllTransaction"] = ViewBag.Invoice;


                    }

                }
                else
                {
                    GetMenus();

                    string dttimes = Month.After("&").Before("?");
                    dttimes = dttimes.Substring(1, dttimes.Length - 5);
                    DateTime startDate = DateTime.Parse(dttimes);
                    var InstitutionFks = Month.After("?");
                    int InstitutionFk = Convert.ToInt16(InstitutionFks);
                    var Flag = Month.After("?");

                    //  DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));

                    if (startDate == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return RedirectToAction("invoice");
                    }

                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]);

                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);
                    var invoice = dp.getInvoices(startDate, EndDate, InstitutionFk);

                    // ViewBag.Invoice = invoice;
                    var vav = GetPastInvoice(invoice, startDate);
                   
                    ViewBag.Invoice = vav;
                    if (invoice == null)
                    {
                        TempData["ErrMsg"] = "No Record Found ! ";
                    }


                    Session["invoice"] = ViewBag.Invoice;

                    data dt = new data();
                    dt.respMSg = true;

                    return Json(new { data = dt });

                }



                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Please Try Again";
                int val = 0;
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                return View();
            }
        }


      



        [HttpPost]
        public ActionResult Confirmpayment(string Month, FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Month == null)
                {
                    {
                        GetMenus();
                        var dttime = Convert.ToString(form["date"]);
                        //DateTime startDate = Convert.ToDateTime(form["date"]);// LoanAP.DateCreated;
                        DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));
                        //DateTime startDate = Convert.ToDateTime(LoanAP.DateCreated);
                        if (startDate == null)
                        {
                            TempData["ErrMsg"] = "Please Try Again";
                            return RedirectToAction("invoice");
                        }
                        var ids = Convert.ToString(LoanAP.DateCreated);
                        DateTime EndDate = Convert.ToDateTime(form["EndDate"]); //(DateTime)LoanAP.DateModified;
                        var startDates = Convert.ToString("30/01/2020");
                        var EndDates = Convert.ToString(startDate);//Convert.ToString("30/12/2019");
                        var InstitutionFk = LoanAP.institutionFk;
                        //   startDate = DateTime.ParseExact(startDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        //EndDate = DateTime.ParseExact(EndDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                        ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);

                        var invoice = dp.getInvoice(startDate, EndDate, InstitutionFk);

                        ViewBag.Invoice = invoice;
                        if (invoice == null)
                        {
                            TempData["ErrMsg"] = "No Record Found ! ";
                        }


                        Session["AllTransaction"] = ViewBag.Invoice;


                    }

                }
                else
                {
                    GetMenus();

                    string dttimes = Month.After("&").Before("?");
                    dttimes = dttimes.Substring(1, dttimes.Length - 5);
                    DateTime startDate = DateTime.Parse(dttimes);
                    var InstitutionFks = Month.After("?");
                    int InstitutionFk = Convert.ToInt16(InstitutionFks);
                    var Flag = Month.After("?");

                    //  DateTime startDate = DateTime.ParseExact(dttime, "MM/dd/yyyy", new CultureInfo("en-US"));

                    if (startDate == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return RedirectToAction("invoice");
                    }

                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]);

                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);
                    var invoice = dp.getInvoice(startDate, EndDate, InstitutionFk);

                    ViewBag.Invoice = invoice;
                    if (invoice == null)
                    {
                        TempData["ErrMsg"] = "No Record Found ! ";
                    }


                    Session["invoice"] = ViewBag.Invoice;

                    data dt = new data();
                    dt.respMSg = true;

                    return Json(new { data = dt });

                }



                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Please Try Again";
                int val = 0;
                ViewBag.nInstitutions = dp.getAllInstitution();
                ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", val);
                return View();
            }
        }





















        /*  [HttpPost]
          public ActionResult ExportInvoiceToExcel(FormCollection form , string ItemList)
          {
              try
              {
                  TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                  var LoanAps = new AppLoan();
                  AppLoan Apploan = new AppLoan();
                  LoanApplication LoanAp = new LoanApplication();
                  DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();

                  int LoanApxs = 0; //int ID = 0;
                  string[] arr = ItemList.Split(',');
                  IEnumerable<int> ID = arr.Select(int.Parse);
                  string IDv = "";
                  if (arr.Length > 1)
                  {
                      for (var m = 0; m < arr.Length; m++)
                      {

                          IDv = arr[m];
                          // = IDv;
                          LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                      }

                      return RedirectToAction("invoice");
                  }
                  if (arr.Length == 1)
                  {
                      IDv = arr[0];

                      LoanApxs = _DM.UpdateLedger(IDv, LoanLdger);
                      return RedirectToAction("invoice");
                  }
                  return View();
              }
              catch(Exception ex)
              {
                  WebLog.Log(ex.Message.ToString());
                  return null;
              }
          }
          */
        [HttpPost]
        public ActionResult ExportInvoiceToExcel(LoanApplication LoanApp, TableObjects.LoanApplication LoanAP, FormCollection form, string ItemList)
        {
            try
            {

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();

                    DateTime startDate = Convert.ToDateTime(LoanAP.DateCreated);
                    if (startDate == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return RedirectToAction("invoice");
                    }
                    var ids = Convert.ToString(LoanAP.DateCreated);
                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]); //(DateTime)LoanAP.DateModified;
                    var startDates = Convert.ToString("30/01/2020");
                    var EndDates = Convert.ToString(startDate);//Convert.ToString("30/12/2019");
                    var InstitutionFk = LoanAP.institutionFk;

                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);
                    var invoice = dp.getInvoice(startDate, EndDate, InstitutionFk);
                    ViewBag.Invoice = invoice;
                    Session["AllTransaction"] = ViewBag.Invoice;
                    return View();
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public ActionResult OutstandingInvoice(FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        {
            try
            {

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    GetMenus();


                    DateTime startDate = (DateTime)LoanAP.DateCreated;// Convert.ToDateTime(form["StartDate"]);//
                    DateTime EndDate = Convert.ToDateTime(form["EndDate"]); //(DateTime)LoanAP.DateModified;
                    var startDates = Convert.ToString("30/01/2020");
                    var EndDates = Convert.ToString("30/12/2019");
                    var InstitutionFk = LoanAP.institutionFk;
                    startDate = DateTime.ParseExact(startDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                    EndDate = DateTime.ParseExact(EndDates, "dd/MM/yyyy", new CultureInfo("en-US"));
                    ViewData["Institution"] = new SelectList(dp.getAllInstitution(), "ID", "NAME", LoanAp.Institution_FK);


                    var invoice = dp.getOutstandingInvoice(startDate, EndDate, InstitutionFk);
                    ViewBag.Invoice = invoice;
                    Session["AllTransaction"] = ViewBag.Invoice;
                    // Brought This In By Myself
                    /*  DateTime TodayDate = startDate;
                      var TodayMonth = TodayDate.Month;
                      var TodayYear = TodayDate.Year;

                      DateTime DisburseDate = invoice.Select(x => x.DisburseDate).First();
                      var DisburseMonth = DisburseDate.Month;
                      var DisburseYear = DisburseDate.Year;

                      DateTime RepayDate = invoice.Select(x => x.RepaymentDate).First();
                      var RepayMonth = RepayDate.Month;
                      var RepayYear = RepayDate.Year;

                      ViewBag.InterestPricipalDue = invoice.Select(x => x.InterestPricipalDue);
                      ViewBag.NumberOFCredit = invoice.Select(x => x.Tenure);
                      var CreditCount = invoice.Where(q => q.Credit == 0 && q.RepaymentDate.Year == TodayYear && q.RepaymentDate.Month < RepayMonth ).Select(x => x.InterestPricipalDue).Count();
                    */
                    // My Code Ends Here .

                    var Count = invoice.Count();
                    int i = 0;
                    for (i = 0; i <= Count; i++)
                    {

                    }
                    var firstRec = invoice.Select(x => x.InterestPricipalDue).First();
                    var LastRec = invoice.Select(x => x.InterestPricipalDue).Last();
                }
                return View(LoanAP);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult Exportoexcel()
        {
            try
            {
                int id = 0;
               // var InvoiceList = _DR.getCheckedExcelInvoice(id);
              //  Session["AllTransaction"] = InvoiceList;
                ExportToExcel();
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult GenerateExcel(string ItemList)
        {
            try
            {
                string[] arr = ItemList.Split(',');
                IEnumerable<int> ID = arr.Select(int.Parse);
                string IDv = "";
                //DataTable dt = new DataTable("Grid");
                //dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Fullname"),
                //                            new DataColumn("Repayment Amount"),
                //                            new DataColumn("Disbursed Date"),
                //                            new DataColumn("Employee ID"),
                //                            new DataColumn("Loan Tenure")});
                if (arr.Length > 0)
                {
                    var Record = _DR.getCheckedExcelInvoice(ID);
                    
                    Session["AllTransaction"] = Record;
                    //foreach (var customer in Record)
                    //{
                    //    dt.Rows.Add(customer.Fullname, customer.InterestPricipalDue, customer.DisburseDate, customer.EmployeeID,customer.Tenure);
                    //}

                    //using (XLWorkbook wb = new XLWorkbook())
                    //{
                    //    wb.Worksheets.Add(dt);
                    //    using (MemoryStream stream = new MemoryStream())
                    //    {
                    //        wb.SaveAs(stream);
                    //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Invoice.xlsx");
                    //    }
                    //}
                    data dt = new data();
                    return Json(dt.InstiD = 0);
                }
                else
                {
                    TempData["ErrMsg"] = "Please Try Again";
                    return View();
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public void ExportToExcel()
        {
            //DataTable dt = new DataTable("GridView_Data");
            var gv = new GridView();
            gv.DataSource = Session["AllTransaction"];
            WebLog.Log("Excel Test1" + gv.Rows.Count);
            gv.DataBind();
            WebLog.Log("Excel Test2" + gv.Rows.Count);
            Response.ClearContent();
            WebLog.Log("Excel Test3" + gv.Rows.Count);
            Response.Buffer = true;
            WebLog.Log("Excel Test4" + gv.Rows.Count);
            Response.AddHeader("content-disposition", "attachment; filename=AllTransaction.xls");
            WebLog.Log("Excel Test5" + gv.Rows.Count);
            Response.ContentType = "application/ms-excel";
            WebLog.Log("Excel Test6" + gv.Rows.Count);
            Response.Charset = "";
            WebLog.Log("Excel Test7" + gv.Rows.Count);
            StringWriter objStringWriter = new StringWriter();
            WebLog.Log("Excel Test8" + gv.Rows.Count);
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            WebLog.Log("Excel Test9" + gv.Rows.Count);
            gv.RenderControl(objHtmlTextWriter);
            WebLog.Log("Excel Test10" + gv.Rows.Count);
            Response.Output.Write(objStringWriter.ToString());
            WebLog.Log("Excel Test11" + gv.Rows.Count);
            Response.Flush();
            WebLog.Log("Excel Test12" + gv.Rows.Count);
            Response.End();
            WebLog.Log("Excel Test13" + gv.Rows.Count);


        }

        //[HttpPost]
        //public ActionResult EarlyPayOff(FormCollection form, LoanApplication LoanApp, TableObjects.LoanApplication LoanAP)
        //{
        //    try
        //    {
        //        TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }
        //        else
        //        {
        //            GetMenus();
        //            //var ReferenceNum = Convert.ToString(form["RefNumber"]);
        //            var ReferenceNum = LoanAP.IdentficationNumber;
        //            var LoanLedger = _DR.GetLoanLedger(ReferenceNum);
        //            if (LoanLedger == null || LoanLedger.Count == 0)
        //            {
        //                TempData["ErrMsg"] = "Invalid Reference Number";
        //                return View();
        //            }
        //            var LastRecord = LoanLedger.Distinct().Last();
        //            if (LastRecord.orgCode == 2)
        //            {
        //                TempData["ErrMsg"] = "Loan Already Restructured";
        //                TempData["code"] = "2";
        //                ViewBag.LoansLedger = LoanLedger;
        //                return View("EarlyPayOff");
        //            }
        //            if (LastRecord.orgCode == 1)
        //            {
        //                TempData["ErrMsg"] = "Loan Repayment Complete";
        //                TempData["code"] = "2";
        //                ViewBag.LoansLedger = LoanLedger;
        //                return View("EarlyPayOff");
        //            }

        //            ViewBag.LoanAmount = LastRecord.LoanAmount;
        //            ViewBag.LoanTenure = LastRecord.LoanTenure;
        //            ViewBag.CreditSum = LoanLedger.Sum(x => x.Credit);
        //            ViewBag.CreditCount = LoanLedger.Where(q => q.Credit != 0).Select(x => x.Credit).Count();
        //            ViewBag.DebitSum = LoanLedger.Sum(x => x.Debit);
        //            ViewBag.Balance = ViewBag.DebitSum - ViewBag.CreditSum;
        //            string balance = Convert.ToString(ViewBag.Balance);
        //            ViewBag.LoansLedger = LoanLedger;
        //            var PayOff = CalPayOff(ViewBag.LoanAmount, ViewBag.LoanTenure, ViewBag.CreditCount);
        //            string PayOffs = Convert.ToString(PayOff);
        //            ViewBag.BalanceDue = PayOff;
        //            ViewBag.Balance = utilities.ConvertToCurrency(balance);
        //            ViewBag.BalanceDue = utilities.ConvertToCurrency(PayOffs);
        //            if (ViewBag.CreditCount == ViewBag.LoanTenure)
        //            {
        //                TempData["flag"] = "0";
        //            }
        //            else if (ViewBag.CreditCount < ViewBag.LoanTenure)
        //            {
        //                TempData["flag"] = "1";
        //            }
        //        }
        //        return View(LoanAP);

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}


        public float CalPayOff(float LoanAmt, int Tenure, int PaidTenure)
        {
            try
            {
                var InterestRate = _DR.GetInterestRate(Tenure);
                var MonthlyRepay = utilities.PayrollLoanCalc(LoanAmt, Tenure, InterestRate.InterestRate);
                var NewMonthlyRepay = Convert.ToDouble(MonthlyRepay);
                float MonthlyPrincipal = (float)(LoanAmt / Tenure);
                float MonthlyInterest = (float)(LoanAmt * (InterestRate.InterestRate / 100));
                if (PaidTenure == 1) { PaidTenure = 0; }
                var AmtPdToDate = (float)(NewMonthlyRepay * PaidTenure);
                var PrincipalPaid = (float)(PaidTenure * MonthlyPrincipal);
                var InterestPaid = (float)(MonthlyInterest * PaidTenure);
                var BalancePrincipal = (float)(LoanAmt - PrincipalPaid);
                var BalanceInterest = (float)(MonthlyInterest * Tenure) - InterestPaid;
                var AmountDue = (float)(0.75 * BalanceInterest) + BalancePrincipal;
                var FullAmountDue = (float)(NewMonthlyRepay * Tenure) - AmtPdToDate;
                return AmountDue;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        [HttpGet]
        public ActionResult RepayLoan()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult LoanRestructure()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult LoanRestructure(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult RepayLoan(TableObjects.LoanApplication LoanAp, FormCollection form, LoanApplication LoanApp)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                GetMenus();
                var ReferenceNum = Convert.ToString(form["RefNum"]);
                // var ReferenceNum = LoanAp.IdentficationNumber;
                // ReferenceNum = "15698469404783";
                var EarlPayOffValues = _DR.GetLoanLedger(ReferenceNum);
                var Lastvalue = EarlPayOffValues.Distinct().Last();
                var tenure = Lastvalue.LoanTenure;
                var RepayAmount = Lastvalue.Debit;
                var LoanLedgerUpdate = _DM.LoanRepayment(tenure, RepayAmount, ReferenceNum);
                if (LoanLedgerUpdate == 0)
                {
                    TempData["SucMsg"] = "Loan Repayment Succesfull";
                }
                return View("EarlyPayOff");

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]

        public ActionResult CheckStudentLoanStatus(FormCollection form, LoanViewModel lvm)
        {
            LoanApplication lonobj = new LoanApplication();
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var ApplicationFk = Convert.ToString(form["RefNumber"]);
                ViewBag.Data = _DR.CheckLoanStatus(ApplicationFk);
                Session["AllTransaction"] = ViewBag.Data;

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult CheckEligibility()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            GetMenus();
            ViewBag.Institution = uvdb.Institutions.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CheckEligibility(LoanViewModel lvm, FormCollection form, DataAccessLayerT.Classes.AppLoans Apploan)
        {

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                StudentRecord StdRec = new StudentRecord();
                ViewBag.Institution = uvdb.Institutions.ToList();
                StdRec.Institution_FK = Convert.ToInt32(form["CheckEligibility"]);
                StdRec.MatriculationNumber = lvm.StudentRecord.MatriculationNumber;
                StdRec.PhoneNumber = lvm.StudentRecord.PhoneNumber;
                Apploan = _DR.CheckInstitution(StdRec);

            }
            GetMenus();
            return View("CheckEligibility2", Apploan);
        }

        [HttpGet]
        public ActionResult CheckEligibility2(LoanViewModel lvm)
        {
            GetMenus();
            return View();
        }

        [HttpGet]
        public ActionResult BVNValidation(string code, DataAccessLayerT.Helper.BVNC cls)
        {
            try
            {

                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                // TempData["SucMsg"] = "";
                //TempData["ErrMsg"] = "";
                bool valid = ValidateUrl();
                if (valid == false)
                {
                    return RedirectToAction("Index", "Admin");
                }
                // var cls = new DataAccessLayerT.Helper.BVNC();
                cls.BVN = code;
                GetMenus();
                return View(cls);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult BVNValidation(FormCollection form, Helper.BVNC bvnc)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            string bvn = Convert.ToString(form["BVN"]) == null ? "" : Convert.ToString(form["BVN"]);

            //Helper helper = new Helper();
            bvnc = Helper.BVNValidationResps(bvn);

            //Insert BVN details here

            DataWriter.SaveBVNDetails(bvnc);

            if (bvnc.respCode == "00")
            {
                GetMenus();
                return View("BVNValidation", bvnc);

            }
            else
            {
                // ViewBag.Data = bvnc.errormessage;
                TempData["ErrMsg"] = bvnc.errormessage;
                GetMenus();
                return View("BVNValidation");
            }

        }




        public ActionResult SetupMandate()

        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            GetMenus();
            var BanlList = _DR.GetBanks();
            ViewBag.BankList = BanlList;

            return View();
        }
        [HttpPost]
        public ActionResult SetupMandate(FormCollection form)
        {

            DataReader dr = new DataReader();
            try
            {

                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();
                    jobj.payerName = Convert.ToString(form["payerName"]);
                    jobj.payerEmail = Convert.ToString(form["payerEmail"]);
                    jobj.payerPhone = Convert.ToString(form["payerPhone"]);
                    jobj.payerBankCode = Convert.ToString(form["payerBankCode"]);
                    jobj.payerAccount = Convert.ToString(form["payerAccount"]);
                    jobj.amount = Convert.ToString(form["amount"]);
                    jobj.startDate = Convert.ToString(form["StartDate"]);
                    jobj.endDate = Convert.ToString(form["EndDate"]);
                    jobj.mandateType = Convert.ToString(form["MandateType"]);
                    jobj.maxNoOfDebits = Convert.ToString(form["maxNoOfDebits"]);
                    var json = jobj.ToString();
                    //var url = ConfigurationManager.AppSettings["MandateSetUpTest"];
                    var mandateSetupurl = ConfigurationManager.AppSettings["mandateSetupurl"];
                    //string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    var data = MyUtilities.DoRemitaPost(mandateSetupurl, json);

                    data = data.Replace("jsonp (", "");
                    data = data.Replace("})", "}");
                    dynamic myJobj = JObject.Parse(data);
                    WebLog.Log("myJobj" + myJobj);
                    if (myJobj.respCode == "00")
                    {
                        WebLog.Log("myJobj.respCode" + myJobj.respCode);
                        TempData["RefNum"] = myJobj.remitaTransRef;
                        WebLog.Log("myJobj.remitaTransRef" + myJobj.remitaTransRef);
                        TempData["requestID"] = myJobj.requestID;
                        WebLog.Log("myJobj.requestID" + myJobj.requestID);
                        TempData["mandateID"] = myJobj.mandateID;
                        WebLog.Log("myJobj.mandateID" + myJobj.mandateID);
                        TempData["Msg"] = myJobj.respDescription;
                        WebLog.Log("myJobj.respDescription" + myJobj.respDescription);
                        return View("OTPValidation");
                    }
                    else
                    {
                        WebLog.Log("myJobj.respCode1" + myJobj.respCode);
                        TempData["Msg"] = myJobj.respDescription;
                        WebLog.Log("myJobj.respDescription1" + myJobj.respDescription);
                        return RedirectToAction("SetupMandate");
                    }

                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

            //  return View("OTPValidation");
        }



        [HttpGet]
        public ActionResult OTPValidation()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            GetMenus();
            return View();
        }
        [HttpPost]
        public ActionResult OTPValidation(FormCollection form)
        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();
                    jobj.otp = Convert.ToString(form["otp"]);
                    jobj.cardNo = Convert.ToString(form["cardNo"]);
                    jobj.remitaRef = Convert.ToString(form["RefNum"]);
                    var json = jobj.ToString();
                    var url = ConfigurationManager.AppSettings["OTPvalidationTest"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);

                    data = data.Replace("jsonp (", "");
                    data = data.Replace("})", "}");
                    dynamic myJobj = JObject.Parse(data);
                    if (myJobj.mandateID == null || myJobj.mandateID == "")
                    {
                        TempData["ErrMsg"] = "Error Activating Mandate";
                        TempData["MandateID"] = "none";
                    }
                    if (myJobj.respCode == "00" && myJobj.mandateID != null)
                    {
                        TempData["SucMsg"] = "Mandate Activated Succesfuly";
                        TempData["MandateID"] = myJobj.mandateID;

                    }
                    else if (myJobj.respCode != "00")
                    {
                        TempData["ErrMsg"] = "Error Activating Mandate";
                        TempData["MandateID"] = myJobj.status;
                    }



                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
            return View();
        }

        [HttpGet]
        public ActionResult DebitInstruction()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            GetMenus();
            var BanlList = _DR.GetBanks();
            ViewBag.BankList = BanlList;
            return View();
        }

        [HttpPost]
        public ActionResult DebitInstruction(FormCollection form)
        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();
                    jobj.totalAmount = Convert.ToString(form["totalAmount"]);
                    jobj.mandateId = Convert.ToString(form["MandateID"]);
                    jobj.fundingAccount = Convert.ToString(form["fundingAccount"]);
                    jobj.fundingBankCode = Convert.ToString(form["fundingBankCode"]);
                    var json = jobj.ToString();
                    var url = ConfigurationManager.AppSettings["DebitInstructionTest"];

                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    dynamic jObj = JObject.Parse(data);
                    if (jObj.respCode == "00")
                    {
                        TempData["SucMsg"] = jObj.respDescription;
                        TempData["requestID"] = jObj.requestID;
                        TempData["mandateId"] = jObj.mandateID;
                        TempData["Amount"] = jObj.RRR;
                        TempData["TransRef"] = jObj.transactionRef;
                        TempData["Status"] = jObj.respDescription;
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Debit Instruction Not Succesful " + jObj.respDescription;
                        TempData["requestID"] = null;
                        TempData["mandateId"] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }

            return RedirectToAction("DebitInstruction");
        }


        [HttpPost]
        public ActionResult UpdateLoanApproval(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var RefNum = Convert.ToString(form["RefNum"]);
                var LoanAmount = Convert.ToDouble(form["LoanAmount"]);
                var LoanTenure = Convert.ToInt16(form["LoanTenure"]);
                var INST = Convert.ToInt16(form["INST"]);
                var resp = _DM.UpdateLoanTenureAndInterestRate(RefNum, LoanAmount, LoanTenure, INST);
                return View("LoanApprovalUpdate");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult CheckEmpName(string code)

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                UvlotEntities uvdb = new UvlotEntities();
                var isValid = dp.validateEmpName(code);
                if (isValid != null)
                {
                    return Json(isValid);
                }
                else
                {
                    return Json(isValid = "none");
                }
                //return Json(new { Success = "true", Data = isValid });

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult DDStatus(FormCollection form, string Refid)
        {
            try
            {
                var resp = DebitStatus(form, Refid);

                return View("RemitaTrans");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public ActionResult DebitStatus()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            GetMenus();
            return View();
        }

        [HttpPost]
        public ActionResult DebitStatus(FormCollection form, string Refid)
        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.Data = Session["recs"];
                    dynamic jobj = new JObject();

                    if (Refid != null || Refid != "")
                    {
                        var rec = _DR.getMandate(Refid);
                        jobj.mandateId = rec.mandateID;//Convert.ToString(rec.MandateId);
                        jobj.requestId = rec.InitialDebitRequestId; //Convert.ToString(rec.requestId);
                    }
                    else
                    {
                        jobj.mandateId = Convert.ToString(form["mandateId"]);
                        jobj.requestId = Convert.ToString(form["requestId"]);
                    }

                    //jobj.mandateId = Convert.ToString(form["mandateId"]);
                    //jobj.requestId = Convert.ToString(form["requestID"]);
                    var json = jobj.ToString();
                    //var url = ConfigurationManager.AppSettings["DebitStatusTest"];
                    var url = ConfigurationManager.AppSettings["DebitStatusLive"];
                    
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    if (data == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return View();
                    }
                    dynamic jObj = JObject.Parse(data);

                    if (jObj.respCode != "" || jObj.respCode != null)
                    {
                        TempData["SucMsg"] = jObj.respDescription;
                        TempData["value1"] = jObj.amount;
                        TempData["value2"] = jObj.RRR;
                        TempData["status"] = jObj.description;
                        TempData["value5"] = jObj.respDescription;
                    }
                    else
                    {

                        TempData["ErrMsg"] = jObj.respDescription;
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
            return View();
        }

        public ActionResult InitialDebit(FormCollection form, string Refid)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.Data = Session["recs"];
                    dynamic jobj = new JObject();


                        var rec = _DR.getMandate(Refid);
                       
                    jobj.totalAmount = ConfigurationManager.AppSettings["InitialDebit"];
                    jobj.mandateId = Convert.ToString(rec.mandateID);
                    jobj.refNumber = Convert.ToString(rec.RefNum);
                    jobj.fundingBankCode = Convert.ToString(rec.BankCode);
                    jobj.fundingAccount = Convert.ToString(rec.BankAcct);
                    var json = jobj.ToString();
                   // var url = ConfigurationManager.AppSettings["DirectDebit"];
                    var url = ConfigurationManager.AppSettings["DirectDebitLive"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    if (data == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return View("RemitaTrans");
                    }
                    dynamic jObjs = JObject.Parse(data);

                    if (jObjs.respCode != null || jObjs.respCode != "")
                    {
                        TempData["SucMsg"] = jObjs.respDescription;
                        TempData["value1"] = jObjs.amount;
                        TempData["value2"] = jObjs.RRR;
                        TempData["status"] = jObjs.respDescription;
                        TempData["value5"] = jObjs.respDescription;

                        if (jObjs.respCode == "00")
                        {
                            var reqID = jObjs.requestID;
                            _DM.UpdateRequestIDFromInitialDebit(Refid, reqID);
                        }
                    }
                    else
                    {

                        TempData["ErrMsg"] = jObjs.respDescription;
                    }
                }
               

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            return View("RemitaTrans");
        }
        
        [HttpGet]
        public ActionResult CheckMandateStatus()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            GetMenus();
            return View();
        }

        [HttpPost]
        public ActionResult GetDebitInstructionStatus(string RefNum)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult TransactLog()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                GetMenus();
                var Data = _DR.RemitaTransactLog();
                //ViewBag.RemitaLog = Data.Table;
                DataTable dt = Data.Table;
                // mandateSetup ms = new mandateSetup();
                List<mandateSetup> cdList = new List<mandateSetup>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mandateSetup ms = new mandateSetup();
                    //ms.Amount = Convert.ToString(dt.Columns[""]);
                    ms.FirstName = Convert.ToString(dt.Rows[i][2].ToString());
                    // ms.LastName = Convert.ToString(dt.Columns[""]);
                    ms.MandateID = Convert.ToString(dt.Rows[i][3].ToString());
                    ms.Phone = Convert.ToString(dt.Rows[i][5].ToString());
                    ms.RequestID = Convert.ToString(dt.Rows[i][7].ToString());
                    ms.Email = Convert.ToString(dt.Rows[i][4].ToString());
                    ms.RemitaTransRef = Convert.ToString(dt.Rows[i][1]);
                    ms.startDate = Convert.ToString(dt.Rows[i][6].ToString());
                    ms.ActivationDate = Convert.ToString(dt.Rows[i][8].ToString());
                    cdList.Add(ms);
                }
                ViewBag.RemitaLog = cdList;

                return View();
                //   return View(dt);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult DebitInstructions()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                GetMenus();
                var Data = _DR.DirectDebitTransactLog();

                DataTable dt = Data.Table;

                List<mandateSetup> cdList = new List<mandateSetup>();
                List<string> mc = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    mandateSetup ms = new mandateSetup();
                    ms.MandateID = Convert.ToString(dt.Rows[i][2].ToString());

                    ms.RequestID = Convert.ToString(dt.Rows[i][4].ToString());

                    ms.RemitaTransRef = Convert.ToString(dt.Rows[i][1]);
                    ms.RRR = Convert.ToString(dt.Rows[i][3].ToString());
                    ms.ActivationDate = Convert.ToString(dt.Rows[i][5].ToString());
                    // ms.value1 = Convert.ToString(dt.Rows[i][6].ToString());
                    ms.value2 = Convert.ToString(dt.Rows[i][7].ToString());
                    cdList.Add(ms);
                }
                ViewBag.DebitInstructions = cdList;

                return View();
                //   return View(dt);
                // return View(dt);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult UpdateLed(List<string> DebPaid, List<string> date, string Refid, FormCollection form)
        {
            try
            {
                // var resp = CheckMandateStatus(form, Refid);
                // ViewBag.Data = Session["recs"];
                var amount = Convert.ToDouble(form["DebPaid"]);
                var dates = form["date"];
                return View("");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult MandateStatus(string Refid,FormCollection form)
        {
            try
            {
                var resp = CheckMandateStatus(form,Refid);
                ViewBag.Data = Session["recs"];
                return View("RemitaTrans");
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult CheckMandateStatus(FormCollection form,string refNum)
        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();

                    if (refNum != null || refNum != "")
                    {
                        var rec = _DR.getMandate(refNum);
                        jobj.mandateId = rec.mandateID;//"160386148950";//Convert.ToString(rec.MandateId);
                        jobj.requestId = rec.PatnerReference;//"1583765067792";//Convert.ToString(rec.requestId);
                    }
                    else
                    {
                        jobj.mandateId = Convert.ToString(form["mandateId"]);
                        jobj.requestId = Convert.ToString(form["requestId"]);
                    }
                    var json = jobj.ToString();
                    //  var url = ConfigurationManager.AppSettings["MandateStatusLive"];
                    var url = ConfigurationManager.AppSettings["MandateStatusLive"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    if(data == null)
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        return View();
                    }
                    dynamic jObj = JObject.Parse(data);
                   // if (jObj.respCode == "00")
                   if(jObj.respCode != null || jObj.respCode != "" || jObj.respCode != "00")
                    {
                        TempData["SucMsg"] = "Request Succesful";
                        TempData["value1"] = jObj.Startdate;
                        TempData["value2"] = jObj.Enddate;
                        TempData["value3"] = jObj.RegistrationDate;
                        //TempData["value4"] = jObj.isActive;
                        TempData["status"] = jObj.description;
                        TempData["value5"] = jObj.respDescription;
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                        // TempData["value1"] = jObj.Startdate;
                        //TempData["value2"] = jObj.Enddate;
                        //TempData["value3"] = jObj.RegistrationDate;
                        //TempData["value4"] = jObj.isActive;
                        //TempData["value5"] = jObj.respDescription;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrMsg"] = "Please Try Again";
                WebLog.Log(ex.Message.ToString());
              
            }
            return View();

        }

        [HttpGet]
        public ActionResult MS(string Refid)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var requestid = Refid.After("}");
                var Mandateid = Refid.Before("}");
                dynamic jobj = new JObject();

                jobj.mandateId = Mandateid;
                jobj.requestId = requestid;
                //var json = jobj.ToString();

                dynamic data = MandateStat(jobj);
                // dynamic jObj = JObject.Parse(data);
                TempData["MNStat"] = "00";
                if (data.respCode == "00")
                {
                    return RedirectToAction("Status");
                }

                return RedirectToAction("Status");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult DI(string Refid)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var Mandateid = Refid.Before("}");
                var requestid = Refid.After("}");
                dynamic jobj = new JObject();

                jobj.mandateId = Mandateid;
                jobj.requestId = requestid;
                //var json = jobj.ToString();

                dynamic data = DebitStat(jobj);
                // dynamic jObj = JObject.Parse(data);
                TempData["DBStat"] = "00";
                if (data.respCode == "04")
                {
                    return RedirectToAction("Status");
                }

                return RedirectToAction("Status");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        //Amaka region

        // This is For Debit Status
        public dynamic DebitStat(dynamic json)
        {
            try
            {

                json = json.ToString();
                var url = ConfigurationManager.AppSettings["DebitStatusTest"];
                string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                dynamic jObj = JObject.Parse(data);
                if (jObj.respCode == "04")
                {
                    TempData["SucMsg"] = jObj.respDescription;
                    TempData["requestID"] = jObj.requestID;
                    TempData["mandateId"] = jObj.mandateID;
                    TempData["Amount"] = jObj.amount;
                    TempData["TransRef"] = jObj.transactionRef;
                    TempData["Status"] = jObj.respDescription;
                    TempData["description"] = jObj.description;
                    TempData["RRR"] = jObj.RRR;


                }
                else
                {
                    TempData["ErrMsg"] = "Debit Instruction Not Succesful " + jObj.respDescription;
                    TempData["requestID"] = null;
                    TempData["mandateId"] = null;
                }
                return jObj;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        // This is For Mandate Status
        public dynamic MandateStat(dynamic jsons)
        {
            try
            {
                var json = jsons.ToString();
                var url = ConfigurationManager.AppSettings["MandateStatusTest"];
                string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                dynamic jObj = JObject.Parse(data);
                if (jObj.respCode == "00")
                {
                    TempData["SucMsg"] = "Request Succesful";
                    TempData["value1"] = jObj.Startdate;
                    TempData["value2"] = jObj.Enddate;
                    TempData["value3"] = jObj.RegistrationDate;
                    TempData["value4"] = jObj.isActive;
                    TempData["value5"] = jObj.respDescription;
                }
                else
                {
                    TempData["ErrMsg"] = "Please Try Again";
                    // TempData["value1"] = jObj.Startdate;
                    //TempData["value2"] = jObj.Enddate;
                    //TempData["value3"] = jObj.RegistrationDate;
                    //TempData["value4"] = jObj.isActive;
                    //TempData["value5"] = jObj.respDescription;
                }
                return jObj;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public ActionResult Status()
        {
            try
            {
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        // This is the Chart Side For Dashboard
        [HttpGet]
        public ActionResult NumberOFLoanAwaitingRecomedtn(int InstFK,int Appstat)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                //var record = _DR.GetChartReports(InstFK,Appstat);
               // var record = _DR.GetChartReport(InstFK, Appstat);


                //List<Summary> lstSummary = new List<Summary>();
                //foreach (DataRow dr in GetVehicleSummary().Rows)
                //{
                //    Summary summary = new Summary();
                //    summary.Value = dr[0].ToString().Trim();
                //    summary.Item = Convert.ToDouble(dr[1]);
                //    lstSummary.Add(summary);
                //}




                dynamic data = new JObject();
                return Json(data.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult NumberOFLoanAwaitingApproval(int InstFK, int AppStat)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                var record = _DR.ChartForApproveLoans(AppStat,InstFK);

                dynamic data = new JObject();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult NumberOFLoanAwaitingDisbursemnt(int InstFK, int AppStat)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                var record = _DR.ChartForApproveLoans(AppStat, InstFK);

                dynamic data = new JObject();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult LastThirtyDaysLoans(int InstFK, int AppStat)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                DateTime dt = MyUtility.getCurrentLocalDateTime();
                DateTime dt1 = dt.AddDays(-30);
                var record = _DR.LastThirtyDaysLoan(dt,dt1,AppStat,InstFK);
                dynamic data = new JObject();
                return Json(new { data });
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }

        public ActionResult LoanAmountOwed(int instFk)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var count = _DR.GetCountOfLoanOwed(instFk);
                var result = _DR.GetLoanAmountowed(instFk);
                dynamic data = new JObject();
                return Json(new { data });
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            
        }

        // Chart Side Ends Here 



        [HttpGet]
        public ActionResult OfferLetter(string Refid)
        {
            try
            {
                //var Refid = Convert.ToString(TempData["RefNum"]);
                //  Refid = "15697824915332";
                // Refid = Convert.ToString(TempData["RefNum"]);
                MyUtility utilities = new MyUtility();
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("Acknowledgement", new { @Refid = Refid });
                }
                var Menus = dp.getOfferDetailsAfterDisbursement(Refid);
                if (Menus == null)
                {
                    return RedirectToAction("ApproveLoan", "Admin");
                }
                var RepayValues = Menus.Distinct().GroupBy(x => x.LoanAmount);
                ViewBag.LoanRepayment = Menus.ToList();
                DataAccessLayerT.DataManager.AppLoan AppLoan = new DataAccessLayerT.DataManager.AppLoan();
                var DistinctVal = Menus.Distinct().Last();
                AppLoan.Firstname = DistinctVal.Firstname;
                AppLoan.Surname = DistinctVal.Surname;
                AppLoan.Title = DistinctVal.Title;
                AppLoan.ApplicationDate = DistinctVal.ApplicationDate.Date;
                AppLoan.GuarSurname = DistinctVal.GuarSurname;
                AppLoan.GuarOthernames = DistinctVal.GuarOthernames;
                AppLoan.GuarPhone = DistinctVal.GuarPhone;
                AppLoan.GuarRelationship = DistinctVal.GuarRelationship;
                AppLoan.GuarEmail = DistinctVal.GuarEmail;
                int year = AppLoan.ApplicationDate.Year;
                int month = AppLoan.ApplicationDate.Month;
                int day = AppLoan.ApplicationDate.Day;
                string days = AppLoan.ApplicationDate.ToLongDateString();
                var Datas = string.Format("{0}-{1}-{2}", days, month, day, year);
                AppLoan.ApplicationDatevalue = days;

                AppLoan.ContactAddress = DistinctVal.ContactAddress;
                AppLoan.PhoneNumber = DistinctVal.PhoneNumber;
                AppLoan.LoanAmount = DistinctVal.LoanAmount;
                AppLoan.ConvertedLoanAmt = MyUtility.ConvertToCurrency(AppLoan.LoanAmount.ToString());
                double inrate = dp.getInterestRate(DistinctVal.LoanTenure);
                AppLoan.interestRate = (float)inrate;
                int amtWordint = Convert.ToInt32(AppLoan.LoanAmount);

                var numberText = new NumberText.NumberText();
                AppLoan.AmountInWords = numberText.ToText(amtWordint) + " Niara";

                AppLoan.LoanTenure = DistinctVal.LoanTenure;
                AppLoan.OutstandingBalance = DistinctVal.OutstandingBalance;
               


                return View(AppLoan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }





        protected void ProcessResponse()
        {
            var trxRef = Request.QueryString["trxRef"];
            var trxToken = Request.QueryString["trxToken"];
            var SecretKey = ConfigurationManager.AppSettings["SecretKey"];
            TransactionalInformation trans;
            try
            {
                dynamic obj = new JObject();
                obj.trxRef = trxRef.Trim();
                obj.trxToken = trxToken.Trim();
                var json = obj.ToString();
                // string url = "http://www.paytrx.org/api/web/GetTransactionRecord";
                // string url = "http://paytrx.net/api/web/GetTransactionRecord";
                string url = ConfigurationManager.AppSettings["FlutterWave_Requery"];
                //re-query payment gateway for the transaction.
                var reqData = "{\"TrxRef\":\"" + trxRef + "\"" + ",\"TrxToken\":\"" + trxToken + "\"}";
                //  var url = ConfigurationManager.AppSettings["FlutterWave_Requery"];
                //var url = FlutteWaveEngine.FlutterWaveRequery;
                string serverResponse = string.Empty;

                var plainText = (trxRef + trxToken + SecretKey);

                var hash = new CryptographyManager().ComputeHash(plainText, HashName.SHA256);
                var isPaid = PaymentRequery(reqData, url, hash, out serverResponse, out trans);
                var jvalue = JObject.Parse(serverResponse);

                dynamic jvalues = JObject.Parse(serverResponse);
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() == "00" && isPaid == true)
                {

                   // var id = UpdateTransaction(jvalues);
                    //if (id != null)
                    //{
                    //    string refNum = jvalues?.data?.trxref;
                    //    var PayRes = "";
                    //    if (refNum.StartsWith("DSTV") || refNum.StartsWith("GOTV"))
                    //    {
                    //        PayRes = payDstvGotv(refNum);
                    //    }
                    //    else
                    //    {
                    //        PayRes = paySubscribtion(refNum);
                    //    }
                    //    dynamic Pay = JObject.Parse(PayRes);
                    //    if (Pay?.returnCode == 0)
                    //    {
                    //        InsertCustomerTransaction(refNum, PayRes);
                            
                    //        Receipt(Pay);
                    //        TempData["SucMsg"] = "Transaction Succesful !";
                    //    }
                    //    else if (Pay?.returnCode != 0)
                    //    {
                          
                    //        Receipt(Pay);
                    //        TempData["SucMsg"] = "Successful " + Pay?.respDescription;

                    //    }
                    //}
                }
                else
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() != "00" || isPaid == false)
                {
                    TempData["ErrMsg"] = "Transaction Failed Please Try Again ! " + jvalues?.data?.resp_desc;
                
                    //var id = UpdateTransaction(jvalues);
                    //if (id != null)
                    //{
                    //    string refNum = jvalues?.data?.trxref;
                    //    dynamic payObjx = new JObject();
                    //    payObjx.returnCode = jvalues?.data?.resp_code;
                    //    payObjx.returnMsg = jvalues?.data?.resp_desc;
                    //    payObjx.tranNum = jvalues?.data?.trxref;
                      
                    //    Receipt(payObjx);
                    //    TempData["ErrMsg"] = "Transaction Failed Please Try Again ! " + jvalues?.data?.resp_desc;


                    //}

                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
               
            }
        }


        public string DisburseCash(dynamic cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();
             
                string seckey = ConfigurationManager.AppSettings["DiburseSeckey"];

                obj.callbackurl = ConfigurationManager.AppSettings["DisburseCllbackUrl"];
                //obj.bankcode = cusObj.bank_codes;
                obj.bankcode = cusObj.flutterwaveBanCode;
                obj.account_number = cusObj.AccountNumber;
                obj.amount = Convert.ToString(cusObj.LoanAmount);
                obj.narration = ConfigurationManager.AppSettings["Narration"] + "" + obj.amount + "For" + cusObj.AccountName;
                obj.currency =  ConfigurationManager.AppSettings["Currency"];
                
                obj.trxRef = cusObj.LoanRefNumber;
               
               
                obj.beneficiary_name = cusObj.AccountName;
                var builder = new StringBuilder();
                builder.Append(seckey).Append(obj.callbackurl);

                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);

                // For The Signature
              

                obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("BuyPayLoad" + json);
                string callbackUrl = obj.callbackurl;
                string hashval = hash.ToString();
                var PostUrl = ConfigurationManager.AppSettings["DisburseCash"];
                WebLog.Log("BuyPayLoad" + json);
                var data = MyUtilities.DoPosts(json,$"{PostUrl}",seckey,callbackUrl,hashval);
                

                WebLog.Log("BuyPayLoad" + json);
                WebLog.Log("PostUrl" + PostUrl);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string GetBankList()
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();

                string seckey = ConfigurationManager.AppSettings["DiburseSeckey"];
                var PostUrl = ConfigurationManager.AppSettings["GetBanks"];
                
                var data = MyUtilities.DoGet($"{PostUrl}", seckey);
                WebLog.Log("PostUrl" + PostUrl);

                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult GetPaymentResponse(JObject value)
        {
            try
            {
                //var resp =  DisburseCash();
                ProcessResponse();
                //Classes.Paytv.Receipt Receipt = new Classes.Paytv.Receipt();
                dynamic RecieptObj = new JObject();
                if (TempData["ErrMsg"] != null)
                {
                    // TempData["ErrMsg"] = "Transaction Not Succesfull. Please Contact Our Customer Care Representative ";
                }
                string RefNum = Request.QueryString["trxRef"];

               // var RecieptVal = _dr.GetCustReceipt(RefNum);
                // I Added This today April 17
               
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        private static HttpWebRequest ReturnHeaderParameters(string method, string url, string hashValue, bool isPayEngine)
        {

            //if (req == null) return null;
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method.ToUpper();
            req.Timeout = Timeout.Infinite;
            req.KeepAlive = true;

            if (isPayEngine)
            {
                //var PublicKey = "pk_8LHVMWLJDXAV8CPEZB9M2YGQ7CMMTWMKZGTTQCKATSFXZNJGGSES0GV9";
                req.Headers.Add($"PaelytAuth:{hashValue}");
                // req.Headers.Add($"PaelytAuth:{PublicKey}");
                req.Headers.Add($"PublicKey: {ConfigurationManager.AppSettings["PublicKey"]}");

                return req;
            }


            return req;
        }

        public static bool PaymentRequery(string postData, string url, string hashValue, out string serverResponse, out TransactionalInformation trans)
        {
            trans = new TransactionalInformation();
            serverResponse = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string responseString;
            try
            {
                _req = ReturnHeaderParameters("POST", url, hashValue, true);

                var data = Encoding.ASCII.GetBytes(postData);
                _req.ContentType = "application/json;charset=UTF-8";
                _req.ContentLength = data.Length;
                HttpStatusCode statusCode;
                using (var stream = _req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    using (_response = (HttpWebResponse)_req.GetResponse())
                    {
                        statusCode = _response.StatusCode;
                        responseString = new StreamReader(_response?.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                        serverResponse = responseString;
                    }
                }
                var jvalue = JObject.Parse(responseString);
                //value to check. status code 200
                var httpStatusCode = (int)statusCode;
                if (httpStatusCode == 200)
                {
                    trans.ReturnMessage.Add((string)jvalue["data"]["resp_desc"]);
                    return trans.IsAuthenicated = ((string)jvalue["data"]["resp_code"]).Equals("00");
                }
            }
            //to read the body of the server _response when status code != 200
            catch (WebException exec)
            {
                var response = (HttpWebResponse)exec.Response;
                var dataStream = response.GetResponseStream();
                trans.ReturnMessage.Add(exec.Message);
                if (dataStream == null) return trans.IsAuthenicated;
                using (var tReader = new StreamReader(dataStream))
                {
                    responseString = tReader.ReadToEnd();
                }
                //trans.ReturnMessage.Add(exec.Message);
                var jvalue = JObject.Parse(responseString);
                // ErrorMessage error = new JavaScriptSerializer().Deserialize<ErrorMessage>(responseString);
                trans.ReturnMessage.Add((string)jvalue["data"]["resp_desc"]);
                trans.IsAuthenicated = false;
            }
            catch (Exception ex)
            {
                trans.ReturnMessage.Add(ex.Message);
                trans.IsAuthenicated = false;
            }
            return trans.IsAuthenicated;
        }




        // Amaka Cashout 

        [HttpGet]
        public ActionResult ComissionCashout()
        {
            try
            {
                TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var userFk = _DR.getUserID(appUser);
                GetMenus();
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                int val = 0;
                ViewData["nBanks"] = new SelectList(dp.GetBanks(), "FlutterWaveBankCode", "NAME", val);
                var Records = _DR.getCommisioRecords(userFk);
                var resp = Comisionsum(Records);
                ViewBag.Records = Records;
                double TotalRepayment = Convert.ToDouble(resp);
                TotalRepayment = Math.Round(TotalRepayment);
                ViewBag.Balance = TotalRepayment;
                TempData["Records"] = Records;
                TempData["Amount"] = TotalRepayment;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        private double Comisionsum(List<LoansLedger> Record)
        {
            try
            {
                List<string> AmountList = new List<string>();
                double Total = 0;
                {
                    for (int i = 0; i < Record.Count; i++)
                    {

                        Total += Convert.ToDouble(Record[i].Debit) - Convert.ToDouble(Record[i].Credit);
                    }
                }
                return Total;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        [HttpPost]
        public ActionResult ComissionCashout(FormCollection form, UvlotApplication.Classes.TableObjects.LoanApplication LP)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var userFk = _DR.getUserID(appUser);
                GetMenus();
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                double TotAmt = Convert.ToDouble(TempData["Amount"]);
                double amt = Convert.ToDouble(form["TotAmt"]);
                double Casoutamt = Convert.ToDouble(form["Amt"]); ;
                if (amt > TotAmt || Casoutamt > TotAmt)
                {
                    TempData["Errmsg"] = "Invalid Amount";
                    return RedirectToAction("ComissionCashout");
                }
                double RealAmt = TotAmt - Casoutamt;
                double mode = 0; double div = 0;
                var num = someCalculations(Casoutamt, out mode, out div);
                div = Math.Truncate(div);
                //LoansLedger Ln = new LoansLedger();
                List<LoansLedger> Ln = new List<LoansLedger>();
                
                /*
                Ln = _DR.getCommisioRecords(userFk);
                
                if(Ln.Count > 0)
                {
                  int  RecordCount = Ln.Count;
                }*/

                dynamic CusObj = new JObject();
                CusObj.bankcode = LP.BankCode;
                CusObj.account_number = LP.AccountNumber;
                CusObj.amount = Casoutamt;
                CusObj.beneficiary_name = LP.AccountName;
                CusObj.narration = "Commission Earned:" + "" + CusObj.amount + "For" + CusObj.beneficiary_name;
                CusObj.currency = ConfigurationManager.AppSettings["Currency"];

                CusObj.trxRef = MyUtility.GenerateRefNo();
                var resp = DisburseCash(CusObj);
                if (resp != null)
                {
                    int res = 0;
                    int vals = 0;
                    dynamic result = JObject.Parse(resp);
                    if (result?.status == "true")
                   {
                    if (mode > 0)
                   {
                             div = div + 1;
                             vals = Convert.ToInt16(div);
                             Ln = _DR.getCommisioRecordsExact(userFk,vals);
                             res = _DM.UpdateNYSCLoanReferalLeder(Ln, mode, div);
                        
                   }
                   if (mode == 0)
                   {
                            vals = Convert.ToInt16(div);
                            Ln = _DR.getCommisioRecordsExact(userFk, vals);
                            res = _DM.UpdateNYSCLoanReferalLeder(Ln, mode, div);
                   }

                }


                }

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public double someCalculations(double realAmt,out double Mode, out double div)
        {
            try
            {
                Mode = realAmt % 100;
                div = realAmt / 100;

                return 0;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                Mode = 0;
                div = 0;
                return 0;
            }
        }

        [HttpGet]
        public ActionResult Csahout()
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
        [HttpGet]
        public ActionResult UpdateLedgerNew()
        {
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();
            GetMenus();
            var appUser = user;
            if (appUser == null)
            {
                return RedirectToAction("/", "Home");
            }
            List<InsuranceController.UserModel> users = InsuranceController.UserModel.getUsers();
            return View(users);
        }

        [HttpGet]
        public ActionResult UpdateLedgerr()
        {
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();
            GetMenus();
            var appUser = user;
            if (appUser == null)
            {
                return RedirectToAction("/", "Home");
            }
            UvlotEntities uvdb = new UvlotEntities();
            List<LoanLedger> customers = _DR.GetFirstThree(); ;

            return View(customers.ToList());
        }

        [HttpPost]
        public ActionResult UpdateLedgerr(LoanLedger LoanAP)
        {
            //GetMenus();
            //UvlotEntities uvdb = new UvlotEntities();
            //List<LoanLedger> customers = uvdb.LoanLedgers.ToList();

            List<LoanLedger> customers = new List<LoanLedger>();
            List<LoanLedger> customer = new List<LoanLedger>();
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();

            var appUser = user;
            if (appUser == null)
            {
                return RedirectToAction("/", "Home");
            }
            else
            {
                GetMenus();

                var ReferenceNum = LoanAP.RefNumber;
                //var LoanLedger = _DR.GetLoanLedgerr(ReferenceNum);
                // customers = _DR.GetLoanLedgerr(ReferenceNum);
                 customer = _DR.getNewCustomer(ReferenceNum);
              //  if (customers == null || customers.Count == 0)
                    if (customer == null || customer.Count == 0)
                    {
                    TempData["ErrMsg"] = "Invalid Reference Number";
                    //customers = _DR.GetFirstThree(); ;
                    customer = _DR.GetFirstThree();
                    // return View(customers.ToList());
                    return View(customer.ToList());
                }
                // ViewBag.LoansLedger = customers;
                ViewBag.LoansLedger = customer;
            }

            return View(customer.ToList());
            //return View(customers.ToList());
        }

        [HttpPost]
        public ActionResult UpdateLedg(LoanLedger customer)
        {
            using (UvlotEntities entities = new UvlotEntities())
            {
                LoanLedger updatedCustomer = (from c in entities.LoanLedgers
                                            where c.ID == customer.ID
                                            select c).FirstOrDefault();
                updatedCustomer.Debit = customer.Debit;
                updatedCustomer.TranxDate = customer.TranxDate;
                entities.SaveChanges();
            }

            return new EmptyResult();
        }

    }

   
}





//public class Pagesx
//{
//    public IEnumerable<Pages> comp { get; set; }
//    public int PageID { get; set; }
//    public string PageName { get; set; }
//    public Nullable<int> IsVisible { get; set; }
//    public string ValueDate { get; set; }
//    public string PageDescription { get; set; }
//    public string PageUrl { get; set; }

//}

public class TransactionalInformation
{
    public bool ReturnStatus { get; set; }
    public List<string> ReturnMessage { get; private set; }
    public int TotalRows { get; set; }
    public bool IsAuthenicated { get; set; }
    public string SortExpression { get; set; }
    public string SortDirection { get; set; }
    public int CurrentPageNumber { get; set; }

    public TransactionalInformation()
    {
        ReturnMessage = new List<string>();
        ReturnStatus = true;
        IsAuthenicated = false;
    }
}
    public class mandateSetup
{
    public string MandateID { set; get; }
    public string RequestID { set; get; }

    public string FirstName { set; get; }

    public string LastName { set; get; }

    public string Amount { set; get; }

    public string Phone { set; get; }

    public string Email { set; get; }

    public string RemitaTransRef {set; get;}

    public string startDate { set; get; }

    public string RRR { set; get; }

    public string ActivationDate { get; set; }
    public string value1 { get; set; }
   public string value2 { get; set; }
}
    public class data
    {
      public  int InstiD { get; set; }
      public double TotalAmounyt { get; set; }
      public bool respMSg { get; set; }
      public string DateTimes { get; set; }

      public string responseMsg { get; set; }

    public string account_name { get; set; }

    public string Tenure { get; set; }

    public int RepayMethod { get; set; }
    }

    public class record
    {
    public  double RecTotalAmount { get; set; }
    public string Recname { get; set; }
    public int RecCount { get; set; }

    public double ApproveTotalAmount { get; set; }

    public string Approvename { get; set; }
    public int ApproveCount { get; set; }

    public double DisbursedTotalAmount { get; set; }

    public string Disbursename { get; set; }
    public int DisursedCount { get; set; }

    public double PendingTotalAmount { get; set; }

    public string Pendingname { get; set; }
    public int PendingCount { get; set; }
}



