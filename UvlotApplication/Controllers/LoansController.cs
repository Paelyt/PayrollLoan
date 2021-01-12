using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccessLayerT;
using DataAccessLayerT.DataManager;
using UvlotApplication.Classes;
using System.Drawing.Printing;
using System.Configuration;
using System.Web.Hosting;
using UvlotApplication.HelperClasses;
using System.IO;
using Newtonsoft.Json.Linq;
//using EvoPdf;
using IronPdf;
using ExpertPdf.HtmlToPdf.PdfDocument;
using System.Text;

namespace UvlotApplication.Controllers
{
    public class LoansController : Controller
    {
        // GET: Loans
        //DataProvider dp = new DataProvider();
        DataReader dp = new DataReader();
        DataWriter dw = new DataWriter();
        UvlotEntities uvdb = new UvlotEntities();
        string user = "";
        string LoggedinInst = "";
        public static bool DemoMode = Convert.ToBoolean(ConfigurationManager.AppSettings["DemoMode"]);
        public static string MoneyWaveApiBase = DemoMode
          ? ConfigurationManager.AppSettings["MoneyWave_ApiBase_Test"]
          : ConfigurationManager.AppSettings["MoneyWave_ApiBase_Live"];
        public static string MoneyWaveResolveAccount = $"{MoneyWaveApiBase}{ConfigurationManager.AppSettings["MoneyWaveResolveAccount"]}";
        public static string MoneyWaveVerify = $"{MoneyWaveApiBase}{ConfigurationManager.AppSettings["MoneyWaveVerify"]}";

        public static string MoneyWaveApiKey = DemoMode
           ? ConfigurationManager.AppSettings["MoneyWave_Api_Key_Test"]
           : ConfigurationManager.AppSettings["MoneyWave_Api_Key_Live"];

        public static string MoneyWaveSecretKey = DemoMode
            ? ConfigurationManager.AppSettings["MoneyWave_Secret_Test"]
            : ConfigurationManager.AppSettings["MoneyWave_Secret_Live"];

