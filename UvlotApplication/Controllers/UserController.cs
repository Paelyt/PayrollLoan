using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccessLayerT.DataManager;
using Utilities;
using UvlotApplication.HelperClasses;
using System.Web.Security;
using System.Configuration;
using UvlotApplication.Classes;
using DataAccessLayerT;
using System.Web.Hosting;

namespace UvlotApplication.Controllers
{
    public class UserController : Controller
    {
       DataWriter _DM = new DataWriter();
       DataReader _DR = new DataReader();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Sigin()
        {
            if(Session["User"] != null || Session["User"].ToString() != "")
            {
                return  Redirect("/");
            }
          
            return View();
        }
        [HttpGet]
        public ActionResult Signin()
        {
            TempData["ErrMsg"] = "";
            TempData["SucMsg"] = "";
             var CashoutLimit = Convert.ToDouble(ConfigurationManager.AppSettings["CashoutLimit"]);
            if (Session["User"] != null )
            {
                return Redirect("/");
            }

            return View();
        }
        [HttpGet]
        public ActionResult Signup()
        {
            TempData["ErrMsg"] = "";
            TempData["SucMsg"] = "";
            if (Session["User"] != null )
            {
                return Redirect("/");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Signin(FormCollection form)
        {
            try
            {
                // Signinx(form);
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                User user = new User();
                int? value = 0;
                user.EmailAddress = Convert.ToString(form["username"]);
                user.PaswordVal = Convert.ToString(form["password"]);
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(user.PaswordVal, HashName.SHA256);
                user.PaswordVal = EncrypPassword;
                var valid = _DR.loggedIn(user.EmailAddress, EncrypPassword,out value);
                
                WebLog.Log("Valid1" + valid);
                if (valid == true && value == 0)
                {
                    WebLog.Log("Valid2" + valid);
                    if (user.EmailAddress != null)
                {
             WebLog.Log("users.Email 2" + user.EmailAddress);
             Session["id"] = LoggedInEmail(user.EmailAddress);
             string email = Convert.ToString(Session["id"]);
                        //today
                        var users = _DR.getUser(email);
                        string InstFkEmail = _DR.getInstFKByEmail(email);
                        Session["InstFkEmail"] = InstFkEmail;
                        Session["User"] = Session["id"];
                        if(users.EmploymentStatus_FK == 1)
                        {
                            return RedirectToAction("MyApplicationsStatus", "Admin");
                        }
                        else
                        { 
                            return RedirectToAction("index", "Admin");
                        }
                    }
                 else
                 TempData["ErrMsg"] = "Invalid User Try Again";
                 return View("Signin");
                }
               else if(valid == false)
               {
                     WebLog.Log("Valid3" + valid);
                     TempData["ErrMsg"] = "User Does Not Exist";
                     return View("Signin");
               }
                else if(valid == true && value == 1)
               {
                    WebLog.Log("Valid3" + valid);
                    TempData["ErrMsg"] = "Please Change Your Password";

                    //  return RedirectToAction("changePassword", "User",user);
                    return View("changePassword", user);
                }
                else if (valid == true && value == null)
                {
                    WebLog.Log("Valid3" + valid);
                    TempData["ErrMsg"] = "Please Change Your Password";

                    //  return RedirectToAction("changePassword", "User",user);
                    return View("changePassword", user);
                }
                return View();
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                // return null;
                TempData["ErrMsg"] = "Error ! Please try again";
                return View();
            }
        }

        [HttpPost]
        public ActionResult changePassword(DataAccessLayerT.DataManager.User user, FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                user.PaswordVal = Convert.ToString(form["passwordVal"]);
                var rpassword = Convert.ToString(form["rpasswordVal"]);
                bool validatepas = ValidatePassword(user.PaswordVal, rpassword);
                if (validatepas == false)
                {
                    TempData["ErrMsg"] = "Password And Confirm Password Must Match";
                    return View();
                }
                if (validatepas == true)
                {
                    var val = _DR.checkEmail(user.EmailAddress);
                    if (val == null)
                    {
                        TempData["ErrMsg"] = "User Does Not Exist";

                        return View();
                    }
                    else
                    {
                        var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(user.PaswordVal, HashName.SHA256);
                        user.PaswordVal = EncrypPassword;
                        _DM.changePassword(user);
                        WebLog.Log("users.Email 2" + user.EmailAddress);
                        Session["id"] = LoggedInEmail(user.EmailAddress);
                        string email = Convert.ToString(Session["id"]);
                        string InstFkEmail = _DR.getInstFKByEmail(email);
                        Session["InstFkEmail"] = InstFkEmail;
                        Session["User"] = Session["id"];

                        return RedirectToAction("index", "Admin");
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

        [HttpGet]
        public ActionResult termcondition()
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
        [HttpGet]
        public ActionResult changePassword(DataAccessLayerT.DataManager.User user)
        {
            try
            {
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                if(user.EmailAddress == null)
                {
                    return RedirectToAction("Signin");
                }
                var email = user.EmailAddress;
                return View(user);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult LogOut()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("HomePage", "Home");

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


       /* public void Signinx(FormCollection form)
        {
            try
            {
                User user = new User();
                user.EmailAddress = Convert.ToString(form["username"]);
                user.PaswordVal = Convert.ToString(form["password"]);
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(user.PaswordVal, HashName.SHA256);
                user.PaswordVal = EncrypPassword;

                var valid = _DR.loggedIn(user.EmailAddress, EncrypPassword);
                WebLog.Log("Valid1" + valid);
                if (valid == true)
                {
                    WebLog.Log("Valid2" + valid);
                    if (user.EmailAddress != null)
                    {
                        WebLog.Log("users.Email 2" + user.EmailAddress);
                        Session["id"] = user.EmailAddress;
                        Session["User"] = Session["id"];
                    }
                    else
                   TempData["message"] = "Invalid User Try Again";

                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEX)
            {
                Exception raise = dbEX;
                foreach (var validationErrors in dbEX.EntityValidationErrors)
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
        */
        [HttpPost]
        public ActionResult Signup(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = "";
                TempData["ErrMsg"] = "";
                User Users = new User();
              string password = Convert.ToString(form["password"]);
              string rpassword = Convert.ToString(form["cpassword"]);
              Users.EmailAddress = Convert.ToString(form["email"]); 
              Users.Firstname = Convert.ToString(form["fname"]);
             // Users.Lastname = Convert.ToString(form["lname"]);
              Users.PhoneNumber = Convert.ToString(form["phone"]);
                var pasw = password;
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(password, HashName.SHA256);
         bool validatepas = ValidatePassword(password, rpassword);
                if (validatepas == false)
                {
                TempData["ErrMsg"] = "Password And Confirm Password Must Match";
                    return View("Signup");
                }
                if (validatepas == true)
                {
                    SendEmail(Users.EmailAddress, pasw);
                    bool val = _DR.Validate(Users.EmailAddress,Users.PhoneNumber);
                    if (val == true)
                    {
                        TempData["ErrMsg"] = "User Already Exist ! Please Check Phone Or Email";

                        return View("Signup");
                    }
                    else if (val == false)
                    {  
                    password = EncrypPassword;
                    Users.PaswordVal = password;
                        //today
                        Users.EmploymentStatus_FK = 1;
                        Users.ReferralLevel = 1;
                    var Userid = _DM.InsertUser(Users);
                    if (Userid != 0)
                     {
           TempData["ErrMsg"] = "User Created Succesfully";
           Session["id"] = LoggedInEmail(Users.EmailAddress);
           Session["User"] = Session["id"];
           string email = Convert.ToString(Session["id"]);
           string InstFkEmail = _DR.getInstFKByEmail(email);
           Session["InstFkEmail"] = InstFkEmail;
           SendEmail(Users.EmailAddress,pasw);

                            /* Today */
                            UserRole UserRoles = new UserRole();
                            UserRoles.User_FK = Users.ID;
                            UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["Applicant"]);
                            UserRoles.IsVisible = 1;
                            _DM.InsertUserRoles(UserRoles);
                            
                          /* Today */
                            return RedirectToAction("MyApplicationsStatus", "Admin");
                            //today
                            // return RedirectToAction("index", "Admin");
                        }
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

        

        public string LoggedInEmail(string email)
        {
            try
            {
                string emails = email;
                return emails;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
         }

        public bool ValidatePassword(string Password, string ConfirmPass)
        {
            try
            {
                string value = Password;
                string value1 = ConfirmPass;
                if (value == value1)
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


        [HttpGet]
        public ActionResult RecoverPassword()
        {
            TempData["ErrMsg"] = "";
            TempData["SucMsg"] = "";
            return View();
        }

        [HttpPost]
        public ActionResult RecoverPassword(UvlotApplication.Classes.TableObjects.LoanApplication LoanApp)
        {
            try
            {
                if (LoanApp.EmailAddress != null)
                {
                    string Email = LoanApp.EmailAddress;
                    sendLink(Email);
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public void SendEmail(string user,string password)
        {
            try
            {

                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/WelcomeEmailNotification.html"));
                ////bodyTxt = bodyTxt.Replace("$UserName",user).Replace("$MerchantName", $"{lvm.Firstname} {lvm.Surname}").Replace("$LoanComment", $"{lvm.LoanComment}");
                //bodyTxt = bodyTxt.Replace("$UserName", user).Replace("$Password", password);
                bodyTxt = bodyTxt.Replace("$MerchantName", $"{user}").Replace("$Password", $"{password}").Replace("$EmailAddress", $"{user}");
                var msgHeader = $"Welcome to Casnownow Payroll Loan";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, user, null, null);

                
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }

        public void sendLink(String Email)
        {
            try
            {
                //LoanViewModel lvm = new LoanViewModel();
                //String email = lvm.AccountsModel.Email;
                User users = new DataAccessLayerT.DataManager.User();
                var result = _DR.checkEmail(Email);
                WebLog.Log("Email +" + result);
                if (result != null)
                {
                    WebLog.Log("Email +" + result);
                    users.EmailAddress = result;
                    WebLog.Log("Email +" + users.EmailAddress);
                    users.ID = _DR.selectUserIDs(users);
                    WebLog.Log("Email +" + users.ID);
                    if (users.ID != 0)
                    {
                        string encrypt = "";
                        try
                        {
                          encrypt = $"tK_{ Classes.Utility.RandomString(30).ToUpper()}" + users.ID;
                            users = _DR.getUser(Email);
                            WebLog.Log("users +" + users.EmailAddress);
                            string resetLink = ConfigurationManager.AppSettings["ResetPasswordLink"] + encrypt;
                            WebLog.Log("resetLink +" + resetLink);
                            string resetLink1 = "Click The Following Link:<a href='" + resetLink + "'>'" + resetLink + "'</a> to change your password";
                            WebLog.Log("resetLink1 +" + resetLink1);
                            WebLog.Log("resetLink: " + resetLink);
                            var bodyTxt = System.IO.File.ReadAllText(Server.MapPath("~/EmailNotifications/ResetPasswordEmailNotification.html"));
                            bodyTxt = bodyTxt.Replace("$MerchantName", $"{users.Firstname} {users.Lastname}");
                            bodyTxt = bodyTxt.Replace("$Message", $"{resetLink1}");
                            var msgHeader = $"Reset Your Password";
                            WebLog.Log("resetLink: " + resetLink);

                            WebLog.Log("bodyTxt:" + bodyTxt);

                        var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, users.EmailAddress, null, null);
                            //users.ResetPassword = encrypt;
                            users.Audit = encrypt;
                            // users.DateTim = MyUtility.getCurrentLocalDateTime();
                    
                    users.LastUpdated = MyUtility.getCurrentLocalDateTime();
                    _DM.UpdateUsers(users);
                    TempData["message"] = "Please Check Your Email For Password Reset Link";
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                            WebLog.Log(ex.Message.ToString());
                        }
                    }
                    else
                    {
                        TempData["message"] = "Please Try Again";
                    }
                }
                else
                {
                    TempData["message"] = "Account Does Not Exist";
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
            }
        }


        [HttpGet]
        public ActionResult ResetPassword()
        {
            try
            {
                TempData["ErrMsg"] = "";
                TempData["SucMsg"] = "";
                string value = Request.QueryString["value"];
                TempData["value"] = value;
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
             return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public void ResetPasswords(string value, DataAccessLayerT.DataManager.User users)
        {
            try
            {
                // if (Request.QueryString["value"] != null)
                if (value != "")
                {
                    // value = Request.QueryString["value"];
                    var result = _DR.checkValue(value);

                    if (result != null)
                    {
                        DateTime dtCreate = result.LastUpdated.Value;
                        DateTime dtNow = MyUtility.getCurrentLocalDateTime();
                        DateTime dtExp = dtCreate.AddMinutes(15);
                        if (dtNow > dtExp)
                        {
                            TempData["ErrMsg"] = "Password Reset Link Expired";
                        }
                        else
                        {
                            var user = _DR.getUsers(value.Trim());
                            users.ID = user.ID;

                            UpdatePassword(users);
                        }


                    }
                    else
                    {
                        var id = _DR.getUsers(value);
                        TempData["ErrMsg"] = "Invalid key please try again.";
                        return;
                    }

                }

                else
                {

                    TempData["ErrMsg"] = "Invalid Url";
                    return;
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
            }
        }



        public void UpdatePassword(DataAccessLayerT.DataManager.User users)
        {
            try
            {
                // LoanViewModel lvm = new LoanViewModel();
               // users.PaswordVal = lvm.AccountsModel.pasword;
               // users.confirmPassword = lvm.AccountsModel.confirmPassword;
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(users.PaswordVal, HashName.SHA256);
                users.PaswordVal = EncrypPassword;
                string value = "";
                users.Audit = value;
                _DM.UpdatePassword(users);
                TempData["SucMsg"] = "Password Successfully Update.";
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
            }

        }

        [HttpPost]
        public ActionResult ResetPassword(FormCollection form, LoanViewModel lvm, DataAccessLayerT.DataManager.User users)
        {
            try
            {
                if (users.PaswordVal != "")
                {
                    string value = Convert.ToString(form["value"]);
                    string password = users.PaswordVal;
                    string rpassword = Convert.ToString(form["rpassword"]); 

                    if(password != rpassword)
                    {
                        TempData["ErrMsg"] = "Password Does Not Match";
                        return View(users);
                    }
                    //string rpassword = lvm.AccountsModel.confirmPassword;

                    ResetPasswords(value, users);
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
        public ActionResult termscondition()
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



       // [HttpPost]
      /*  public ActionResult ResetPassword(FormCollection form)
        {
            try
            {
               
                string password = "";
                var confirmPassword = "";
                string EncrypPassword = "";
                User users = new User();
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    password = Convert.ToString(form["Password"]);
                    users.PaswordVal = Convert.ToString(form["newPassword"]);
                    confirmPassword = Convert.ToString(form["confirmPassword"]);
                    EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(password, HashName.SHA256);
                }
                if (confirmPassword != users.PaswordVal)
                {
                    TempData["ErrMsg"] = "New Password and Confirm Password Must Match";

                    return View();
                }
                else
                {
                    var NewEncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(users.PaswordVal, HashName.SHA256);
                    users.PaswordVal = NewEncrypPassword;
                    users.EmailAddress = Session["id"].ToString();
                    var valid = _DR.loggedIn(users.EmailAddress, EncrypPassword);
                    users.PaswordVal = NewEncrypPassword;

                    string Email = users.EmailAddress;
                    User id = _DR.getUser(Email);
                    users.ID = id.ID;
                    if (valid == true)
                    {

                        _DM.UpdatePassword(users);
                        TempData["ErrMsg"] = "Password Succesfully Updated";
                       
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Please Try Again";
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
        */
    }
}