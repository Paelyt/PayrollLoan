using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer.DataManager
{
    public class DataWriter
    {
       static  UvlotEntities uvDb = new UvlotEntities();



        public  LoanApproval CreateLoanApproval(LoanApproval instObj)
        {
            try
            {
                
                uvDb.LoanApprovals.Add(instObj);
                uvDb.SaveChanges();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            return instObj;
        }

        public static Institution CreateIntitution(Institution instObj)
        {
            try
            {
               // uvDb= new UvlotEntities();
                uvDb.Institutions.Add(instObj);
                uvDb.SaveChanges();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            return instObj;
        }

        

              public static EmployerLoanDetail CreateEmployerLoanDetails(EmployerLoanDetail instObj)
        {
            try
            {
                // uvDb= new UvlotEntities();
                uvDb.EmployerLoanDetails.Add(instObj);
                uvDb.SaveChanges();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            return instObj;
        }
        public static LoanApplication CreateLoanApplication(LoanApplication instObj)
        {
            try
            {
                // uvDb= new UvlotEntities();
                uvDb.LoanApplications.Add(instObj);
                uvDb.SaveChanges();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            return instObj;
        }



       
        public int InsertUser(User user)
        {

            try
            {


                uvDb.Users.Add(user);
                uvDb.SaveChanges();
                return user.ID;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.Count();
                 
            }

        }

        public int CreatePages(Page pages)
        {

            try
            {
                uvDb.Pages.Add(pages);
                uvDb.SaveChanges();

                return pages.PageID;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
                
            }

        }
        

        public int UpdateLoanApplication(LoanApplication LoanApp)
        {
            try
            {

                var original = uvDb.LoanApplications.Find(LoanApp.LoanRefNumber);


                if (original != null)
                {



                    original.ApplicationStatus_FK = LoanApp.ApplicationStatus_FK;
                   


                    uvDb.SaveChanges();
                }
                return original.ID;

            }


            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }

        }
        public void UpdatePassword(User user)
        {
            try
            {

                var original = uvDb.Users.Find(user.EmailAddress);


                if (original != null)
                {



                    original.PaswordVal = user.PaswordVal;
                    // original.confirmPassword = user.confirmPassword;


                    uvDb.SaveChanges();
                }

            }


            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
            }

        }

        public int CreateRole(Role roles)
        {

            try
            {
                uvDb.Roles.Add(roles);
                uvDb.SaveChanges();

                return roles.RoleId;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
               
            }

        }

        public string UpdateRole(Role roles)
        {
            try
            {
             
              var original  = (from a in uvDb.Roles
                                             where a.RoleId == roles.RoleId
                                             select a).FirstOrDefault();
                if (original != null)
                {
                   original.RoleDescription = roles.RoleDescription;
                    uvDb.SaveChanges();
                }
                return original.RoleId.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.StackTrace.ToString();
                
            }
        }

        public void InsertUserRoles(UserRole userrole)
        {

            try
            {
                uvDb.UserRoles.Add(userrole);
                uvDb.SaveChanges();


            }

            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
            }

        }
        
             public LoanApproval UpdateLoanApproval(LoanApproval LoanApp,LoanApplication LoanApplication)
           {
            try
            {
                var original = (from a in uvDb.LoanApprovals
                                where a.ID == LoanApp.ID
                                select a).FirstOrDefault();

                if (original != null)
                {
                    //original.LoanComment = LoanApp.LoanComment;
                    original.ApplicationStatus_FK = LoanApp.ApplicationStatus_FK;

                    uvDb.SaveChanges();
                }
                return original;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int UpdateLoanApp(LoanApplication LoanApp)
        {
            try
            {
                var original = (from a in uvDb.LoanApplications
                                where a.LoanRefNumber == LoanApp.LoanRefNumber
                                select a).FirstOrDefault();

                if (original != null)
                {
                    original.LoanComment = LoanApp.LoanComment;
                    original.ApplicationStatus_FK = LoanApp.ApplicationStatus_FK;
                  
                    uvDb.SaveChanges();
                }
                return original.ID;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.StackTrace.Count();
            }
        }
        public string UpdateProfile(User users)
        {
            try
            {
          var original = (from a in uvDb.Users
          where a.ID == users.ID select a).FirstOrDefault();

                if (original != null)
                {
                    original.Firstname = users.Firstname;
                    original.Lastname = users.Lastname;
                    //original.EmailAddress = users.EmailAddress;
                    original.PhoneNumber = users.PhoneNumber;
                   // original.PaswordVal = users.PaswordVal;
                    

                    uvDb.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.StackTrace.ToString();
            }
        }

        public void InsertPageRoles(PageAuthentication pagerole)
        {

            try
            {
                uvDb.PageAuthentications.Add(pagerole);
                uvDb.SaveChanges();


            }

            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
            }

        }



    }
}