        [HttpGet]
        public ActionResult ConverttoPdf()
        {
            try
            {

                pdf();
                return View();
                
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public void pdf()
        {
           
            var uri = new Uri("http://localhost:28957/Loans/OfferLetter?Refid=15797962948938");
            var urlToPdf = new HtmlToPdf
            {
                PrintOptions = new PdfPrintOptions()
                {
                   
                    PaperSize = PdfPrintOptions.PdfPaperSize.A4,
                   
                    CssMediaType = PdfPrintOptions.PdfCssMediaType.Print
                },
                // setting login credentials to bypass basic authentication
                LoginCredentials = new HttpLoginCredentials()
                {
                    NetworkUsername = "testUser",
                    NetworkPassword = "testPassword"
                }
            };
            var pdf = urlToPdf.RenderUrlAsPdf(uri);
            pdf.SaveAs("C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/UvlotApplication/UvlotApplication/Images/wikipedia.pdf");
            System.Diagnostics.Process.Start("C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/UvlotApplication/UvlotApplication/Images/wikipedia.pdf");
        }

        [HttpGet]
        public ActionResult StudentLoan(TableObjects.LoanApplication LoanApp)
        {
            try
            {
                if(LoanApp.Firstname == null || LoanApp.Firstname == "")
                {
                    return RedirectToAction("StudentPreLoan");
                }
                ViewBag.nBanks = dp.GetBanks();
                ViewBag.nRepmtMethods = dp.GetRepaymentMethods();
                ViewBag.nStates = dp.GetNigerianStates();
                ViewBag.nMeansOfIDs = dp.GetMeansOfIdentifications();
                ViewBag.nAccomodationTypes = dp.GetAccomodationTypes();
                ViewBag.nLGAs = dp.GetAllLGAs();
                ViewBag.nTitles = dp.GetTitles();
                ViewBag.nMarital = dp.GetMaritalStatus();
               
                return View();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public ActionResult Print()
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


        [HttpPost]
        public ActionResult StudentLoan(FormCollection form, TableObjects.LoanApplication lApObj)
        {
            try
            {
               
                string DOB = lApObj.DateOfBirth.ToString();
                DataAccessLayerT.DataManager.LoanApplication inst = new DataAccessLayerT.DataManager.LoanApplication
                {
                    Institution_FK = Convert.ToInt16(form["InstitutionFk"]),
                    AccomodationType_FK = Convert.ToInt32(form["AccomodationTypes"]), //Pending status
                    AccountName = lApObj.AccountName,
                    AccountNumber = lApObj.AccountNumber,
                    ApplicantID = lApObj.IdentficationNumber,
                    ApplicationStatus_FK = 3, //Pending status,
                    BankCode = Convert.ToString(form["Bank"]),
                    BVN = lApObj.BVN,
                    ClosestBusStop = lApObj.ClosestBusStop,
                    ContactAddress = lApObj.ContactAddress,
                    CreatedBy = "", //Change To User ID
                    DateCreated = MyUtility.getCurrentLocalDateTime(),
                    DateModified = MyUtility.getCurrentLocalDateTime(),
                    DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
                    EmailAddress = lApObj.EmailAddress,
                    ExistingLoan = false,//lApObj.ExistingLoan,
                    ExistingLoan_NoOfMonthsLeft = 0,//lApObj.ExistingLoan_NoOfMonthsLeft,
                   ExistingLoan_OutstandingAmount = 0,//lApObj.ExistingLoan_OutstandingAmount,
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
                    StudentLoanDetail empObj = new StudentLoanDetail();
                    empObj.MatricNumber = lApObj.IdentficationNumber;
                    empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                    empObj.DateModified = MyUtility.getCurrentLocalDateTime();
                    empObj.Department = Convert.ToString(form["department"]);
                    empObj.Faculty = Convert.ToString(form["FACULTY"]);
                    empObj.Institution = lApObj.Organization;
                    empObj.CourseOfStudy = Convert.ToString(form["Course"]);
                    empObj.InstitutionAddress = lApObj.InstitutionAddress;
                    empObj.YearOfAdmission = Convert.ToString(form["Year_admission"]);
                    empObj.YearOfExpectedCompletion = Convert.ToString(form["Year_Completion"]);
                    empObj.CurrentLevel_FK = Convert.ToInt16(form["CurrentLevel"]);
                    empObj.CurrentSemester_FK = Convert.ToInt16(form["CurrentSemester"]);
                    empObj.IsVisible = 1;
                    empObj.LoanApplication_FK = inst.ID;
                    //var loss = form["Los"].ToString();
                    //int los = MyUtility.isNumeric(loss) == true ? Convert.ToInt16(loss) : 0;
                    //string Amount = form["netMonthlyIncome"].ToString();
                   empObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                    empObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                  
                    DataWriter.CreateStudentLoanDetails(empObj);
                    return RedirectToAction("StuAcknowledgement", new { @Refid = inst.LoanRefNumber });
                    //return View("StuAcknowledgement");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpGet]
        public ActionResult StudentPreLoan()
        {
            try
            {
                ViewBag.StudentQualification = dp.getAllSchools();
                
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        


        [HttpGet]
        public ActionResult DisburseOfferLetter(string Refid)
        {
            try
            {
               
                MyUtility utilities = new MyUtility();
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("Acknowledgement", new { @Refid = Refid });
                }

                var Menus = dp.getOfferDetailsAfterDisbursement(Refid);
                if (Menus == null)
                {
                    return RedirectToAction("DisburseLoan", "Admin");
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

                AppLoan.LoanRefNumber = DistinctVal.LoanRefNumber;
                return View(AppLoan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

      //  [HttpGet]
      //public ActionResult  DisburseOfferLetter(string Refid)
      //  {
      //      try
      //      {

      //          DataAccessLayerT.DataManager.AppLoan AppLoan = new DataAccessLayerT.DataManager.AppLoan();
      //          MyUtility utilities = new MyUtility();
      //          if (Refid == null || Refid == "")
      //          {
      //              return RedirectToAction("Acknowledgement", new { @Refid = Refid });
      //          }
      //          var asig = dp.getSignature(Refid);
      //          // var sig = asig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
      //          WebLog.Log("1" + asig);
      //          if (asig != null)
      //          {
      //              //var sic = sig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
      //              var sig = asig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
      //              if (sig != null)
      //              {
      //                  AppLoan.signature = sig.signature;

      //                  WebLog.Log("2" + AppLoan.signature);
      //              }
      //          }
      //          // var Lastsig = signature.FirstOrDefault();


      //          var Menus = dp.getOfferDetails(Refid);
      //          WebLog.Log("3" + Menus);
      //          if (Menus == null)
      //          {
      //              return RedirectToAction("index", "Home");
      //          }

      //          var RepayValues = Menus.Distinct().GroupBy(x => x.LoanAmount);
      //          ViewBag.LoanRepayment = Menus.ToList();

      //          var DistinctVal = Menus.Distinct().Last();
      //          // AppLoan.signature = string.IsNullOrEmpty(DistinctVal.signature) ? "none" : AppLoan.signature;
      //          if (asig != null)
      //          {
      //              //var sic = sig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
      //              var sig = asig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
      //              if (sig != null)
      //              {
      //                  AppLoan.signature = sig.signature;

      //                  WebLog.Log("2" + AppLoan.signature);

      //                  //AppLoan.signature = sig.signature;
      //                  //WebLog.Log("4" + AppLoan.signature);
      //                  if (AppLoan.signature != "Guarantor Letter Signed")
      //                  {
      //                      AppLoan.signature = "none";
      //                  }
      //              }
      //              else
      //              {
      //                  AppLoan.signature = "none";
      //              }
      //          }
      //          else
      //          {
      //              AppLoan.signature = "none";
      //              WebLog.Log("5" + AppLoan.signature);
      //          }
      //          AppLoan.Firstname = DistinctVal.Firstname;
      //          AppLoan.Surname = DistinctVal.Surname;
      //          AppLoan.Title = DistinctVal.Title;
      //          AppLoan.ApplicationDate = DistinctVal.ApplicationDate.Date;
      //          AppLoan.GuarSurname = DistinctVal.GuarSurname;
      //          AppLoan.GuarOthernames = DistinctVal.GuarOthernames;
      //          AppLoan.GuarPhone = DistinctVal.GuarPhone;
      //          AppLoan.GuarRelationship = DistinctVal.GuarRelationship;
      //          AppLoan.GuarEmail = DistinctVal.GuarEmail;
      //          AppLoan.GuarContact = DistinctVal.GuarContact;
      //          AppLoan.LoanRefNumber = DistinctVal.LoanRefNumber;
      //          int year = AppLoan.ApplicationDate.Year;
      //          int month = AppLoan.ApplicationDate.Month;
      //          int day = AppLoan.ApplicationDate.Day;
      //          string days = AppLoan.ApplicationDate.ToLongDateString();
      //          var Datas = string.Format("{0}-{1}-{2}", days, month, day, year);
      //          AppLoan.ApplicationDatevalue = days;

      //          AppLoan.ContactAddress = DistinctVal.ContactAddress;
      //          AppLoan.PhoneNumber = DistinctVal.PhoneNumber;
      //          AppLoan.LoanAmount = DistinctVal.LoanAmount;
      //          AppLoan.ConvertedLoanAmt = MyUtility.ConvertToCurrency(AppLoan.LoanAmount.ToString());
      //          double inrate = dp.getInterestRate(DistinctVal.LoanTenure);
      //          AppLoan.interestRate = (float)inrate;
      //          int amtWordint = Convert.ToInt32(AppLoan.LoanAmount);

      //          var numberText = new NumberText.NumberText();
      //          AppLoan.AmountInWords = numberText.ToText(amtWordint) + " Niara";

      //          AppLoan.LoanTenure = DistinctVal.LoanTenure;
      //          AppLoan.OutstandingBalance = DistinctVal.OutstandingBalance;

      //          return View(AppLoan);
      //      }
      //      catch (Exception ex)
      //      {
      //          WebLog.Log(ex.Message.ToString());
      //          return null;
      //      }
      //  }
        [HttpGet]
        public ActionResult GuarantorLetter(string Refid)
        {
            try
            {

                DataAccessLayerT.DataManager.AppLoan AppLoan = new DataAccessLayerT.DataManager.AppLoan();
                MyUtility utilities = new MyUtility();
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("Acknowledgement", new { @Refid = Refid });
                }
                var asig = dp.getSignature(Refid);
               // var sig = asig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
                WebLog.Log("1"+ asig);
                if (asig != null )
                {
                    //var sic = sig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
                    var sig = asig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
                    if (sig != null)
                    {
                        AppLoan.signature = sig.signature;
                   
                    WebLog.Log("2" + AppLoan.signature);
                    }
                }
               // var Lastsig = signature.FirstOrDefault();
               
                
                var Menus = dp.getOfferDetails(Refid);
                WebLog.Log("3" + Menus);
                if (Menus == null)
                {
                    return RedirectToAction("index", "Home");
                }

                var RepayValues = Menus.Distinct().GroupBy(x => x.LoanAmount);
                ViewBag.LoanRepayment = Menus.ToList();
               
                var DistinctVal = Menus.Distinct().Last();
                // AppLoan.signature = string.IsNullOrEmpty(DistinctVal.signature) ? "none" : AppLoan.signature;
                if (asig != null)
                {
                    //var sic = sig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
                    var sig = asig.Where(x => x.signature == "Guarantor Letter Signed").FirstOrDefault();
                    if (sig != null)
                    {
                        AppLoan.signature = sig.signature;

                        WebLog.Log("2" + AppLoan.signature);

                        //AppLoan.signature = sig.signature;
                        //WebLog.Log("4" + AppLoan.signature);
                        if (AppLoan.signature != "Guarantor Letter Signed")
                        {
                            AppLoan.signature = "none";
                        }
                    }
                    else
                    {
                        AppLoan.signature = "none";
                    }
                }
                else
                {
                    AppLoan.signature = "none";
                    WebLog.Log("5" + AppLoan.signature);
                }
                AppLoan.Firstname = DistinctVal.Firstname;
                AppLoan.Surname = DistinctVal.Surname;
                AppLoan.Title = DistinctVal.Title;
                AppLoan.ApplicationDate = DistinctVal.ApplicationDate.Date;
                AppLoan.GuarSurname = DistinctVal.GuarSurname;
                AppLoan.GuarOthernames = DistinctVal.GuarOthernames;
                AppLoan.GuarPhone = DistinctVal.GuarPhone;
                AppLoan.GuarRelationship = DistinctVal.GuarRelationship;
                AppLoan.GuarEmail = DistinctVal.GuarEmail;
                AppLoan.GuarContact = DistinctVal.GuarContact;
                AppLoan.LoanRefNumber = DistinctVal.LoanRefNumber;
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
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


       

            public ActionResult PrintPdf(string Refid)
            {
            var report = new Rotativa.ActionAsPdf("GuarantorLetter", new { Refid = Refid });
            return report;
            }

           public ActionResult PrintOfferPDF(string Refid)
           {
            var report = new Rotativa.ActionAsPdf("OfferLetter", new { Refid = Refid });
            return report;
           }

        public ActionResult PrintDisbursedPdf(string Refid)
        {
            var report = new Rotativa.ActionAsPdf("DisburseOfferLetter", new { Refid = Refid });
            return report;
        }


        [HttpPost]
        public ActionResult SaveSignature(string signature)
        {
            try
            {
                DataReader dr = new DataReader();
                var signatures = signature.Before("&");
                var LoanRefNum = signature.After("&");
                signatures = signatures + " " + "Signed";
               /* var CheckSignture = dr.checkSignature(LoanRefNum);
                foreach(var i in CheckSignture)
                {
                    if(i.Signature1 == signatures)
                    {

                    }
                }*/
                var resp = insertSignature(signatures,LoanRefNum);

                //  return RedirectToAction("SignedOfferLetter", new { @Refid = LoanRefNum });
                var URL = "";
                if(signatures == "Guarantor Letter Signed")
                {
                    URL = "GuarantorLetter";
                    //return RedirectToAction("OfferLetter", new { @Refid = LoanRefNum });
                }
                if (signatures == "Offer Letter Signed")
                {
                    URL = "OfferLetter";
                    // return RedirectToAction("OfferLetter", new { @Refid = LoanRefNum });
                }
                // return RedirectToAction(URL, new { @Refid = LoanRefNum });
                return Json(new { data = true });

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult SignedOfferLetter(string Refid)
        {
            try
            {
                MyUtility utilities = new MyUtility();
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("Acknowledgement", new { @Refid = Refid });
                }

                var Menus = dp.getSignedOfferDetails(Refid);
                if (Menus == null)
                {
                    return RedirectToAction("index", "Home");
                }
                var RepayValues = Menus.Distinct().GroupBy(x => x.LoanAmount);
                ViewBag.LoanRepayment = Menus.ToList();
                DataAccessLayerT.DataManager.AppLoan AppLoan = new DataAccessLayerT.DataManager.AppLoan();
                var DistinctVal = Menus.Distinct().Last();
                AppLoan.signature = string.IsNullOrEmpty(DistinctVal.signature) ? "none" : DistinctVal.signature;
                AppLoan.Firstname = DistinctVal.Firstname;
                AppLoan.Surname = DistinctVal.Surname;
                AppLoan.Title = DistinctVal.Title;
                AppLoan.ApplicationDate = DistinctVal.ApplicationDate.Date;
                AppLoan.GuarSurname = DistinctVal.GuarSurname;
                AppLoan.GuarOthernames = DistinctVal.GuarOthernames;
                AppLoan.GuarPhone = DistinctVal.GuarPhone;
                AppLoan.GuarRelationship = DistinctVal.GuarRelationship;
                AppLoan.GuarEmail = DistinctVal.GuarEmail;
                AppLoan.GuarContact = DistinctVal.GuarContact;
                AppLoan.LoanRefNumber = DistinctVal.LoanRefNumber;
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

        public int insertSignature(string signatures, string LoanRefNum)
        {
            try
            {
                var check = (from a in uvdb.Signatures where a.LoanRefNum == LoanRefNum && a.Signature1 == signatures select a).FirstOrDefault();
                if (check != null)
                {
                    return 0;
                }
                else
                {
                    Signature signature = new Signature();
                    signature.LoanFk = "";
                    signature.LoanRefNum = LoanRefNum;
                    signature.Signature1 = signatures;

                    dw.InsertSinature(signature);
                    return signature.ID;
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        
        [HttpGet]
        public ActionResult TestSignature()
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
        public ActionResult OfferLetter(string Refid)

        {
            try
            {
                //var Refid = Convert.ToString(TempData["RefNum"]);
                //  Refid = "15697824915332";
                // Refid = Convert.ToString(TempData["RefNum"]);
                MyUtility utilities = new MyUtility();
                if(Refid== null || Refid == "")
                {
                    return RedirectToAction("Acknowledgement", new { @Refid = Refid });
                }
                
               var Menus = dp.getOfferDetails(Refid);
                if(Menus == null)
                {
                    return RedirectToAction("ApproveLoan","Admin");
                }

                var RepayValues = Menus.Distinct().GroupBy(x => x.LoanAmount);
                ViewBag.LoanRepayment = Menus.ToList();
                DataAccessLayerT.DataManager.AppLoan AppLoan = new DataAccessLayerT.DataManager.AppLoan();
                
                var asig = dp.getSignature(Refid);
                //var sig = asig.Where(x => x.signature == "Offer Letter Signed").FirstOrDefault();
                WebLog.Log("1" + asig);
                if (asig != null)
                {
                    //var sic = sig.Where(x => x.signature == "Offer Letter Signed").FirstOrDefault();
                    var sig = asig.Where(x => x.signature == "Offer Letter Signed").FirstOrDefault();
                   
                    if (sig != null)
                    {
                        AppLoan.signature = sig.signature;

                        WebLog.Log("2" + AppLoan.signature);
                    }
                }


                if (asig != null)
                {
                    //var sic = sig.Where(x => x.signature == "Offer Letter Signed").FirstOrDefault();
                    var sig = asig.Where(x => x.signature == "Offer Letter Signed").FirstOrDefault();
                   
                    if (sig != null)
                    {
                        AppLoan.signature = sig.signature;

                        WebLog.Log("2" + AppLoan.signature);

                        //AppLoan.signature = sig.signature;
                        //WebLog.Log("4" + AppLoan.signature);
                        if (AppLoan.signature != "Offer Letter Signed")
                        {
                            AppLoan.signature = "none";
                        }
                    }
                    else
                    {
                        AppLoan.signature = "none";
                    }
                }
                else
                {
                    AppLoan.signature = "none";
                    WebLog.Log("5" + AppLoan.signature);
                }

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
                double inrate =dp.getInterestRate(DistinctVal.LoanTenure);
                AppLoan.interestRate = (float)inrate;
                int amtWordint = Convert.ToInt32(AppLoan.LoanAmount);
                
                var numberText = new NumberText.NumberText();
                AppLoan.AmountInWords = numberText.ToText(amtWordint) + " Niara";

                AppLoan.LoanTenure = DistinctVal.LoanTenure;
                AppLoan.OutstandingBalance = DistinctVal.OutstandingBalance;
                AppLoan.LoanRefNumber = DistinctVal.LoanRefNumber;
                //Menus = Menus.ToList().Distinct().GroupBy(k => (k.LoanAmount)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());

                return View(AppLoan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult StudentPreLoan(FormCollection form, TableObjects.LoanApplication LoanApp)
        {
            try
            {
                var InstitutionId = Convert.ToInt16(form["id"]);
                var MatricNumber = Convert.ToString(form["MatricNum"]);
                var Phone = Convert.ToString(form["PhoneNum"]);

                var Record = dp.getStudentRecord(InstitutionId, MatricNumber, Phone);
                if(Record == null)
                {
                    TempData["ErrMsg"] = "Student Does Not Qualify For Loan";
                    ViewBag.StudentQualification = dp.getAllSchools();
                    return View();
                }
                else if(Record.OutstandingBalance > 0 )
                {
                    TempData["ErrMsg"] = "Student Already Have Outstanding Loan";
                    ViewBag.StudentQualification = dp.getAllSchools();
                    return View();
                }
                else 
                {
                    //return RedirectToAction("StudentLoan", new { @Refid = Record.LoanRefNumber });

                    LoanApp.Firstname = Record.Firstname;
                    LoanApp.Surname = Record.Surname;
                    LoanApp.Othernames = Record.Othernames;
                    LoanApp.PhoneNumber = Record.PhoneNumber;
                    LoanApp.EmailAddress = Record.EmailAddress;
                    LoanApp.faculty = Record.faculty;
                    LoanApp.Department =Record.Department;
                    LoanApp.IdentficationNumber = Record.IdentficationNumber;
                    LoanApp.Organization = Record.Organization;
                    LoanApp.DateOfBirth = Convert.ToDateTime(Record.DateOfBirth);
                    LoanApp.Gender = Record.Gender;
                    LoanApp.LoanAmount = Convert.ToString(Record.LoanAmount);
                    LoanApp.InstitutionAddress = Record.InstitutionAddress;
                    //Institution ID
                    LoanApp.institutionFk = Record.ID;
                    //return RedirectToAction("StudentLoan", LoanApp);
                    ViewBag.nBanks = dp.GetBanks();
                    ViewBag.nRepmtMethods = dp.GetRepaymentMethods();
                    ViewBag.nStates = dp.GetNigerianStates();
                    ViewBag.nMeansOfIDs = dp.GetMeansOfIdentifications();
                    ViewBag.nAccomodationTypes = dp.GetAccomodationTypes();
                    ViewBag.nLGAs = dp.GetAllLGAs();
                    ViewBag.nTitles = dp.GetTitles();
                    ViewBag.nMarital = dp.GetMaritalStatus();
                    return View("StudentLoan",LoanApp);

                }
                //ViewBag.StudentQualification = dp.getAllInstitution();
               // return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult GetLGAsByStateFK(int StateFK)
        {
            ViewBag.nLGAs = null;
            string val = Request.QueryString["val"];
            if (val != null)
            {
                int stateFK = Convert.ToInt16(val);
                ViewBag.nLGAs = dp.GetLGAsByStateFK(stateFK);
                return Json(new { Success = "true", Data = ViewBag.nLGAs });
            }

            return Json(new { Success = "false", Data = ViewBag.nLGAs });

        }

        [HttpPost]
        public JsonResult getLGAsByStateID(int id)
        {
            var ddlLGA = dp.GetLGAsByStateFK(id).ToList();
            List<SelectListItem> liLGAs = new List<SelectListItem>();

            liLGAs.Add(new SelectListItem { Text = "SELECT LGA", Value = "0" });
            if (ddlLGA != null)
            {
                foreach (var x in ddlLGA)
                {
                    liLGAs.Add(new SelectListItem { Text = x.Name, Value = x.ID.ToString() });
                }
            }
            return Json(new SelectList(liLGAs, "Value", "Text", JsonRequestBehavior.AllowGet));
        }


        public void getDropdownvalues(TableObjects.LoanApplication LAOBJ)
        {
            int val = LAOBJ.Title_FK;
            ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", val);
            ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", LAOBJ.MaritalStatus_FK);
            ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", LAOBJ.LGA_FK);

            ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", LAOBJ.AccomodationType_FK);
            ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", LAOBJ.MeansOfID_FK);
            ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", LAOBJ.StateofResidence_FK);
            ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", LAOBJ.Bank_FK);
            ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", LAOBJ.RepaymentMethod_FK);
            ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "NAME", LAOBJ.LoanTenure);
            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", LAOBJ.Gender_FK );
            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", 0);
            
            ViewBag.nBanks = dp.GetBanks();
            ViewBag.nRepmtMethods = dp.GetRepaymentMethods();
            ViewBag.nStates = dp.GetNigerianStates();
            ViewBag.nMeansOfIDs = dp.GetMeansOfIdentifications();
            ViewBag.nAccomodationTypes = dp.GetAccomodationTypes();
            ViewBag.nLGAs = dp.GetAllLGAs();
            ViewBag.nTitles = dp.GetTitles();
            ViewBag.nMarital = dp.GetMaritalStatus();
        }
        [HttpGet]
        public ActionResult LoanApplication()
        {
           
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

            ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME",val);
            ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", val);
            ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", val);
            ViewData["nBanks"] = new SelectList(dp.GetBanks(), "FlutterWaveBankCode", "NAME", val);
            ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", val);
            ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", val);
            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
            return View();
        }


        [HttpGet]
       public ActionResult TestLoanApplication()
        {
            try
            {
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
                ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", val);
                ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", val);
                ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", val);
              //  ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
              //  ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
                return View();

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult TestLoanApplication(FormCollection form, TableObjects.LoanApplication lApObj, HttpPostedFileBase PostedFile)
        {
             //form["institutionName"]
            try
            {
                Institution INST = new Institution();
                // var LnRefNum = lApObj.Organization + "/" + "PY" + "/" + DateTime.Now.Year.ToString().Substring(2)  + "/" + MyUtility.GenerateRefNo();
                INST.InstitutionCode = lApObj.Organization;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                string Amount = Convert.ToString(lApObj.LoanAmount).Replace(",", "").Split('.')[0].Trim();
                string SalaryAmount = Convert.ToString(lApObj.SalaryAmount).Replace(",", "").Split('.')[0].Trim();
                var Account_name = lApObj.AccountName;
                double repaymentmsg = 0;
                string respmsg = ""; string Phones = "";
                string InstCode = "";
                string Phone = lApObj.PhoneNumber;
                string NextOfKinPhone = lApObj.NOK_PhoneNumber;
                double SalaryAmt = 0; double LoanAmt = 0;
                SalaryAmt = Convert.ToDouble(SalaryAmount);
                LoanAmt = Convert.ToDouble(Amount);
                string filePath = saveImage(lApObj, PostedFile);
                Utility utilities = new Utility();
                bool ValidateLoan = false;
                bool Result = utilities.ValidateNum(Amount, SalaryAmount);
                var EmployerName = dp.ValidatePayrollInstitution(INST);
                lApObj.RepaymentMethod_FK = Convert.ToInt16(form["RepmtMethod"]);
                if (lApObj.ExistingLoan_OutstandingAmount == null)
                {
                    lApObj.ExistingLoan_OutstandingAmount = 0;
                }
                if (EmployerName != null)
                {
                    ValidateLoan = PayrollLoanCalculation(SalaryAmt, LoanAmt, lApObj.LoanTenure, out repaymentmsg, out respmsg, INST.InstitutionCode);
                }

                if (Account_name == null || Account_name == "" || Account_name == "Invalid Account Number")
                {
                    respmsg = "Please Make Sure You Provide A Valid Account Number";
                    TempData["Error"] = respmsg;
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    // getDropdownvalues(lApObj);
                    // int val = lApObj.Title_FK;

                    // ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", val);
                    return View("LoanApplication", lApObj);
                }
                if (EmployerName == null)
                {
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    TempData["Error"] = "Invalid Organization Code";
                    return View("LoanApplication", lApObj);
                }
                if (ValidateLoan == false)
                {
                    TempData["Error"] = respmsg;
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    // getDropdownvalues(lApObj);
                    // int val = lApObj.Title_FK;

                    // ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", val);
                    return View("LoanApplication", lApObj);
                }
                if (Result == false)
                {
                    TempData["Error"] = "Invalid Loan Amount OR Salary Amount";
                    // getDropdownvalues(lApObj);
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    return View("LoanApplication", lApObj);
                }
                var isValid = Utility.ValidatePhoneNumber(Phone, out Phones);
                if (isValid == false)
                {
                    TempData["Error"] = "Please check Phone Number";
                    // getDropdownvalues(lApObj);
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    return View("LoanApplication", lApObj);
                }
                var isNokPhone = Utility.ValidatePhoneNumber(NextOfKinPhone, out Phones);
                if (isNokPhone == false)
                {
                    TempData["Error"] = "Please Check Next Of kin Phone Number";
                    // getDropdownvalues(lApObj);
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    return View("LoanApplication", lApObj);
                }
                string DOB = lApObj.DateOfBirth.ToString();
                DataAccessLayerT.DataManager.LoanApplication inst = new DataAccessLayerT.DataManager.LoanApplication
                {
                    Institution_FK = EmployerName.ID,
                    MeansOfIDFilePath = filePath,
                    AccomodationType_FK = lApObj.AccomodationType_FK,//Convert.ToInt32(form["AccomodationTypes"]), //Pending status
                    AccountName = lApObj.AccountName,
                    AccountNumber = lApObj.AccountNumber,
                    ApplicantID = lApObj.ApplicantID,
                    ApplicationStatus_FK = 3, //Pending status,
                    BankCode = Convert.ToString(lApObj.BankCode),//Convert.ToString(form["Bank"]),
                    BVN = lApObj.BVN,
                    ClosestBusStop = lApObj.ClosestBusStop,
                    ContactAddress = lApObj.ContactAddress,
                    CreatedBy = Convert.ToString(userid), //Change To User ID
                    DateCreated = MyUtility.getCurrentLocalDateTime(),
                    DateModified = MyUtility.getCurrentLocalDateTime(),
                    DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
                    EmailAddress = lApObj.EmailAddress,
                    ExistingLoan = lApObj.ExistingLoan,
                    ExistingLoan_NoOfMonthsLeft = lApObj.ExistingLoan_NoOfMonthsLeft,
                    ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
                    Firstname = lApObj.Firstname,
                    Gender_FK = lApObj.Gender_FK,//Convert.ToInt32(form["selectGender"]),
                    IdentficationNumber = lApObj.IdentficationNumber,
                    Landmark = lApObj.Landmark,
                    LGA_FK = Convert.ToInt16(form["lgaList"]),
                    LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
                    LoanComment = "",
                    LoanRefNumber = lApObj.Organization + "/" + "PY" + "/" + DateTime.Now.Year.ToString().Substring(2) + "/" + MyUtility.GenerateRefNo(),
                    LoanTenure = lApObj.LoanTenure,//Convert.ToInt16(form["LoanTenure"]),// lApObj.LoanTenure,
                    MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
                    MeansOfID_FK = lApObj.MeansOfID_FK,//Convert.ToInt32(form["meansOfID"]),
                    NOK_EmailAddress = lApObj.NOK_EmailAddress,
                    NOK_FullName = lApObj.NOK_FullName,
                    NOK_HomeAddress = lApObj.NOK_HomeAddress,
                    NOK_PhoneNumber = lApObj.NOK_PhoneNumber,
                    NOK_Relationship = lApObj.NOK_Relationship,
                    Organization = EmployerName.Name,//lApObj.Organization,
                    Othernames = lApObj.Othernames,
                    PhoneNumber = lApObj.PhoneNumber,
                    RepaymentMethod_FK = lApObj.RepaymentMethod_FK,//Convert.ToInt32(form["RepmtMethod"]),
                    StateofResidence_FK = lApObj.StateofResidence_FK,//Convert.ToInt32(form["States"]),
                    Surname = lApObj.Surname,
                    Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
                    IsVisible = 1,
                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
                    LoanProduct_FK = 2
                };
                var resp = createUser(inst);
                /* if(resp == 0)
                 {
                     TempData["Error"] = "Email Already Exist";
                     ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                     ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                     ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                     ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                     ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                     ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                     ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                     ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                     ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                     ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                     ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                     return View("LoanApplication", lApObj);
                 }*/
                //string mario = form["institutionName"];
                if (resp > 0)
                {
                    if (userid == 0)
                    {
                        inst.CreatedBy = Convert.ToString(resp);
                    }

                    DataWriter.CreateLoanApplication(inst);
                }
                if (resp == 0)
                {
                    if (userid == 0)
                    {
                        string email = inst.EmailAddress;
                        int uid = dp.getUserID(email);
                        inst.CreatedBy = Convert.ToString(uid);
                    }
                    DataWriter.CreateLoanApplication(inst);
                }

                if (inst.ID > 0)
                {
                    //Create Users
                    //var resp = createUser(inst);

                    CreateGuarantor(lApObj, inst);
                    EmployerLoanDetail empObj = new EmployerLoanDetail();
                    empObj.ClosestBusStop = lApObj.ClosestBusStop;
                    empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                    empObj.DateModified = MyUtility.getCurrentLocalDateTime();
                    empObj.Department = lApObj.Department;//Convert.ToString(form["department"]);
                    empObj.Designation = lApObj.Designation;//Convert.ToString(form["designation"]);
                    empObj.EmployerID = inst.ApplicantID;
                    empObj.EmploymentStatus_FK = lApObj.Contract;//Convert.ToInt32(form["employment_status"]);
                    empObj.IsVisible = 1;
                    empObj.LandMark = lApObj.Landmark;
                    var loss = lApObj.LOS;//form["Los"].ToString();
                    int los = MyUtility.isNumeric(loss) == true ? Convert.ToInt16(loss) : 0;
                    empObj.LengthOfServiceInMth = los;
                    empObj.LengthOfServiceInMth = Convert.ToInt16(form["LOSMONTHS"]);
                    empObj.LengthOfServiceInYrs = Convert.ToInt16(form["LOSYEARS"]);
                    empObj.LGA_FK = Convert.ToString(form["lgaList"]);
                    //Amount = form["netMonthlyIncome"].ToString();
                    empObj.NetMonthlyIncome = MyUtility.isFloat(SalaryAmount) == true ? Convert.ToDouble(SalaryAmount) : 0;
                    // empObj.NetMonthlyIncome = Convert.ToDouble(SalaryAmt);
                    empObj.Occupation = lApObj.Occupation;//Convert.ToString(form["occupation"]);
                    empObj.OfficialEmailAddress = lApObj.OfficialEmail;//Convert.ToString(form["officeEmail"]);
                    empObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                    empObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                    empObj.LoanApplication_FK = inst.ID;

                    DataWriter.CreateEmployerLoanDetails(empObj);
                   // SendLoanApp(lApObj.EmailAddress);
                    // Send Email to Recommeders
                    AdminController ac = new AdminController();
                    // var Roleid = 2;
                    int Roleid = Convert.ToInt16(ConfigurationManager.AppSettings["RecommendRole"]);
                    LoggedinInst = LoggedInuser.LoggedInInstitution();
                    //LoanApp.InstitutionEmailAddress = LoggedinInst;
                    var EmailList = dp.GetNextLevelUser(Roleid);
                    if (EmailList.Count > 1)
                    {
                        var i = 0;
                        for (i = 0; i < EmailList.Count; i++)
                        {
                            user = EmailList[i];
                            ac.SendNextLevelEmail(user, LoggedinInst);
                        }
                    }
                    if (EmailList.Count == 1)
                    {
                        ac.SendNextLevelEmail(EmailList[0], LoggedinInst);
                    }
                    //  return View("Acknowledgement");
                    return RedirectToAction("Acknowledgement", new { @Refid = inst.LoanRefNumber });
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
    
        
        public List<SelectListItem> GetAllGender()
        {
            try
            {
                List<SelectListItem> Gender = new List<SelectListItem>();
                Gender.Add(new SelectListItem { Value = "1",  Text = "Male" });
                Gender.Add(new SelectListItem { Value = "2", Text = "Female" });
                return Gender;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<SelectListItem> GetAppStatus()
        {
            try
            {
                List<SelectListItem> Contract = new List<SelectListItem>();
                Contract.Add(new SelectListItem { Value = "1", Text = "Contract" });
                Contract.Add(new SelectListItem { Value = "2", Text = "Permanent" });
                return Contract;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult Acknowledgement(string Refid)
        {
            Utility utilities = new Utility();
            if (Refid == null || Refid == "")
            {
                return RedirectToAction("/");
            }
            var LoanApps = dp.LoanDetails(Refid);
            if (LoanApps == null)
            {
                return RedirectToAction("/");
            }
            LoanApps.ConvertedAmount = utilities.ConvertToCurrency(LoanApps.LoanAmount.ToString());
            

            return View(LoanApps);
        }
        
        public ActionResult StuAcknowledgement(string Refid)
        {
            Utility utilities = new Utility();
            if (Refid == null || Refid == "")
            {
                return RedirectToAction("/");
            }
            var LoanApps = dp.StudentLoanDetails(Refid);
            if (LoanApps == null)
            {
                return RedirectToAction("/");
            }
            LoanApps.ConvertedAmount = utilities.ConvertToCurrency(LoanApps.LoanAmount.ToString());
            
           
            return View(LoanApps);
        }

        public List<LRepay> AllTenuerPayrollLoanCalculation(double salaryAmt, double LoanAmt, int Tenure, out double repaymentAmount, out string respMessage, string InstCode )
        {
            repaymentAmount = 0;
            respMessage = "";
            double repays = 0;
            var repay = new List<LRepay>();
           
            try
            {
               // var LoanRate = dp.GetRate(Tenure, InstCode);
                var LoanRate = dp.GetRateQuartely(Tenure, InstCode);
                foreach(var i in LoanRate)
                {
                    if (i.LoanTenure > 12)
                   {
                     respMessage = "Invalid Tenure: Tenure cannot be greater than 12 months";
                     // return false;
                    }
                    if (i.LoanTenure == 3)
                    {
                        var LI = i.InterestRate / 100;
                        Tenure = 3;
                        repaymentAmount = (LoanAmt + (LoanAmt * Tenure) * LI) /    Tenure;
                        
                repay.Add(new LRepay { LoanAmount = MyUtility.ConvertToCurrency( LoanAmt.ToString()), LoanTenure = Tenure, LoanRepayment = MyUtility.ConvertToCurrency(repaymentAmount.ToString()) });
                    }
                    if (i.LoanTenure == 6)
                    {
                        Tenure = 6;
                        var LI = i.InterestRate / 100;
                        repaymentAmount = (LoanAmt + (LoanAmt * Tenure) * LI) / Tenure;
                        repay.Add(new LRepay { LoanAmount = MyUtility.ConvertToCurrency(LoanAmt.ToString()), LoanTenure = Tenure, LoanRepayment = MyUtility.ConvertToCurrency(repaymentAmount.ToString()) });
                    }
                    if (i.LoanTenure == 9)
                    {
                        Tenure = 9;
                        var LI = i.InterestRate / 100;
                        repaymentAmount = (LoanAmt + (LoanAmt * Tenure) * LI) / Tenure;
                      repay.Add(new LRepay { LoanAmount = MyUtility.ConvertToCurrency(LoanAmt.ToString()), LoanTenure = Tenure, LoanRepayment = MyUtility.ConvertToCurrency(repaymentAmount.ToString()) });
                    }
                    if (i.LoanTenure == 12)
                    {
                        Tenure = 12;
                        var LI = i.InterestRate / 100;
                      repaymentAmount = (LoanAmt + (LoanAmt * Tenure) * LI) / Tenure;
                      repay.Add(new LRepay { LoanAmount = MyUtility.ConvertToCurrency(LoanAmt.ToString()), LoanTenure = Tenure, LoanRepayment = MyUtility.ConvertToCurrency(repaymentAmount.ToString()) });
                    }
                }
                if (LoanRate == null)
                {
                    respMessage = "Invalid Tenure";
                   //return false;
                }

                return repay;
               


            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        

        public bool PayrollLoanCalculation(double salaryAmt,double LoanAmt , int Tenure, out double repaymentAmount, out string respMessage,string InstCode)
        {
            repaymentAmount = 0;
            respMessage = "";
            try
            {
                //since 250/100 is 0.4 divide by salary to get RealLoanAmt
                var maxLoanAllowed = salaryAmt * 2.5;
                // 60% salary
                //  var sixtyPercentSal =  (0.33 * salaryAmt);//One third of the salary fpr repaYMENT
                var sixtyPercentSal = (0.4 * salaryAmt); // fourty Percent of Salary For Repayment
                if (LoanAmt > maxLoanAllowed)
                {
                    respMessage = "Invalid Loan Amount! You cannot apply for more than 2.5x your salary";
                    return false;
                }
                var LoanRate = dp.GetRate(Tenure,InstCode);
                if (LoanRate == null)
                { 
                    respMessage = "Invalid Tenure";
                    return false;
                }
              if (LoanRate.LoanTenure > 12)
                {
                    respMessage = "Invalid Tenure: Tenure cannot be greater than 12 months";
                    return false;
                }
                var LI = LoanRate.InterestRate / 100;
                repaymentAmount = (LoanAmt+(LoanAmt*Tenure)*LI)/Tenure;
                if(repaymentAmount >= sixtyPercentSal)
                {
                    respMessage = "Invalid Loan Amount: Monthly repayment amount cannot be greater than 2/5 your salary";
                    return false;
                }
                else
                {
                    respMessage = "0";
                    return true;
                }
               
               
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        /* [HttpPost]
          public ActionResult CreatePayrollLoan(FormCollection form, TableObjects.LoanApplication lApObj,HttpPostedFileBase PostedFile)
             {
               //form["institutionName"]
               try
               {
                   Institution INST = new Institution();
                   INST.InstitutionCode = lApObj.Organization;
                   var LoggedInuser = new LogginHelper();
                   user = LoggedInuser.LoggedInUser();
                   var userid = LoggedInuser.LoggedInUserID(user);
                   var appUser = user;
                   string Amount = Convert.ToString(lApObj.LoanAmount).Replace(",", "").Split('.')[0].Trim();
                   string SalaryAmount = Convert.ToString(lApObj.SalaryAmount).Replace(",", "").Split('.')[0].Trim();
                   double repaymentmsg = 0;
                   string respmsg = ""; string Phones = "";
                   string InstCode = "";
                   string Phone = lApObj.PhoneNumber;
                   string NextOfKinPhone = lApObj.NOK_PhoneNumber;
                   double SalaryAmt = 0; double LoanAmt = 0;
                   SalaryAmt = Convert.ToDouble(SalaryAmount);
                   LoanAmt = Convert.ToDouble(Amount);
                   string filePath = saveImage(lApObj, PostedFile);
                   Utility utilities = new Utility();
                   bool ValidateLoan = false;
                   bool Result = utilities.ValidateNum(Amount, SalaryAmount);
                   var EmployerName = dp.ValidatePayrollInstitution(INST);
                   if (EmployerName != null)
                   {
                       ValidateLoan = PayrollLoanCalculation(SalaryAmt, LoanAmt, lApObj.LoanTenure, out repaymentmsg, out respmsg, INST.InstitutionCode);
                   }
                   if (EmployerName == null)
                   {

                       TempData["Error"] = "Invalid Organization Code";
                       return View("LoanApplication", lApObj);
                   }
                   if (ValidateLoan == false)
                   {
                       TempData["Error"] = respmsg;

                       // getDropdownvalues(lApObj);
                       // int val = lApObj.Title_FK;

                       // ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", val);
                       return View("LoanApplication", lApObj);
                   }
                   if (Result == false)
                   {
                       TempData["Error"] = "Invalid Loan Amount OR Salary Amount";
                       // getDropdownvalues(lApObj);

                       return View("LoanApplication", lApObj);
                   }
                   var isValid = Utility.ValidatePhoneNumber(Phone, out Phones);
                   if (isValid == false)
                   {
                       TempData["Error"] = "Please check Phone Number";
                       // getDropdownvalues(lApObj);

                       return View("LoanApplication", lApObj);
                   }
                   var isNokPhone = Utility.ValidatePhoneNumber(NextOfKinPhone, out Phones);
                   if (isNokPhone == false)
                   {

                       return View("LoanApplication", lApObj);
                   }
                   string DOB = lApObj.DateOfBirth.ToString();
                   DataAccessLayerT.DataManager.LoanApplication inst = new DataAccessLayerT.DataManager.LoanApplication
                   {
                       Institution_FK = EmployerName.ID,
                       MeansOfIDFilePath = filePath,
                       AccomodationType_FK = lApObj.AccomodationType_FK,//Convert.ToInt32(form["AccomodationTypes"]), //Pending status
                       AccountName = lApObj.AccountName,
                       AccountNumber = lApObj.AccountNumber,
                       ApplicantID = lApObj.ApplicantID,
                       ApplicationStatus_FK = 3, //Pending status,
                       BankCode = Convert.ToString(lApObj.BankCode),//Convert.ToString(form["Bank"]),
                       BVN = lApObj.BVN,
                       ClosestBusStop = lApObj.ClosestBusStop,
                       ContactAddress = lApObj.ContactAddress,
                       CreatedBy = Convert.ToString(userid), //Change To User ID
                       DateCreated = MyUtility.getCurrentLocalDateTime(),
                       DateModified = MyUtility.getCurrentLocalDateTime(),
                       DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
                       EmailAddress = lApObj.EmailAddress,
                       ExistingLoan = lApObj.ExistingLoan,
                       ExistingLoan_NoOfMonthsLeft = lApObj.ExistingLoan_NoOfMonthsLeft,
                       ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
                       Firstname = lApObj.Firstname,
                       Gender_FK = lApObj.Gender_FK,//Convert.ToInt32(form["selectGender"]),
                       IdentficationNumber = lApObj.IdentficationNumber,
                       Landmark = lApObj.Landmark,
                       LGA_FK = Convert.ToInt16(form["lgaList"]),
                       LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
                       LoanComment = "",
                       LoanRefNumber = MyUtility.GenerateRefNo(),
                       LoanTenure = lApObj.LoanTenure,//Convert.ToInt16(form["LoanTenure"]),// lApObj.LoanTenure,
                       MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
                       MeansOfID_FK = lApObj.MeansOfID_FK,//Convert.ToInt32(form["meansOfID"]),
                       NOK_EmailAddress = lApObj.NOK_EmailAddress,
                       NOK_FullName = lApObj.NOK_FullName,
                       NOK_HomeAddress = lApObj.NOK_HomeAddress,
                       NOK_PhoneNumber = lApObj.NOK_PhoneNumber,
                       NOK_Relationship = lApObj.NOK_Relationship,
                       Organization = EmployerName.Name,//lApObj.Organization,
                       Othernames = lApObj.Othernames,
                       PhoneNumber = lApObj.PhoneNumber,
                       RepaymentMethod_FK = lApObj.RepaymentMethod_FK,//Convert.ToInt32(form["RepmtMethod"]),
                       StateofResidence_FK = lApObj.StateofResidence_FK,//Convert.ToInt32(form["States"]),
                       Surname = lApObj.Surname,
                       Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
                       IsVisible = 1,
                       ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                       ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
                       LoanProduct_FK = 2


                   };
                   //string mario = form["institutionName"];
                   DataWriter.CreateLoanApplication(inst);

                   if (inst.ID > 0)
                   {
                        //Create Users
                        createUser(inst);

                       CreateGuarantor(lApObj,inst);
                       EmployerLoanDetail empObj = new EmployerLoanDetail();
                       empObj.ClosestBusStop = lApObj.ClosestBusStop;
                       empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                       empObj.DateModified = MyUtility.getCurrentLocalDateTime();
                       empObj.Department = lApObj.Department;//Convert.ToString(form["department"]);
                       empObj.Designation = lApObj.Designation;//Convert.ToString(form["designation"]);
                       empObj.EmployerID = inst.ApplicantID;
                       empObj.EmploymentStatus_FK = lApObj.Contract;//Convert.ToInt32(form["employment_status"]);
                       empObj.IsVisible = 1;
                       empObj.LandMark = lApObj.Landmark;
                       var loss = lApObj.LOS;//form["Los"].ToString();
                       int los = MyUtility.isNumeric(loss) == true ? Convert.ToInt16(loss) : 0;
                       empObj.LengthOfServiceInMth = los;
                       empObj.LGA_FK = Convert.ToString(form["lgaList"]);
                       //Amount = form["netMonthlyIncome"].ToString();
                       empObj.NetMonthlyIncome = MyUtility.isFloat(SalaryAmount) == true ? Convert.ToDouble(SalaryAmount) : 0;
                       // empObj.NetMonthlyIncome = Convert.ToDouble(SalaryAmt);
                       empObj.Occupation = lApObj.Occupation;//Convert.ToString(form["occupation"]);
                       empObj.OfficialEmailAddress = lApObj.OfficialEmail;//Convert.ToString(form["officeEmail"]);
                       empObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                       empObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                       empObj.LoanApplication_FK = inst.ID;

                       DataWriter.CreateEmployerLoanDetails(empObj);
                       //  return View("Acknowledgement");
                       return RedirectToAction("Acknowledgement", new { @Refid = inst.LoanRefNumber });
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
           */

        public class lApObjs
        {
            public string Surname { get; set; }
        }

        [HttpPost]
        public ActionResult SaveApplication(TableObjects.LoanApplication lApObj)
        {
            try
            {
                var uc = lApObj.Surname;
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
       // [HttpPost]
       //  public ActionResult SaveApplication(string lApObjc, TableObjects.LoanApplication lApObj, FormCollection form, HttpPostedFileBase PostedFile)
       /*    public ActionResult SaveApplication(TableObjects.LoanApplication lApObj)
       {
           try
           {
               FormCollection form  = new FormCollection();
               HttpPostedFileBase PostedFile;
               Institution INST = new Institution();
               INST.InstitutionCode = lApObj.Organization;
               var LoggedInuser = new LogginHelper();
               user = LoggedInuser.LoggedInUser();
               var userid = LoggedInuser.LoggedInUserID(user);
               var appUser = user;
               string Amount = "0";
               string SalaryAmount = "0";
               if (Amount != "0") {
                Amount = Convert.ToString(lApObj.LoanAmount).Replace(",", "").Split('.')[0].Trim();
                SalaryAmount = Convert.ToString(lApObj.SalaryAmount).Replace(",", "").Split('.')[0].Trim();
               }

               string lgaList = Convert.ToString(form["lgaList"]);
               var Account_name = lApObj.AccountName;
               double repaymentmsg = 0;
               string respmsg = ""; string Phones = "";
               string InstCode = "";
               string Phone = lApObj.PhoneNumber;
               string NextOfKinPhone = lApObj.NOK_PhoneNumber;
               double SalaryAmt = 0; double LoanAmt = 0;
               SalaryAmt = Convert.ToDouble(SalaryAmount);
               LoanAmt = Convert.ToDouble(Amount);
               //string filePath = saveImage(lApObj, PostedFile);
               //if(filePath == null || filePath == "")
               //{
               //    filePath = "none";
               //}
               Utility utilities = new Utility();
               bool ValidateLoan = false;
               bool Result = utilities.ValidateNum(Amount, SalaryAmount);
               var EmployerName = dp.ValidatePayrollInstitution(INST);
               lApObj.RepaymentMethod_FK = Convert.ToInt16(form["RepmtMethod"]);
               if (lApObj.ExistingLoan_OutstandingAmount == null)
               {
                   lApObj.ExistingLoan_OutstandingAmount = 0;
               }
               if (EmployerName != null)
               {
                   ValidateLoan = PayrollLoanCalculation(SalaryAmt, LoanAmt, lApObj.LoanTenure, out repaymentmsg, out respmsg, INST.InstitutionCode);
               }
               string DOB = lApObj.DateOfBirth.ToString();
               DataAccessLayerT.DataManager.LoanApplication inst = new DataAccessLayerT.DataManager.LoanApplication();
            //   {
              // inst.Institution_FK = EmployerName.ID;
             //  inst.MeansOfIDFilePath = filePath;
               inst.AccomodationType_FK = lApObj.AccomodationType_FK;
               inst.AccountName = lApObj.AccountName;
                   inst.AccountNumber = lApObj.AccountNumber;
                   inst.ApplicantID = lApObj.ApplicantID;
                   inst.ApplicationStatus_FK = 3; 
                   inst.BankCode = Convert.ToString(lApObj.BankCode);
                   inst.BVN = lApObj.BVN;
                   inst.ClosestBusStop = lApObj.ClosestBusStop;
                   inst.ContactAddress = lApObj.ContactAddress;
                   inst.CreatedBy = Convert.ToString(userid); //Change To User ID
               inst.DateCreated = MyUtility.getCurrentLocalDateTime();
               inst.DateModified = MyUtility.getCurrentLocalDateTime();
               inst.DateOfBirth = Convert.ToString(lApObj.DateOfBirth);
               inst.EmailAddress = lApObj.EmailAddress;
               inst.ExistingLoan = lApObj.ExistingLoan;
               inst.ExistingLoan_NoOfMonthsLeft = lApObj.ExistingLoan_NoOfMonthsLeft;
               inst.ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount;
               inst.Firstname = lApObj.Firstname;
               inst.Gender_FK = lApObj.Gender_FK;
               inst.IdentficationNumber = lApObj.IdentficationNumber;
               inst.Landmark = lApObj.Landmark;
               inst.LGA_FK = Convert.ToInt16(form["lgaList"]);
               inst.LoanAmount = Convert.ToDouble(lApObj.LoanAmount);
               inst.LoanComment = "";
               if (lApObj.Organization != null)
               {
                   inst.LoanRefNumber = lApObj.Organization + "/" + "PY" + "/" + DateTime.Now.Year.ToString().Substring(2) + "/" + MyUtility.GenerateRefNo();
               }
               else
               {
                   lApObj.Organization = null;
               }
               inst.LoanTenure = lApObj.LoanTenure;
               inst.MaritalStatus_FK = lApObj.MaritalStatus_FK;
               inst.MeansOfID_FK = lApObj.MeansOfID_FK;
               inst.NOK_EmailAddress = lApObj.NOK_EmailAddress;
               inst.NOK_FullName = lApObj.NOK_FullName;
               inst.NOK_HomeAddress = lApObj.NOK_HomeAddress;
               inst.NOK_PhoneNumber = lApObj.NOK_PhoneNumber;
               inst.NOK_Relationship = lApObj.NOK_Relationship;
               //if (EmployerName.Name == null )
               //{
               //    inst.Organization = EmployerName.Name;//lApObj.Organization,
               //}
               inst.Othernames = lApObj.Othernames;
               inst.PhoneNumber = lApObj.PhoneNumber;
               inst.RepaymentMethod_FK = lApObj.RepaymentMethod_FK;
               inst.StateofResidence_FK = lApObj.StateofResidence_FK;
               inst.Surname = lApObj.Surname;
               inst.Title_FK = lApObj.Title_FK;
               inst.IsVisible = 9;
               inst.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
               inst.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
               inst.LoanProduct_FK = 2;


               DataWriter.CreateLoanApplication(inst);


               if (inst.ID > 0)
               {

                   CreateGuarantor(lApObj, inst);
                   EmployerLoanDetail empObj = new EmployerLoanDetail();
                   empObj.ClosestBusStop = lApObj.ClosestBusStop;
                   empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                   empObj.DateModified = MyUtility.getCurrentLocalDateTime();
                   empObj.Department = lApObj.Department;
                   empObj.Designation = lApObj.Designation;
                   empObj.EmployerID = inst.ApplicantID;
                   empObj.EmploymentStatus_FK = lApObj.Contract;
                   empObj.IsVisible = 1;
                   empObj.LandMark = lApObj.Landmark;
                   var loss = lApObj.LOS;
                   int los = MyUtility.isNumeric(loss) == true ? Convert.ToInt16(loss) : 0;
                   empObj.LengthOfServiceInMth = los;
                   empObj.LengthOfServiceInMth = Convert.ToInt16(form["LOSMONTHS"]);
                   empObj.LengthOfServiceInYrs = Convert.ToInt16(form["LOSYEARS"]);
                   empObj.LGA_FK = Convert.ToString(form["lgaList"]);

                   empObj.NetMonthlyIncome = MyUtility.isFloat(SalaryAmount) == true ? Convert.ToDouble(SalaryAmount) : 0;
                   empObj.Occupation = lApObj.Occupation;
                   empObj.OfficialEmailAddress = lApObj.OfficialEmail;
                   empObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                   empObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                   empObj.LoanApplication_FK = inst.ID;

                   DataWriter.CreateEmployerLoanDetails(empObj);

                   TempData["SucMsg"] = "Application Succesfuly Saved";
                   return View();
               }
           }
           catch (Exception ex)
           {
               WebLog.Log(ex.Message.ToString());
               return null;
           }
           return View();
       }
       */
        public ActionResult CreatePayrollLoan(string submit,FormCollection form, TableObjects.LoanApplication lApObj, HttpPostedFileBase PostedFile)
        {
            //form["institutionName"]
            try
            {
                int AppStat = 3;
                if (submit == "Adsubmit")
                {
                    var DtCreated = Convert.ToString(form["DateCreated"]);
                    var DtDisbursed = Convert.ToString(form["DateDisbursed"]);
                    AppStat = 6;
                   // var DtDOB = Convert.ToDateTime(form["DOB"]);
                    if (DtCreated != null && DtCreated != "" && DtDisbursed != null)
                    {
                        lApObj.DateCreated = Convert.ToDateTime(form["DateCreated"]);
                        lApObj.DateModified = Convert.ToDateTime(form["DateDisbursed"]);
                        lApObj.DateOfBirth = Convert.ToDateTime(form["DOB"]);
                    }
                    else
                    {

                        lApObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                        lApObj.DateModified = MyUtility.getCurrentLocalDateTime();
                    }
                }
                else
                {
                    lApObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                    lApObj.DateModified = MyUtility.getCurrentLocalDateTime();
                }
                Institution INST = new Institution();
               // var LnRefNum = lApObj.Organization + "/" + "PY" + "/" + DateTime.Now.Year.ToString().Substring(2)  + "/" + MyUtility.GenerateRefNo();
                INST.InstitutionCode = lApObj.Organization;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                var userid = LoggedInuser.LoggedInUserID(user);
                var appUser = user;
                string Amount = Convert.ToString(lApObj.LoanAmount).Replace(",", "").Split('.')[0].Trim();
                string SalaryAmount = Convert.ToString(lApObj.SalaryAmount).Replace(",", "").Split('.')[0].Trim();
                
                
                string lgaList = Convert.ToString(form["lgaList"]);
                var Account_name = lApObj.AccountName;
                double repaymentmsg = 0;
                string respmsg = ""; string Phones = "";
                string InstCode = "";
                string Phone = lApObj.PhoneNumber;
                string NextOfKinPhone = lApObj.NOK_PhoneNumber;
                double SalaryAmt = 0; double LoanAmt = 0;
                SalaryAmt = Convert.ToDouble(SalaryAmount);
                LoanAmt = Convert.ToDouble(Amount);
                string filePath = saveImage(lApObj, PostedFile);
                Utility utilities = new Utility();
                bool ValidateLoan = false;
                bool Result = utilities.ValidateNum(Amount, SalaryAmount);
                var EmployerName = dp.ValidatePayrollInstitution(INST);
                lApObj.RepaymentMethod_FK = 3; //Convert.ToInt16(form["RepmtMethod"]);
                if (lApObj.ExistingLoan_OutstandingAmount == null)
                {
                    lApObj.ExistingLoan_OutstandingAmount = 0;
                    lApObj.ExistingLoan = false;
                }
                if(lApObj.ExistingLoan_OutstandingAmount != 0)
                {
                    lApObj.ExistingLoan = true;
                }
                if (EmployerName != null)
                {
                    ValidateLoan = PayrollLoanCalculation(SalaryAmt, LoanAmt, lApObj.LoanTenure, out repaymentmsg, out respmsg, INST.InstitutionCode);
                }

                if (lgaList == null || lgaList == "")
                {
                    respmsg = "Please Make Sure You Select Your Local Govt";
                    TempData["Error"] = respmsg;
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);

                    return View("LoanApplication", lApObj);
                }
                if (PostedFile.ContentLength == 0)
                {
                    respmsg = "Please Make Sure You Upload Means Of Identification";
                    TempData["Error"] = respmsg;
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                   
                    return View("LoanApplication", lApObj);
                }
                 if(Account_name == null || Account_name == "" || Account_name == "Invalid Account Number")
                {
                    respmsg = "Please Make Sure You Provide A Valid Account Number";
                    TempData["Error"] = respmsg;
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    // getDropdownvalues(lApObj);
                    // int val = lApObj.Title_FK;

                    // ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", val);
                    return View("LoanApplication", lApObj);
                }
                if (EmployerName == null)
                {
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    TempData["Error"] = "Invalid Organization Code";
                    return View("LoanApplication", lApObj);
                }
                if (ValidateLoan == false)
                {
                    TempData["Error"] = respmsg;
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    // getDropdownvalues(lApObj);
                    // int val = lApObj.Title_FK;

                    // ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", val);
                    return View("LoanApplication", lApObj);
                }
                if (Result == false)
                {
                    TempData["Error"] = "Invalid Loan Amount OR Salary Amount";
                    // getDropdownvalues(lApObj);
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    return View("LoanApplication", lApObj);
                }
                var isValid = Utility.ValidatePhoneNumber(Phone, out Phones);
                if (isValid == false)
                {
                    TempData["Error"] = "Please check Phone Number";
                    // getDropdownvalues(lApObj);
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    return View("LoanApplication", lApObj);
                }
                var isNokPhone = Utility.ValidatePhoneNumber(NextOfKinPhone, out Phones);
                if (isNokPhone == false)
                {
                    TempData["Error"] = "Please Check Next Of kin Phone Number";
                    // getDropdownvalues(lApObj);
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    return View("LoanApplication", lApObj);
                }
                var FlutterwaveBankCode = dp.GetFlutterwaveCode(lApObj.BankCode);
                string DOB = lApObj.DateOfBirth.ToString();
                DataAccessLayerT.DataManager.LoanApplication inst = new DataAccessLayerT.DataManager.LoanApplication
                {
                    Institution_FK = EmployerName.ID,
                    MeansOfIDFilePath = filePath,
                    AccomodationType_FK = lApObj.AccomodationType_FK,//Convert.ToInt32(form["AccomodationTypes"]), //Pending status
                    AccountName = lApObj.AccountName,
                    AccountNumber = lApObj.AccountNumber,
                    ApplicantID = lApObj.ApplicantID,
                    ApplicationStatus_FK = AppStat, //Pending status,
                    BankCode = Convert.ToString(lApObj.BankCode),
                    RemitaBankCode = FlutterwaveBankCode.Code,//Convert.ToString(form["Bank"]),
                    BVN = lApObj.BVN,
                    ClosestBusStop = lApObj.ClosestBusStop,
                    ContactAddress = lApObj.ContactAddress,
                    CreatedBy = Convert.ToString(userid), //Change To User ID
                                                          // DateCreated = MyUtility.getCurrentLocalDateTime(),
                                                          // DateModified = MyUtility.getCurrentLocalDateTime(),
                    DateCreated = lApObj.DateCreated,
                    DateModified = lApObj.DateModified,
                    DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
                    EmailAddress = lApObj.EmailAddress,
                    ExistingLoan = lApObj.ExistingLoan,
                    ExistingLoan_NoOfMonthsLeft = lApObj.ExistingLoan_NoOfMonthsLeft,
                    ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
                    Firstname = lApObj.Firstname,
                    Gender_FK = lApObj.Gender_FK,//Convert.ToInt32(form["selectGender"]),
                    IdentficationNumber = lApObj.IdentficationNumber,
                    Landmark = lApObj.Landmark,
                    LGA_FK = Convert.ToInt16(form["lgaList"]),
                    LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
                    LoanComment = "",
                    LoanRefNumber = lApObj.Organization + "/" + "PY" + "/" + DateTime.Now.Year.ToString().Substring(2) + "/" + MyUtility.GenerateRefNo(),
                    LoanTenure = lApObj.LoanTenure,//Convert.ToInt16(form["LoanTenure"]),// lApObj.LoanTenure,
                    MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
                    MeansOfID_FK = lApObj.MeansOfID_FK,//Convert.ToInt32(form["meansOfID"]),
                    NOK_EmailAddress = lApObj.NOK_EmailAddress,
                    NOK_FullName = lApObj.NOK_FullName,
                    NOK_HomeAddress = lApObj.NOK_HomeAddress,
                    NOK_PhoneNumber = lApObj.NOK_PhoneNumber,
                    NOK_Relationship = lApObj.NOK_Relationship,
                    Organization = EmployerName.Name,//lApObj.Organization,
                    Othernames = lApObj.Othernames,
                    PhoneNumber = lApObj.PhoneNumber,
                    RepaymentMethod_FK = lApObj.RepaymentMethod_FK,//Convert.ToInt32(form["RepmtMethod"]),
                    StateofResidence_FK = lApObj.StateofResidence_FK,//Convert.ToInt32(form["States"]),
                    Surname = lApObj.Surname,
                    Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
                    IsVisible = 1,
                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
                    LoanProduct_FK = 2
                };
                var resp = createUser(inst);
               /* if(resp == 0)
                {
                    TempData["Error"] = "Email Already Exist";
                    ViewData["nTitles"] = new SelectList(dp.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(dp.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(dp.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(dp.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(dp.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(dp.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(dp.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(dp.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(dp.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    return View("LoanApplication", lApObj);
                }*/
                //string mario = form["institutionName"];
                if(resp > 0)
                {
                    if (userid == 0)
                    {
                        inst.CreatedBy = Convert.ToString(resp);
                    }
                    
                DataWriter.CreateLoanApplication(inst);
                }
                if(resp == 0)
                {
                    if (userid == 0)
                    {
                        string email = inst.EmailAddress;
                        int uid = dp.getUserID(email);
                        inst.CreatedBy = Convert.ToString(uid);
                    }
                    DataWriter.CreateLoanApplication(inst);
                }

                if (inst.ID > 0)
                {
                    //Create Users
                   //var resp = createUser(inst);

                    CreateGuarantor(lApObj, inst);
                    EmployerLoanDetail empObj = new EmployerLoanDetail();
                    empObj.ClosestBusStop = lApObj.ClosestBusStop;
                    empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                    empObj.DateModified = MyUtility.getCurrentLocalDateTime();
                    empObj.Department = lApObj.Department;//Convert.ToString(form["department"]);
                    empObj.Designation = lApObj.Designation;//Convert.ToString(form["designation"]);
                    empObj.EmployerID = inst.ApplicantID;
                    empObj.EmploymentStatus_FK = lApObj.Contract;//Convert.ToInt32(form["employment_status"]);
                    empObj.IsVisible = 1;
                    empObj.LandMark = lApObj.Landmark;
                    var loss = lApObj.LOS;//form["Los"].ToString();
                    int los = MyUtility.isNumeric(loss) == true ? Convert.ToInt16(loss) : 0;
                    empObj.LengthOfServiceInMth = los;
                    empObj.LengthOfServiceInMth = Convert.ToInt16(form["LOSMONTHS"]);
                    empObj.LengthOfServiceInYrs = Convert.ToInt16(form["LOSYEARS"]);
                    empObj.LGA_FK = Convert.ToString(form["lgaList"]);
                    //Amount = form["netMonthlyIncome"].ToString();
                    empObj.NetMonthlyIncome = MyUtility.isFloat(SalaryAmount) == true ? Convert.ToDouble(SalaryAmount) : 0;
                    // empObj.NetMonthlyIncome = Convert.ToDouble(SalaryAmt);
                    empObj.Occupation = lApObj.Occupation;//Convert.ToString(form["occupation"]);
                    empObj.OfficialEmailAddress = lApObj.OfficialEmail;//Convert.ToString(form["officeEmail"]);
                    empObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                    empObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                    empObj.LoanApplication_FK = inst.ID;
                    
                    DataWriter.CreateEmployerLoanDetails(empObj);
                    SendLoanApp(lApObj.EmailAddress);
                    // Send Email to Recommeders
                    AdminController ac = new AdminController(); 
                   // var Roleid = 2;
                   // i will have to uncomment this later
                 /*   int Roleid = Convert.ToInt16(ConfigurationManager.AppSettings["RecommendRole"]);
                    LoggedinInst = LoggedInuser.LoggedInInstitution();
                    //LoanApp.InstitutionEmailAddress = LoggedinInst;
                    var EmailList = dp.GetNextLevelUser(Roleid);
                    if (EmailList.Count > 1)
                    {
                        var i = 0;
                        for (i = 0; i < EmailList.Count; i++)
                        {
                            user = EmailList[i];
                            ac.SendNextLevelEmail(user, LoggedinInst);
                        }
                    }
                    if (EmailList.Count == 1)
                    {
                        ac.SendNextLevelEmail(EmailList[0], LoggedinInst);
                    }*/
                    //  return View("Acknowledgement");
                    return RedirectToAction("Acknowledgement", new { @Refid = inst.LoanRefNumber });
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

        public void SendLoanApp(string user)
        {
            try
            {

                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/EmailNotification.html"));
                bodyTxt = bodyTxt.Replace("$UserName", user);

                var msgHeader = $"Welcome to CashNowNow";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, user, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }

        public string saveImage(TableObjects.LoanApplication lApObj,HttpPostedFileBase PostedFile)
        {
            try
            {
                string filePath = "";
                if (PostedFile != null && PostedFile.ContentLength > 0)
                {
                    string filename = Path.GetFileName(PostedFile.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);
                   
                    if (fileExt == ".jpg"|| fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        PostedFile.SaveAs((filePath));
                    }
                    
                }
                return filePath;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public string CreateGuarantor(TableObjects.LoanApplication lApObj,DataAccessLayerT.DataManager.LoanApplication inst)
        {
            try
            {
                Guarantor guarantor = new Guarantor();
                guarantor.ContactAddress = lApObj.GuarContact;
                guarantor.EmailAddress = lApObj.GuarEmail;
                guarantor.Othernames = lApObj.GuarOthernames;
                guarantor.PhoneNumber = lApObj.GuarPhone;
                guarantor.RelationShipWithApplicant = lApObj.GuarRelationship;
                guarantor.Surname = lApObj.GuarSurname;
                guarantor.LoanApplication_FK =inst.ID;
                guarantor.IsVisible = 1;
                guarantor.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                DataWriter.CreateGuarantor(guarantor);


                return guarantor.ID.ToString();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetTenureByInstCode(string code)




        {
            try
            {
                ViewBag.ServicesList = dp.GetTenureByInstCode(code);
                data dt = new data();
                
                return Json(new { Success = "true", Data = ViewBag.ServicesList });
                //for (var i = 0 ; i<= LoanTenure.Count(); i++)
                //{
                //    dt.Tenure =Convert.ToString(LoanTenure[i].LoanTenure);
                //}
                //return Json(dt.Tenure);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult CheckIfExistingLoan(string LoanAmount)
            {
            try
            {
                Institution Inst = new Institution();
                string respmsg = "";
                double repaymentmsg = 0;
                string LoanAmountv = Convert.ToString(LoanAmount).Replace(@"\", "");
                double SalaryAmount = Convert.ToDouble(LoanAmount.After("^").Before("!"));

                double LoanAmounts = Convert.ToDouble(LoanAmount.Before("&"));
                int LoanTenure = Convert.ToInt16(LoanAmount.After("?").Before("^"));
                var InstCode = LoanAmount.After("&").Before("?");
                double ExistingLA = Convert.ToDouble(LoanAmount.After("!"));
                // Inst = dp.getInstParam(InstCode);
                LoanAmounts = ExistingLA + LoanAmounts;
                bool valid = PayrollLoanCalculation(SalaryAmount, LoanAmounts, LoanTenure, out repaymentmsg, out respmsg, InstCode);

                if(respmsg != "0")
                {
                    // respmsg = "You No Longer Qualify For The Loan Amount Requested Reduce The Loan Amount And Try Again";
                   // respmsg = respmsg;
                }
                List<LRepay> LoanRecords = new List<LRepay>();
                if (respmsg == "0")
                {
                    LoanRecords = CalculateThreeTenureRecords(SalaryAmount, LoanAmounts, LoanTenure, InstCode);
                }

                /*  if(respmsg == "0")
                  {
                      List<LRepay> LoanRecords = CalculateThreeTenureRecords(SalaryAmount, LoanAmounts, LoanTenure, InstCode);
                      // I Added This Today

                      if (respmsg == "0")
                        {
                            LoanRecords = (from a in LoanRecords where a.LoanTenure >= LoanTenure select a).ToList();

                        }
                      return Json(new { response = valid, Data = respmsg, Loanrec = LoanRecords });
                  }
                  */
                // List<LRepay> LoanRecords = CalculateThreeTenureRecords(SalaryAmount, LoanAmounts, LoanTenure, InstCode);


               // return Json(new { Data = respmsg, Loanrec = LoanRecords });
                return Json(respmsg);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
         }
        public ActionResult CheckIfQualifyForLoan(string LoanAmount)
      {
            try
            {
                Institution Inst = new Institution();
                string respmsg = "";
                double repaymentmsg = 0;
                string LoanAmountv = Convert.ToString(LoanAmount).Replace(@"\", "");
                double SalaryAmount = Convert.ToDouble(LoanAmount.After("^").Before("vb"));
              
                double LoanAmounts = Convert.ToDouble(LoanAmount.Before("&"));
                double LoanAmountss = Convert.ToDouble(LoanAmount.Before("&"));
                int LoanTenure = Convert.ToInt16(LoanAmount.After("?").Before("^"));
                var InstCode = LoanAmount.After("&").Before("?");
                var Existing = Convert.ToDouble(LoanAmount.After("vb"));
                if(Existing > 0)
                {
                    LoanAmounts = LoanAmounts + Existing;
                }
                // Inst = dp.getInstParam(InstCode);

                bool valid = PayrollLoanCalculation(SalaryAmount, LoanAmounts, LoanTenure, out repaymentmsg, out respmsg,InstCode);
               
               
                 
                List<LRepay> LoanRecords = CalculateThreeTenureRecords(SalaryAmount, LoanAmountss,LoanTenure,InstCode);
                // I Added This Today
               
              if (respmsg == "0" && Existing > 0)
                {
                    LoanRecords = (from a in LoanRecords where a.LoanTenure >= LoanTenure select a).ToList();
                }
              
                return Json(new { response = valid, Data = respmsg,Loanrec = LoanRecords });
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<LRepay> CalculateThreeTenureRecords(double SA,double LA,int LT,string ISC)
        {
            try
            {
               double rM = 0;
               string rMsg = "";
                //var Lists = PayrollLoanCalculation(SA,LA,LT,out rM,out rMsg,ISC);
               List<LRepay> _items = new List<LRepay>();
                _items = AllTenuerPayrollLoanCalculation(SA, LA, LT, out rM, out rMsg, ISC);
                ViewBag.Data = _items;
                
                return _items.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;   
            }
        }
        [HttpPost]
        public ActionResult CheckEmpCode(string code)


       {
            try
            {
                UvlotEntities uvdb = new UvlotEntities();
                int DebitType = 0;
                bool isValid = dp.validateInstCode(code,out DebitType);
                //return Json(new { Success = "true", Data = isValid });
                data dt = new data();
                dt.respMSg = isValid;
                dt.RepayMethod = DebitType;
                // return Json(isValid);
                return Json(dt,JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult CheckEmail(string code)

        {
            try
            {
                UvlotEntities uvdb = new UvlotEntities();
                bool isValid = dp.validateEmail(code);
                //return Json(new { Success = "true", Data = isValid });
                return Json(isValid);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public int createUser(LoanApplication LoanApp)
        {
            try
            {
                bool valid = dp.Validate(LoanApp.EmailAddress, LoanApp.PhoneNumber);
               // bool valid = dp.Validate(LoanApp.EmailAddress);
                if (valid == false)
                {
                      if (LoanApp.EmailAddress == null || LoanApp.EmailAddress == "")
                   // if (LoanApp.EmailAddress == null )
                    {
                        LoanApp.EmailAddress = LoanApp.PhoneNumber;
                    }
                }
                if(valid == true)
                {
                    return 0;
                }
                    User users = new User();
                /*Create user After Loan Approved */
                users.Firstname = LoanApp.Firstname;
                users.Lastname = LoanApp.Surname;
                users.PhoneNumber = LoanApp.PhoneNumber;
                string password = MyUtility.GeneratePassword();
                //users.PaswordVal = ConfigurationManager.AppSettings["RecommendLevelPassword];
                users.PaswordVal = password;
                var passwords = users.PaswordVal;
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(password, HashName.SHA256);
                users.EmailAddress = LoanApp.EmailAddress;
                users.UserAddress = LoanApp.ContactAddress;
                users.ValueDate = MyUtility.getCurrentLocalDateTime();
                users.IsVisible = 1;
                users.LastUpdated = null;
                users.PinNumber = "";
                users.PaswordVal = EncrypPassword;
                users.Title_FK = Convert.ToInt16(LoanApp.Title_FK);
                users.GenderID = Convert.ToInt16(LoanApp.Gender_FK);
                users.DateOfBirth = Convert.ToString(LoanApp.DateOfBirth);
                users.ReferralCode = "";
                users.MyReferralCode = "";
                users.Audit = "";
                users.ReferralLevel = 0;
                users.EmploymentStatus_FK = 1;
                var id = dw.InsertUser(users);
                if (id > 0)
                {
                    /* Today */
                    UserRole UserRoles = new UserRole();
                    UserRoles.User_FK = id;
                    UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["Applicant"]);
                    UserRoles.IsVisible = 1;
                    dw.InsertUserRoles(UserRoles);

                    /* Today */
                    SendPassword(users,password);

                }

                return id;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        [HttpGet]
        public ActionResult BVNValidation()
        {
            return View();
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
               // Utility utilities = new Utility();
                string msg = "";
                var ApplicationFk = Convert.ToString(form["RefNumber"]);

                DataAccessLayerT.Classes.AppLoans Apploan = dp.CheckAppStatus(ApplicationFk);
               
                //string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                //Apploan.ConvertedLoanAmt = utilities.ConvertToCurrency(LoanAmt);
                if (Apploan == null)
                {
                    TempData["ErrMsg"] = "Record Not Found! ";
                    return View();
                }

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Record Not Found! ";
                return null;
            }
        }



        [HttpGet]
        public ActionResult CheckStudentLoanStatus()
        {
            try
            {
               return View();
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return
                    null;
            }

        }

        public static bool Verify(out string token)
        {

            token = null;
            dynamic obj = new JObject();
            obj.apiKey = MoneyWaveApiKey;//System.Web.Configuration.WebConfigurationManager.AppSettings["MoneyWave_Api_Key_Test"];
            obj.secret = MoneyWaveSecretKey; //System.Web.Configuration.WebConfigurationManager.AppSettings["MoneyWave_Secret_Test"];

            var json = obj.ToString();
            var resp = MyUtilities.DoPostToken(json, MoneyWaveVerify);

            if (resp == null) return false;
            var jvalue = JObject.Parse(resp);
            var status = (string)jvalue["status"];
            token = status.ToLower() == "success" ? (string)jvalue["token"] : string.Empty;
            return !string.IsNullOrWhiteSpace(token);
        }


        [HttpPost]
        public ActionResult VerifyAccountNumber(string Account)
        {
            try
            {
                dynamic obj =new JObject();
                var acctNumber = Account.After("&");
                var BankCode = Account.Before("&");
                obj.account_number = acctNumber;
                obj.bank_code = BankCode;
                var json = obj.ToString();
                string token = "";
                Verify(out token);
                var resp = MyUtilities.DoPost(json, MoneyWaveResolveAccount, token);
                dynamic respa = JObject.Parse(resp);
                data dt = new data();
                dt.responseMsg = respa?.status;
                if(dt.responseMsg == "success")
                {
                    //dt.responseMsg = respa?.status;
                    dt.respMSg = true;
                    dt.account_name = respa?.data?.account_name;
                   
                }
                if (dt.responseMsg == "error")
                {
                    dt.respMSg = false;
                    dt.responseMsg = respa?.status;
                    dt.account_name = "Invalid Account Number".Trim();
                    dt.account_name = "Invalid Account Number".ToString();
                }
                var cct = dt.account_name.TrimStart();
                //  { "status":"error","code":"UNAUTHORIZED_ACCESS","message":"Unauthorized access."}
                return Json(new {  dt });
            }
            catch(Exception ex)
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
                var ApplicationFk = Convert.ToString(form["RefNumber"]);
                  
                DataAccessLayerT.Classes.AppLoans Apploan = dp.CheckLoanStatus(ApplicationFk);

                if (Apploan == null)
                {
                    TempData["ErrMsg"] = "Record Not Found! ";
                    return View();
                }

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult CheckEligibility()
        {
            
            ViewBag.Institution = uvdb.Institutions.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CheckEligibility(LoanViewModel lvm, FormCollection form, DataAccessLayerT.Classes.AppLoans Apploan)
        {
            try
            {
                
                    StudentRecord StdRec = new StudentRecord();
                    ViewBag.Institution = uvdb.Institutions.ToList();
                    StdRec.Institution_FK = Convert.ToInt32(form["CheckEligibility"]);
                    StdRec.MatriculationNumber = lvm.StudentRecord.MatriculationNumber;
                    StdRec.PhoneNumber = lvm.StudentRecord.PhoneNumber;
                    Apploan = dp.CheckInstitution(StdRec);

                
                if (Apploan == null)
                {
                    TempData["ErrMsg"] = "Student Record Not Found !";
                    return RedirectToAction("CheckEligibility");
                }
                   return View("CheckEligibility2", Apploan);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult CheckEligibility2(LoanViewModel lvm)
        {
           
            return View();
        }

        public ActionResult BVNValidation(FormCollection form, Helper.BVNC bvnc)
        {

            string bvn = Convert.ToString(form["BVN"]) == null ? "" : Convert.ToString(form["BVN"]);


            bvnc = Helper.BVNValidationResps(bvn);
            if (bvnc.respCode == "00")
            {
                return View("BVNValidationResp", bvnc);
            }
            else
            {
                ViewBag.Data = bvnc.errormessage;
                return View("BVNValidation");
            }


        }



        public void SendEmail(dynamic lvm)
        {
            try
            {

                // var smsResponse = NotificationService.SendSms(tAlert);
                
                if (lvm.Contract == null)
                {
                    var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/RecommendEmailNotification.html"));
                    bodyTxt = bodyTxt.Replace("$MerchantName", $"{lvm.Firstname} {lvm.Surname}").Replace("$LoanComment", $"{lvm.LoanComment}");//.Replace("$Contract", $"{lvm.Contract}");
                    var msgHeader = $"Welcome to CashNowNow";
                    var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, lvm.InstitutionEmailAddress, null);
                }
                if (lvm.Contract != null)
                {
                    DataAccessLayerT.DataManager.AppLoan LoanApp = new AppLoan();
                  //  LoanApp.LoanTenure
                 
                    var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/ApproveEmailNotification.html"));
                    bodyTxt = bodyTxt.Replace("$MerchantName", $"{lvm.Firstname} {lvm.Surname}").Replace("$LoanComment", $"{lvm.LoanComment}").Replace("$Contract", $"{lvm.Contract}").Replace("$RepaymentAmt", $"{lvm.RepaymentAmount}").Replace("$refNumber", $"{lvm.LoanRefNumber}").Replace("$LoanAmount", $"{lvm.ConvertedAmount}").Replace("$LoanTenure", $"{lvm.LoanTenure}");
                    var msgHeader = $"URGENT : CashNowNow Payroll Loan Offer Letter";
                    var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, lvm.InstitutionEmailAddress, null);
                   // var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, lvm.InstitutionEmailAddress, lvm.files);
                }

                if (lvm.GuaContract != null)
                {
                    DataAccessLayerT.DataManager.AppLoan LoanApp = new AppLoan();
                    var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/ApproveGuaEmailNotification.html"));
                    bodyTxt = bodyTxt.Replace("$MerchantName", $"{lvm.GuarSurname} {lvm.GuarOthernames}").Replace("$LoanComment", $"{lvm.LoanComment}").Replace("$Contract", $"{lvm.GuaContract}").Replace("$RepaymentAmt", $"{lvm.RepaymentAmount}").Replace("$refNumber", $"{lvm.LoanRefNumber}").Replace("$LoanAmount", $"{lvm.ConvertedAmount}").Replace("$LoanTenure", $"{lvm.LoanTenure}");
                    var msgHeader = $"URGENT : CashNowNow Payroll Loan Guarantor Letter";
                    var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.GuarEmail, lvm.InstitutionEmailAddress, null);
                    // var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, lvm.InstitutionEmailAddress, lvm.files);
                }


                if (lvm.Contract != null && lvm.AppStat == 6)
                {
                    var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/DisburseEmailNotificaton.html"));
                    bodyTxt = bodyTxt.Replace("$MerchantName", $"{lvm.Firstname} {lvm.Surname}").Replace("$LoanComment", $"{lvm.LoanComment}").Replace("$Contract", $"{lvm.Contract}");
                    var msgHeader = $"URGENT : CashNowNow Payroll Loan Disbursement";
                    var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, lvm.InstitutionEmailAddress, null);
                }


                //var bodyTxt = lvm.LoanComment;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        public void SendPassword(dynamic lvm,string password)
        {
            try
            {

                // var smsResponse = NotificationService.SendSms(tAlert);
                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/WelcomeEmailNotification.html"));
                bodyTxt = bodyTxt.Replace("$MerchantName", $"{lvm.Firstname} {lvm.Lastname}").Replace("$Password", $"{password}").Replace("$EmailAddress", $"{lvm.EmailAddress}");
                //var bodyTxt = lvm.LoanComment;
                var msgHeader = $"Welcome to CashNowNow Payroll Loan";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        public void SendRemita(dynamic lvm,string link)
        {
            try
            {
                DataAccessLayerT.DataManager.AppLoan LoanApp = new DataAccessLayerT.DataManager.AppLoan();
                //LoanApp.Repayment
                // var smsResponse = NotificationService.SendSms(tAlert);
                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/remitaformEmail.html"));
                //bodyTxt = bodyTxt.Replace("$username", $"{lvm.Firstname} {lvm.Lastname}").Replace("$LoanAmount", $"{lvm.LoanAmount}").Replace("$LoanTenure", $"{lvm.LoanTenure}").Replace("$LoanTenure", $"{lvm.LoanTenure}").Replace("$RepaymentAmt", $"{lvm.Repayment}");



               bodyTxt = bodyTxt.Replace("$username", $"{lvm.Firstname} {lvm.Surname}").Replace("$LoanComment", $"{lvm.LoanComment}").Replace("$Contract", $"{lvm.Contract}").Replace("$RepaymentAmt", $"{lvm.RepaymentAmount}").Replace("$refNumber", $"{lvm.LoanRefNumber}").Replace("$LoanAmount", $"{lvm.ConvertedAmount}").Replace("$LoanTenure", $"{lvm.LoanTenure}").Replace("$remitalink", $"{link}"); 

                 var msgHeader = $"CashNowNow Remita Standing Order Form";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }






    }
    public class Foo
    {
        public double Column1 { get; set; }
        public int Column2 { get; set; }
        public double Column3 { get; set; }
    }

    public class LRepay
    {
        public string LoanAmount { get; set; }

        public double LoanTenure { get; set; }

        public string LoanRepayment { get; set; }
    }
}


