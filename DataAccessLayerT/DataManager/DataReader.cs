using DataAccessLayerT.Classes;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataAccessLayerT.DataManager
{
    public class DataReader
    {
        static UvlotEntities uvDb = new UvlotEntities();
        UserRole userrole = new UserRole();
        string connectionStrings = ConfigurationManager.AppSettings["ConnectionString"];
      
        public DataView RemitaTransactLog()
        {
            try
            {
                WebLog.Log("connectionStrings : " + connectionStrings);
               // WebLog.Log("PayTrxxEntities : " + con);
                DataSet ds;
                
                ds = SqlHelper.ExecuteDataset(connectionStrings, CommandType.StoredProcedure, "RemitaTransactLog");
                if (ds.Tables[0] == null)
                {
                    return null;
                }
              
                DataView dv = new DataView(ds.Tables[0]);
                return dv;
            }
            catch (Exception ex)
            {
               WebLog.Log(ex.Message.ToString(), ex.StackTrace.ToString());
                return null;
            }

        }


        public DataView DirectDebitTransactLog()
        {
            try
            {
                WebLog.Log("connectionStrings : " + connectionStrings);
                // WebLog.Log("PayTrxxEntities : " + con);
                DataSet ds;

                ds = SqlHelper.ExecuteDataset(connectionStrings, CommandType.StoredProcedure, "DirectDebitTransactLog");
                if (ds.Tables[0] == null)
                {
                    return null;
                }
                DataView dv = new DataView(ds.Tables[0]);
                return dv;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString(), ex.StackTrace.ToString());
                return null;
            }

        }

        public LoanApplication GetLP(string RefNum)
        {

            try
            {
                var LoanDisbursedDate = (from a in uvDb.LoanApplications where a.LoanRefNumber == RefNum && a.ApplicationStatus_FK == 6 select a).FirstOrDefault();
                //var RemitaGenerated = (from b in uvDb.PatnerTransactLogs where b.RefNum == RefNum select b).FirstOrDefault();

                //var diffDate = RemitaGenerated.DateCreated.Value.Subtract(LoanCreateDate.DateCreated.Value);

                // return (diffDate.ToString());
                if(LoanDisbursedDate.ID != 0)
                {
                    return null;
                }
                return LoanDisbursedDate;
               
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
       

        public float RepaymentEfficiency()
        {


            try
            {
                var DefaultLoans = (from a in uvDb.LoanLedgers where a.PaymentFlag_FK != 1 && a.TranxDate.Value.Month <= DateTime.Today.Month && a.TranxDate.Value.Year <= DateTime.Today.Year select a.Credit).Sum();

               // DefaultLoans = DefaultLoans / 100;
               // DefaultLoans = Math.Round((double)DefaultLoans, 4);
                return (float)DefaultLoans;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public List<Invoice> GetDuePaymentsByMonth(DateTime Date)
        {
            try
            {
                var LoansDue = (from s in uvDb.LoansLedgers
                                join v in uvDb.PatnerTransactLogs on s.RefNumber equals v.RefNum
                                where s.trnDate.Value.Month == Date.Date.Month && s.trnDate.Value.Year == Date.Date.Year && v.PatnerCode == "Remita"

                                select new Invoice
                                {
                                    Debit = (double)s.Debit,
                                    ReferenceNum = s.RefNumber,
                                    MandateId = v.mandateID,
                                    BankCode = v.BankCode,
                                    BankAcct = v.BankAcct,
                                    requestId = v.PatnerResponse,
                                }).ToList();

                if (LoansDue == null || LoansDue.Count == 0)
                {
                    return null;
                }
                return LoansDue;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public float GetAbandonrate()
        {
            try
            {
                int DateCap = 30;
               
               
                var AbandonApplication = (from a in uvDb.PatnerTransactLogs join b in uvDb.LoanApplications
                                          on a.RefNum equals b.LoanRefNumber
                                          where DateCap > System.Data.Entity.DbFunctions.DiffDays(DateTime.Now, a.DateCreated)  select a).Count();

                // var TotalApplication = getCountOFLoansNotDisbursed();
                //var  AbandonApplications =Math.Round(AbandonApplication/TotalApplication,4);
                // AbandonApplications = AbandonApplications * 100;
                // return (float)AbandonApplications;
                return AbandonApplication;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


       public float getCountOFLoans()
        {
            try
            {
                var AppDisbursed = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == 6 select a).Count();
             

                return AppDisbursed;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public float getCountOFLoansNotDisbursed()
        {
            try
            {
                // var AppSubmitted = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == 1 select a).Count();
                var AppSubmitted = (from a in uvDb.LoanApplications where a.IsVisible == 1 select a).Count();

                return AppSubmitted;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public float GetTotalAcceptanceRate()
        {


            try
            {
                // var AppDisbursed = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == 6  select a).Count();
                var AppDisbursed = getCountOFLoans();
                //var AppSubmitted = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == 1 select a).Count();
                var AppSubmitted = getCountOFLoansNotDisbursed();
                var Total = Math.Round(AppDisbursed / AppSubmitted,4);
               // decimal Result = Math.Round((decimal)Total,2);

                return (float)Total;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public string KpiLoanAppToRemitaGeneration(string RefNum)
        {


            try
            {
                var LoanCreateDate = (from a in uvDb.LoanApplications where a.LoanRefNumber == RefNum select a).FirstOrDefault();
                var RemitaGenerated = (from b in uvDb.PatnerTransactLogs where b.RefNum == RefNum select b).FirstOrDefault();

                var diffDate = RemitaGenerated.DateCreated.Value.Subtract(LoanCreateDate.DateCreated.Value);

                return (diffDate.ToString());
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        

         public string GetRefNum(int value)
        {


            try
            {
                var RefNum = (from a in uvDb.LoanApplications where a.ID == value select a.LoanRefNumber).FirstOrDefault();

                if (RefNum == null)
                {
                    return null;
                }


                return RefNum;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public User checkValue(string value)
        {
            

            try
            {
                var resetPass = (from a in uvDb.Users where a.Audit == value select a).FirstOrDefault();

                if (resetPass == null)
                {
                    return null;
                }


                return resetPass;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public User getUsers(string value)
        {
            try
            {
                var user = (from a in uvDb.Users where a.Audit == value select a).FirstOrDefault();

                if (user == null)
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
                return null;
            }


        }

        public string checkEmail(string email)
        {


            try
            {
                var emails = (from a in uvDb.Users where a.EmailAddress == email select a.EmailAddress).FirstOrDefault();

                if (emails == null)
                {
                    return null;
                }


                return emails;
            }
            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
                return null;
            }


        }
        
             public List<AppLoan> GetDocForApprovalByRefNum(string Refid)
            {
            try
            {
                var record = (from a in uvDb.LoanApplications
                              join b in uvDb.DocUploads on a.LoanRefNumber equals b.ReferenceNum
                              where a.ApplicationStatus_FK == 1 && b.ReferenceNum == Refid
                              select new AppLoan
                              {
                                  Firstname = a.Firstname + a.Surname,
                                  Organization = a.Organization,
                                  DocumentPath = b.DocImagePath,
                                  DocumentName = b.DocName,
                                  LoanRefNumber = a.LoanRefNumber,
                                  ID = a.ID,
                              });

                if (record == null)
                {
                    return null;
                }
                return record.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Signature> checkSignature(string RefNum)
        {
            try
            {
                var rec = (from a in uvDb.Signatures where a.LoanRefNum == RefNum select a).ToList();

                if(rec == null)
                {
                    return null;
                }
                return rec.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<AppLoan> GetDocForApproval(string Refid)
        {
            try
            {
                var record = (from a in uvDb.LoanApplications
                              join b in uvDb.DocUploads on a.LoanRefNumber equals b.ReferenceNum
                              where a.ApplicationStatus_FK == 1 && b.ReferenceNum ==Refid
                              select new AppLoan
                              {
                                  Firstname = a.Firstname + a.Surname,
                                  Organization = a.Organization,
                                  DocumentPath = b.DocImagePath,
                                  DocumentName = b.DocName,
                                  LoanRefNumber = a.LoanRefNumber,
                                  ID = a.ID,
                              });

                if(record == null)
                {
                    return null;
                }
                return record.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<AppLoan> GetDocForApprovals()
        {
            try
            {
                var record = (from a in uvDb.LoanApplications
                              join b in uvDb.DocUploads on a.LoanRefNumber equals b.ReferenceNum
                              where a.ApplicationStatus_FK == 1
                              select new AppLoan
                              {
                                  Firstname = a.Firstname ,
                                  Surname = a.Surname,
                                  Organization = a.Organization,
                                  DocumentPath = b.DocImagePath,
                                  DocumentName = b.DocName,
                                  LoanRefNumber = a.LoanRefNumber,
                               // ApplicationStatus = a.ApplicationStatus_FK.Value,
                                  ID = a.ID,
                              }).Distinct();

                if (record == null)
                {
                    return null;
                }
                return record.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<AppLoan> GetSignedofferLetters()
        {
            try
            {
                var record = (from a in uvDb.LoanApplications
                              join b in uvDb.Signatures on a.LoanRefNumber equals b.LoanRefNum
                              where a.ApplicationStatus_FK == 1
                              select new AppLoan
                              {
                                  Firstname = a.Firstname,
                                  Surname = a.Surname,
                                  Organization = a.Organization,
                                  //DocumentPath = b.DocImagePath,
                                  //DocumentName = b.DocName,
                                  LoanRefNumber = a.LoanRefNumber,
                                  // ApplicationStatus = a.ApplicationStatus_FK.Value,
                                  ID = a.ID,
                              }).Distinct();

                if (record == null)
                {
                    return null;
                }
                return record.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<LoanApplication> GetApplicantName(string prefix)
        {
            try
            {
                var AppName = (from a in uvDb.LoanApplications
                               where a.Firstname.StartsWith(prefix)
                               select a).ToList();
                //select new  
                //{
                //    label = a.Firstname,
                //    val = a.Surname,
                //}).FirstOrDefault();

                if (AppName == null)
                {
                    return null;
                }
                return AppName ;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public int CountDocsForReview(string Refid, out string respMessage)
        {
            try
            {
                var valid = (from a in uvDb.DocUploads where a.ReferenceNum == Refid  select a).ToList();

                if (valid.Count == 3)
                {
                    respMessage = "Maximum Number Of Uploads is 3";
                    return valid.Count;
                }
                respMessage = "";
                return valid.Count;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                respMessage = "";
                return 0;
            }
        }


        public bool CheckAppStatusWithRef(string Refid, out string respMessage)
        {
            try
            {
                var valid = (from a in uvDb.LoanApplications where a.LoanRefNumber == Refid && a.ApplicationStatus_FK == 1 select a).FirstOrDefault();

                if(valid == null)
                {
                    respMessage = "Invalid Reference Number OR Application Not Yet Approved";
                    return false;
                }
                respMessage = "";
                return true;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                respMessage = "";
                return false;
            }
        }

       public List<AppLoan> GetDocument(int Users,string RefNum)
        {
            try
            {
                var records = (from a in uvDb.DocUploads join b in uvDb.LoanApplications on a.ReferenceNum equals b.LoanRefNumber where a.User_FK == Users && a.ReferenceNum == RefNum
                                select new AppLoan
                               {
                                   Firstname = b.Firstname + b.Surname,
                                   IdentficationNumberImage = a.DocImagePath,
                                   imageName = a.DocName,
                                   LoanRefNumber = a.ReferenceNum,
                                    ApplicationDatevalue = a.Cols1,
                               }).ToList();

                              // select a).ToList();
         
                if(records == null)
                {
                    return null;
                }
                return records;

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int selectUserIDs(User users)
        {

            try
            {
                var emails = (from a in uvDb.Users where a.EmailAddress == users.EmailAddress select a).FirstOrDefault();

                if (emails == null)
                {
                    return 0;
                }
                users.ID = Convert.ToInt16(emails.ID);
                return users.ID;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public int getRepaymentByName(string value)
        {
            try
            {
                var Repment = (from a in uvDb.RepaymentMethods where a.Name == value select a.ID).FirstOrDefault();
                if(Repment == 0)
                {
                    return 0;
                }
                return Repment;
            }

            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public List<LoansLedger> getLedgerRecord(string Refnum)
        {
            try
            {
                var record = (from a in uvDb.LoansLedgers where a.RefNumber == Refnum && a.IsVisible == 1 select a).ToList();

                if (record == null)
                {
                    return null;
                }
                return record.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public AppLoan LoanDetails(string Refid)
        {
            try
            {
                var LoanDetails = (from a in uvDb.LoanApplications
                                   join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                     join c in uvDb.ApplicationStatus on
                     a.ApplicationStatus_FK equals c.ID
                      where a.LoanRefNumber == Refid
                                   //where a.ApplicationStatus_FK == 1 || a.ApplicationStatus_FK == 2 && a.LoanRefNumber == Refid

                                   select new AppLoan
                                   {
                                       ID = a.ID,
                                       LoanRefNumber = a.LoanRefNumber,
                                       Firstname = a.Firstname,
                                       Surname = a.Surname,
                                       LoanTenure = (int)a.LoanTenure,
                                       LoanAmount = (double)a.LoanAmount,
                                       ApplicationStatus = c.Name,
                                       PhoneNumber = a.PhoneNumber,
                                       EmailAddress = a.EmailAddress,
                                      // interestRate = a 
                                       //  ApplicationStatus = a.ApplicationStatus_FK == 2 ? "Recommended" : a.ApplicationStatus_FK == 1 ? "Approved" : "", 
                                   }).FirstOrDefault();

                if (LoanDetails == null)
                {
                    return null;
                }
                return LoanDetails;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public AppLoan StudentLoanDetails(string Refid)
        {
            try
            {
                var LoanDetails = (from a in uvDb.LoanApplications
                                   join b in uvDb.StudentLoanDetails on a.ID equals b.LoanApplication_FK join c in uvDb.ApplicationStatus on 
                                   a.ApplicationStatus_FK equals c.ID
                                   where a.LoanRefNumber == Refid
                                  
                                   select new AppLoan
                                   {
                                       ID = a.ID,
                                       LoanRefNumber = a.LoanRefNumber,
                                       Firstname = a.Firstname,
                                       Surname = a.Surname,
                                       LoanTenure = (int)a.LoanTenure,
                                       LoanAmount = (double)a.LoanAmount,
                                       ApplicationStatus = c.Name,
                                       PhoneNumber = a.PhoneNumber,
                                       EmailAddress = a.EmailAddress,
                     //  ApplicationStatus = a.ApplicationStatus_FK == 2 ? "Recommended" : a.ApplicationStatus_FK == 1 ? "Approved" : "", 
                                   }).FirstOrDefault();

                if (LoanDetails == null)
                {
                    return null;
                }
                return LoanDetails;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public DataAccessLayerT.Classes.LoanApplication getApplicantEmpInfo(int id)
        {
            try
            {
                var EmpInfo = (from a in uvDb.LoanApplications 
                 join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK join c in uvDb.Institutions on a.Institution_FK equals c.ID where a.ID == id
                select new DataAccessLayerT.Classes.LoanApplication
                {
                ID = a.ID,
                SalaryAmount = (double)b.NetMonthlyIncome,
                LOS = (int)b.LengthOfServiceInMth,
                ApplicantID = a.ApplicantID,
                Occupation = b.Occupation,
                 Designation= b.Designation,
                Department = b.Department,
                EmployeeStatusFK= (int)b.EmploymentStatus_FK,
                LoanTenure = (int)a.LoanTenure,
                Organization = c.InstitutionCode,
                LoanAmount = (double)a.LoanAmount,
                OfficialEmail = b.OfficialEmailAddress,
            }).FirstOrDefault();

                if (EmpInfo == null)
                {
                    return EmpInfo;
                }
                return EmpInfo;
                               }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public AppLoan GetLoanApplication(string LoanID, int AppstatFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join z in uvDb.Guarantors on a.ID equals z.LoanApplication_FK
                               join j in uvDb.Institutions on a.Institution_FK equals j.ID
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               //join i in uvDb.Banks on a.BankCode equals i.Code
                               join i in uvDb.Banks on a.BankCode equals i.FlutterWaveBankCode
                               join m in uvDb.EmployerLoanDetails on a.ID equals m.LoanApplication_FK
                               join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
                               where a.LoanRefNumber == LoanID && a.ApplicationStatus_FK == AppstatFk
                               select new AppLoan
                               {
                                   LengthOfService = (int)m.LengthOfServiceInYrs,
                                   LengthOfServiceMonth = (int)m.LengthOfServiceInMth,
                                   ID = a.ID,
                                   MeansOfIdentifications = e.Name,
                                   MeansOfID_FK = (int)a.MeansOfID_FK,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   Title_FK = (int)a.Title_FK,
                                   NigerianStates = f.Name,
                                   StateofResidence_FK = (int)a.StateofResidence_FK,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   Gender_FK = (int)a.Gender_FK,
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus = d.Name,
                                   MaritalStatus_FK = (int)a.MaritalStatus_FK,
                                   LoanProduct = h.LoanProduct1,
                                   AppStat = (int)a.ApplicationStatus_FK,
                                   Salary = m.NetMonthlyIncome.Value,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = i.Name,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   Organization = a.Organization,
                                   Designation = m.Designation,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   Occupation = m.Occupation,
                                   Department = m.Department,
                                   Repayment = x.Name,
                                   LoanComment = a.LoanComment,
                                   EmployeeStatus = es.Name,
                                   GuarContact = z.ContactAddress,
                                   GuarEmail = z.EmailAddress,
                                   GuarRelationship = z.PhoneNumber,
                                   GuarPhone = z.PhoneNumber,
                                   GuarSurname = z.Surname,
                                   GuarOthernames = z.Othernames,
                                   InstitutionCode = j.InstitutionCode,
                                   IdentficationNumberImage = a.MeansOfIDFilePath,
                                   AccomodationType_FK = (int)a.AccomodationType_FK,
                                   RepaymentMethod_FK = (int)a.RepaymentMethod_FK,
                                   institutionFk = (int)a.Institution_FK,
                                   bank_codes = a.RemitaBankCode,
                                   flutterwaveBanCode = a.BankCode,
                                   DisburseDate = (DateTime)a.DateModified,
                               }).FirstOrDefault();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<Invoice> getAllLedRecords(string RefNum, DateTime startDate)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                               // join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber
                               join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber
                               join d in uvDb.Institutions on a.Institution_FK equals d.ID

                                where c.TranxDate.Value.Month <= startDate.Month && c.TranxDate.Value.Year <= startDate.Year && c.IsVisible == 1 && c.PaymentFlag_FK != 1
                                && c.RefNumber == RefNum
                                select new Invoice
                               {
                                   LID = (int)c.ID,
                                   Company = a.Organization,
                                   Location = d.InstitutionAddress,
                                   Fullname = a.Firstname + " " + " " + a.Surname,
                                   DisburseDate = (DateTime)a.DateModified,
                                   RepaymentDate = (DateTime)c.TranxDate,
                                   Interestrate = 0,//(double)e.InterestRate,
                                   Tenure = (int)a.LoanTenure,
                                   InterestPricipalDue = Math.Ceiling((double)c.Debit),
                                   ReferenceNum = a.LoanRefNumber,
                                   Credit = (double)c.Credit,
                                   Debit = (double)c.Debit,
                                   EmployeeID = a.ApplicantID,
                               }
                               ).Distinct().ToList();

                if (Invoice == null || Invoice.Count == 0)
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Invoice> getInvoice(DateTime startDate,DateTime EndDate, int InstitutionFk)
           {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                               join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber join d in uvDb.Institutions on a.Institution_FK equals d.ID
                           //join e in uvDb.LoanInterestRates on d.ID equals   e.Institution_FK 
                      where c.TranxDate.Value.Month == startDate.Month && c.TranxDate.Value.Year == startDate.Year &&  c.Credit == 0 && a.Institution_FK == InstitutionFk && c.PaymentFlag_FK == 0
                      && c.IsVisible == 1
                               // c.PaymentFlag_FK != 1 && c.PaymentFlag_FK != 2
                               //&& c.TranxDate <= EndDate 
                               select new Invoice
                               {
                               LID = (int)c.ID,
                               Company = a.Organization,
                               Location = d.InstitutionAddress,
                               Fullname = a.Firstname + " " + " " + a.Surname,
                               DisburseDate = (DateTime)a.DateModified,
                               RepaymentDate = (DateTime)c.TranxDate,
                               Interestrate = 0,//(double)e.InterestRate,
                               Tenure = (int)a.LoanTenure,
                               InterestPricipalDue = Math.Ceiling((double)c.Debit),
                               ReferenceNum = a.LoanRefNumber,
                               Credit = (double)c.Credit,
                               EmployeeID = a.ApplicantID,
                               }
                               ).Distinct().ToList();

                if(Invoice == null || Invoice.Count == 0)
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<Invoice> getInvoiceR(DateTime startDate, DateTime EndDate, int InstitutionFk)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                               join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber
                               join d in uvDb.Institutions on a.Institution_FK equals d.ID
                               //join e in uvDb.LoanInterestRates on d.ID equals   e.Institution_FK 
                               where c.TranxDate.Value.Month == startDate.Month && c.TranxDate.Value.Year == startDate.Year && c.Credit > 0 && a.Institution_FK == InstitutionFk && c.PaymentFlag_FK == 1
                               && c.IsVisible == 1
                               // c.PaymentFlag_FK != 1 && c.PaymentFlag_FK != 2
                               //&& c.TranxDate <= EndDate 
                               select new Invoice
                               {
                                   LID = (int)c.ID,
                                   Company = a.Organization,
                                   Location = d.InstitutionAddress,
                                   Fullname = a.Firstname + " " + " " + a.Surname,
                                   DisburseDate = (DateTime)a.DateModified,
                                   RepaymentDate = (DateTime)c.TranxDate,
                                   Interestrate = 0,//(double)e.InterestRate,
                                   Tenure = (int)a.LoanTenure,
                                   InterestPricipalDue = Math.Ceiling((double)c.Debit),
                                   ReferenceNum = a.LoanRefNumber,
                                   Credit = (double)c.Credit,
                                   EmployeeID = a.ApplicantID,
                               }
                               ).Distinct().ToList();

                if (Invoice == null || Invoice.Count == 0)
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<AppLoan> GetKpi(int ID)
        {
            try {
                var Rec = (from a in uvDb.LoanApplications
                           join b in uvDb.LoanApprovals on a.ID equals b.LoanApplication_FK
                           where a.ID == ID
                           select new AppLoan
                           {
                               LoanRefNumber = a.LoanRefNumber,
                               ApplicationDate = (DateTime)a.DateCreated,
                               ApplicationApprove = (DateTime)b.DateCreated,
                               LoanComment = b.Comment
                               

                           }).ToList();

                if (Rec == null || Rec.Count == 0)
                {
                    return null;
                }

                return Rec.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<LoanApplication> GetDIsbursedApplcation(int Appfk)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == Appfk select a

                               /*select new Invoice
                               {
                                   LID = (int)a.ID,
                                   Company = a.Organization,
                                   Location = a.ContactAddress,
                                   Fullname = a.Firstname + " " + " " + a.Surname,
                                   DisburseDate = (DateTime)a.DateModified,
                                   // RepaymentDate = (DateTime)c.TranxDate,
                                   Interestrate = 0,//(double)e.InterestRate,
                                   Tenure = (int)a.LoanTenure,
                                   InterestPricipalDue = Math.Ceiling((double)a.LoanAmount),
                                   ReferenceNum = a.LoanRefNumber,
                                   // Credit = (double)c.Credit,
                                   EmployeeID = a.ApplicantID,
                               }*/
                               ).Distinct().ToList();

                if (Invoice == null || Invoice.Count == 0)
                {
                    return null;
                }
                return Invoice.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
        public List<Invoice> getEmployees(int InstitutionFk)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               //join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                               //join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber
                               //join d in uvDb.Institutions on a.Institution_FK equals d.ID
                               //where c.TranxDate.Value.Month == startDate.Month && c.TranxDate.Value.Year == startDate.Year && c.Credit == 0 && a.Institution_FK == InstitutionFk && c.PaymentFlag_FK == 0
                               //&& c.IsVisible == 1
                               where a.ApplicationStatus_FK == 6 && a.DateModified != null && a.Institution_FK == InstitutionFk
                              
                               select new Invoice
                               {
                                   LID = (int)a.ID,
                                   Company = a.Organization,
                                   Location = a.ContactAddress,
                                   Fullname = a.Firstname + " " + " " + a.Surname,
                                   DisburseDate = (DateTime)a.DateModified,
                                  // RepaymentDate = (DateTime)c.TranxDate,
                                   Interestrate = 0,//(double)e.InterestRate,
                                   Tenure = (int)a.LoanTenure,
                                   InterestPricipalDue = Math.Ceiling((double)a.LoanAmount),
                                   ReferenceNum = a.LoanRefNumber,
                                  // Credit = (double)c.Credit,
                                   EmployeeID = a.ApplicantID,
                               }
                               ).Distinct().ToList();

                if (Invoice == null || Invoice.Count == 0)
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Invoice> getInvoices(DateTime startDate, DateTime EndDate, int InstitutionFk)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                               join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber
                              // join c in uvDb.LoansLedgers on a.LoanRefNumber equals c.RefNumber
                               join d in uvDb.Institutions on a.Institution_FK equals d.ID
                               //join e in uvDb.LoanInterestRates on d.ID equals   e.Institution_FK 
                                 where c.TranxDate.Value.Month == startDate.Month && c.TranxDate.Value.Year == startDate.Year && a.Institution_FK == InstitutionFk &&
                                 c.PaymentFlag_FK != 1 && c.PaymentFlag_FK != 2 && c.IsVisible == 1
                              
                             
                               select new Invoice
                               {
                                   LID = (int)c.ID,
                                   Company = a.Organization,
                                   Location = d.InstitutionAddress,
                                   Fullname = a.Firstname + " " + " " + a.Surname,
                                   DisburseDate = (DateTime)a.DateModified,
                                   RepaymentDate = (DateTime)c.TranxDate,
                                   Interestrate = 0,//(double)e.InterestRate,
                                   Tenure = (int)a.LoanTenure,
                                   InterestPricipalDue = Math.Ceiling((double)c.Debit),
                                   ReferenceNum = a.LoanRefNumber,
                                   Credit = (double)c.Credit,
                                   EmployeeID = a.ApplicantID,
                               }
                               ).Distinct().ToList();

                if (Invoice == null || Invoice.Count == 0)
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<Invoice> getPaymentLog(DateTime startDate, DateTime EndDate, int InstitutionFk)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               join c in uvDb.LedgerTransacts on a.LoanRefNumber equals c.ReferenceNum
                                   where c.cols == InstitutionFk
                                   && c.DatePaid >= startDate
                                   && c.DatePaid <= EndDate
                               orderby c.DatePaid

                               select new Invoice
                               {
                                   LID = (int)c.ID,
                                   Company = a.Organization,
                                   Fullname = a.Firstname + " " + " " + a.Surname,
                                   RepaymentDate = (DateTime)c.DueDate,
                                   DisburseDate = (DateTime)c.DatePaid,
                                   ReferenceNum = a.LoanRefNumber,
                                   Credit = (double)c.Credit,
                                   EmployeeID = a.ApplicantID,
                               }
                               ).Distinct().ToList();

                if (Invoice == null )
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<Invoices> getCheckedExcelInvoice(IEnumerable<int> ID)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                               join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber
                               join d in uvDb.Institutions on a.Institution_FK equals d.ID where ID.Contains(c.ID)
                             
                             
                               select new Invoices
                               {
                                 
                                   Company = a.Organization,
                                   Location = d.InstitutionAddress,
                                   Fullname = a.Firstname + " " + " " + a.Surname,
                                   DisburseDate = (DateTime)a.DateModified,
                                   RepaymentDate = (DateTime)c.TranxDate,
                                   Tenure = (int)a.LoanTenure,
                                   InterestPricipalDue = Math.Ceiling((double)c.Debit),
                                   EmployeeID = a.ApplicantID,
                               }
                               ).Distinct().ToList();

                if (Invoice == null || Invoice.Count == 0)
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public LoanLedger getDateTime(string id)
        {
            try
            {
                int ids = Convert.ToInt16(id);
                var Datet = (from a in uvDb.LoanLedgers where a.ID == ids select a).FirstOrDefault();
                if(Datet == null)
                {
                    return null;
                }
                return Datet;
            }

            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Invoice> getOutstandingInvoice(DateTime startDate, DateTime EndDate, int InstitutionFk)
        {
            try
            {
                var Invoice = (from a in uvDb.LoanApplications
                               join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                               join c in uvDb.LoanLedgers on a.LoanRefNumber equals c.RefNumber
                               join d in uvDb.Institutions on a.Institution_FK equals d.ID
                          where c.TranxDate.Value.Month < startDate.Month && c.TranxDate.Value.Year <= startDate.Year && c.Credit == 0 && a.Institution_FK == InstitutionFk
                           
                               select new Invoice
                               {
                                   LID = (int)c.ID,
                                   Company = a.Organization,
                                   Location = d.InstitutionAddress,
                                   Fullname = a.Firstname + a.Surname,
                                   DisburseDate = (DateTime)a.DateModified,
                                   RepaymentDate = (DateTime)c.TranxDate,
                                   Interestrate = 0,//(double)e.InterestRate,
                                   Tenure = (int)a.LoanTenure,
                                   InterestPricipalDue = (double)c.Debit,
                                   ReferenceNum = a.LoanRefNumber,
                                   Credit = (double)c.Credit,
                               }
                               ).OrderByDescending(c=>c.RepaymentDate).Distinct().ToList();

                if (Invoice == null)
                {
                    return null;
                }
                return Invoice;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


       public List<string> GetNextLevelUser(int roleid)
        {
            try
            {
                var useremails = (from a in uvDb.Users
                                  join b in uvDb.UserRoles on a.ID equals b.User_FK
                                  where b.Role_FK == roleid
                                  select a.EmailAddress).ToList();

                if(useremails == null)
                {
                    return null;
                }
                return useremails;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public Classes.LoanApplication GetLoanApplicationByRefNum(string LoanID)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications

                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               //join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               //join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               //join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               //join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               //join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               //join i in uvDb.Banks on a.BankCode equals i.Code
                               join m in uvDb.EmployerLoanDetails on a.ID equals m.LoanApplication_FK
                               //join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID
                               join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
                               where a.LoanRefNumber == LoanID 
                               select new Classes.LoanApplication
                               {
                                   ID = a.ID,
                                   MeansOfID_FK = (int)a.MeansOfID_FK,
                                   AccountName = a.AccountName,
                                   Title_FK = (int)a.Title_FK,
                                   StateofResidence_FK = (int)a.StateofResidence_FK,
                                   LGA_FK = (int)a.LGA_FK,
                                   //LGAs = g.Name,
                                   Gender_FK = (int)a.Gender_FK,
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus_FK = (int)a.MaritalStatus_FK,
                                  // LoanProduct = h.LoanProduct1,
                                   ApplicationStatus_FK = (int)a.ApplicationStatus_FK,
                                   SalaryAmount = (double)m.NetMonthlyIncome,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = a.BankCode,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   //DateOfBirth = a.DateOfBirth.,
                                   DateModified = a.DateModified,
                                   DateCreated = a.DateCreated,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = (double)a.LoanAmount,
                                   Organization = a.Organization,
                                   Designation = m.Designation,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   Occupation = m.Occupation,
                                   Department = m.Department,
                                   RepaymentMethod_FK = (int)a.RepaymentMethod_FK,
                                   LoanComment = a.LoanComment,
                                   EmployeeStatus = es.Name,
                                   AccomodationType_FK = (int)a.AccomodationType_FK,
                                   Contract = (int)m.EmploymentStatus_FK,
                                   LOS = (int)m.LengthOfServiceInYrs,
                                   OfficialEmail = m.OfficialEmailAddress,
                                   LOSMnth = (int)m.LengthOfServiceInMth,
                                   institutionFk = (int)a.Institution_FK
                                   // MeanOFIDFIlePath = a.MeansOfIDFilePath,
                                 
                               }).FirstOrDefault();

                if (AppLoan == null)
                {
                   return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public AppLoan GetStudentLoanApplication(string LoanID, int AppstatFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications

                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               join i in uvDb.Banks on a.BankCode equals i.Code
                               join m in uvDb.StudentLoanDetails on a.ID equals m.LoanApplication_FK
                               join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID
                               //join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
                               where a.LoanRefNumber == LoanID && a.ApplicationStatus_FK == AppstatFk
                               select new AppLoan
                               {
                                   ID = a.ID,
                                   MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   NigerianStates = f.Name,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus = d.Name,
                                   LoanProduct = h.LoanProduct1,
                                   AppStat = (int)a.ApplicationStatus_FK,
                                  // Salary = m.NetMonthlyIncome.Value,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = i.Name,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   Organization = a.Organization,
                                   //Designation = m.Designation,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,

                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                  // Occupation = m.Occupation,
                                   Department = m.Department,
                                   Repayment = x.Name,
                                   LoanComment = a.LoanComment,
                                  // EmployeeStatus = es.Name,
                               }).FirstOrDefault();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        //public AppLoan GetLoanApplicationDisburseLevel(string LoanID, int AppstatFk)
        //{
        //    try
        //    {
        //        var AppLoan = (from a in uvDb.LoanApplications

        //                       join c in uvDb.Titles on a.Title_FK equals c.ID
        //                       join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
        //                       join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
        //                       join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
        //                       join g in uvDb.LGAs on a.LGA_FK equals g.ID
        //                       join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
        //                       join i in uvDb.Banks on a.BankCode equals i.Code
        //                       join m in uvDb.EmployerLoanDetails on a.ID equals m.LoanApplication_FK
        //                       join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID
        //                       join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID 
        //                       join ln in uvDb.LoanApprovals on a.ID equals ln.LoanApplication_FK
        //                       where a.LoanRefNumber == LoanID && a.ApplicationStatus_FK == AppstatFk
        //                       select new AppLoan
        //                       {
        //                           ID = a.ID,
        //                           MeansOfIdentifications = e.Name,
        //                           AccountName = a.AccountName,
        //                           Title = c.Name,
        //                           NigerianStates = f.Name,
        //                           LGAs = g.Name,
        //                           Gender = a.Gender_FK == 1 ? "Male" : "Female",
        //                           LoanRefNumber = a.LoanRefNumber,
        //                           MaritalStatus = d.Name,
        //                           LoanProduct = h.LoanProduct1,
        //                           AppStat = (int)a.ApplicationStatus_FK,
        //                           Salary = (float)m.NetMonthlyIncome,
        //                           AccountNumber = a.AccountNumber,
        //                           ApplicantID = a.ApplicantID,
        //                           BankCode = i.Name,
        //                           BVN = a.BVN,
        //                           ClosestBusStop = a.ClosestBusStop,
        //                           ContactAddress = a.ContactAddress,
        //                           DateOfBirth = a.DateOfBirth,
        //                           EmailAddress = a.EmailAddress,
        //                           ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
        //                           ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
        //                           ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
        //                           Firstname = a.Firstname,
        //                           IdentficationNumber = a.IdentficationNumber,
        //                           LoanAmount = a.LoanAmount.Value,

        //                           LoanTenure = a.LoanTenure.Value,
        //                           NOK_EmailAddress = a.NOK_EmailAddress,
        //                           NOK_FullName = a.NOK_FullName,
        //                           NOK_HomeAddress = a.NOK_HomeAddress,
        //                           NOK_PhoneNumber = a.NOK_PhoneNumber,
        //                           NOK_Relationship = a.NOK_Relationship,
        //                           Organization = a.Organization,
        //                           PhoneNumber = a.PhoneNumber,
        //                           ValueDate = a.ValueDate,
        //                           ValueTime = a.ValueTime,
        //                           Landmark = a.Landmark,
        //                           Othernames = a.Othernames,
        //                           Surname = a.Surname,
        //                           Occupation = m.Occupation,
        //                           Department = m.Department,
        //                           Repayment = x.Name,
        //                           LoanComment = a.LoanComment,
        //                           EmployeeStatus = es.Name,
        //                       }).FirstOrDefault();

        //        if (AppLoan == null)
        //        {

        //            return null;
        //        }

        //        return AppLoan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}


        public List<AppLoan> GetLoanComment(int AppID)
        {
            try
            {
                var Comment = (from a in uvDb.LoanApprovals
                           join u in uvDb.Users on a.CommentBy equals u.ID
                               where a.LoanApplication_FK == AppID
                               select new AppLoan
                               {
                                   ID = a.ID,
                                   LoanComment = a.Comment,
                                   Firstname = u.Firstname,
                                   ValueDate = a.ValueDate,

                               }).ToList();


                if (Comment == null)
                {
                    return null;
                }

                return Comment.ToList();

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public double getInterestRate(int value)
        {
            try
           {
                var InterestRate = (from a in uvDb.LoanInterestRates where a.LoanTenure == value select a).FirstOrDefault();

                if(InterestRate == null)
                {
                    return 0;
                }
                var ALlRecords = uvDb.LoanInterestRates;
                return InterestRate.InterestRate;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public int getInstFK(string RefNum)
        {
            try
            {
                var InstFk = (from a in uvDb.LoanApplications where a.LoanRefNumber == RefNum select a.Institution_FK).FirstOrDefault();
                if(InstFk == null)
                {
                    return 0;
                }
                return (int)InstFk;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public List<AppLoan> GetLoanComments(int loanFk)
        {
            try
            {
                var comment = (from a in uvDb.LoanApprovals
                               join b in uvDb.Users
                               on a.CommentBy equals b.ID
                               where a.LoanApplication_FK == loanFk
                               select new AppLoan
                               {
                                   AccountName = b.EmailAddress,
                                   LoanComment = a.Comment,
                                   ApplicationDate = a.DateCreated.Value,
                               }).ToList();

                if(comment == null)
                {
                    return null;
                }

                return comment.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<AppLoan> GetAllLoanComments(List<int> loanFk)
        {
            try
            {
                var comment = (from a in uvDb.LoanApprovals
                               join b in uvDb.Users
                               on a.CommentBy equals b.ID
                               where loanFk.Contains(a.ID)
                               select new AppLoan
                               {
                                   AccountName = b.EmailAddress,
                                   LoanComment = a.Comment,
                                   ApplicationDate = a.DateCreated.Value,
                               }).ToList();

                if (comment == null)
                {
                    return null;
                }

                return comment.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string getInstFKByEmail(string Email)
        {
            try
            {

                // var query = "SELECT * from [User] u inner join Institution i on u.PinNumber = i.ID  WHERE u.EmailAddress = '" + Email + "'  ";
                string InstEmail = "";
                using (var context = new UvlotEntities())
                {
                   InstEmail = uvDb.Database.SqlQuery<string>("SELECT i.ContactEmailAddress from [User] u inner join Institution i on u.PinNumber = i.ID  WHERE u.EmailAddress = '" + Email + "'  ").FirstOrDefault();
                }
                if (InstEmail == null)
                {
                    return null;
                }
                return InstEmail;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        //public double getInterestRateByInst(string value,int value1)
        //{
        //    try
        //    {
        //        var InterestRate = (from a in uvDb.LoanInterestRates where a.LoanTenure == value && a.Institution_FK == value1 select a).FirstOrDefault();

        //        if (InterestRate == null)
        //        {
        //            return 0;
        //        }
        //        var ALlRecords = uvDb.LoanInterestRates;
        //        return InterestRate.InterestRate;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return 0;
        //    }
        //}


        public double getInterestRateByInstFk(int value, int InstFk)
        {
            try
            {
                var InterestRate = (from a in uvDb.LoanInterestRates where a.LoanTenure == value && a.Institution_FK == InstFk select a).FirstOrDefault();

                if (InterestRate == null)
                {
                    return 0;
                }
                var ALlRecords = uvDb.LoanInterestRates;
                return InterestRate.InterestRate;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public LoanInterestRate GetRate(int tenure, string InstCode)
        {
            try
            {
                var InterestRate = (from a in uvDb.LoanInterestRates
                                    join b in uvDb.Institutions on
                                    a.Institution_FK equals b.ID
                                    where a.LoanTenure == tenure && b.InstitutionCode == InstCode select a).FirstOrDefault();

                if(InterestRate == null)
                {
                    return null;
                }
                return InterestRate;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<LoanInterestRate> GetRateQuartely(int tenure, string InstCode)
        {
            try
            {
                var InterestRate = (from a in uvDb.LoanInterestRates
                                    join b in uvDb.Institutions on
                                    a.Institution_FK equals b.ID
                                    where b.InstitutionCode == InstCode
                                    select a).ToList();

                if (InterestRate == null)
                {
                    return null;
                }
                return InterestRate.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        /*  public LoanApplication GetLoanApplication(string LoanID)
          {
              try
              {
                  var LoanRecord = (from a in uvDb.LoanApplications
                                    join 
                                    where a.LoanRefNumber == LoanID  
                                   select new LoanApplications
                                    {
                                        ID = 
                                        LoanRefNumber  =
                                        Title_FK =
                                        Surname =  
                                        Firstname =
                                        Othernames =
                                        Gender_FK =
                                        MaritalStatus_FK =
                                        MeansOfID_FK =
                                        IdentficationNumber = 
                                        DateOfBirth =
                                        PhoneNumber =
                                        EmailAddress =
                                        ContactAddress =
                                        Landmark =
                                        ClosestBusStop =
                                        StateofResidence_FK =
                                        LGA_FK =
                                        AccomodationType_FK =
                                        Organization =
                                        ApplicantID =
                                        NOK_FullName =
                                        NOK_Relationship =
                                        NOK_PhoneNumber =
                                        NOK_EmailAddress =
                                        NOK_HomeAddress =
                                        LoanAmount =
                                        LoanTenure =
                                        RepaymentMethod_FK =
                                        ExistingLoan =
                                        ExistingLoan_OutstandingAmount =
                                        ExistingLoan_NoOfMonthsLeft =
                                        Bank_FK =
                                        AccountNumber =
                                        AccountName =
                                        BVN =
                                        ValueDate =
                                        ValueTime =
                                        DateCreated =
                                        DateModified =
                                        ApplicationStatus_FK =
                                        LoanComment =
                                        IsVisible =
                                        CreatedBy =
                                        Recommend =
                                        StateofResidence =
                                        }).FirstOrDefault();
                  if (LoanRecord == null)
                  {
                      return null;
                  }
                  return LoanRecord;
              }
              catch (Exception ex)
              {
                  WebLog.Log(ex.Message.ToString());
                  return null;
              }
          }
          */

        public LoanApplication GetLoanApplicationold(string LoanID)
        {
            try
            {
                var LoanRecord = (from a in uvDb.LoanApplications where a.LoanRefNumber == LoanID select a).FirstOrDefault();
                if(LoanRecord == null)
                {
                    return null;




    }
                return LoanRecord;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



       public bool ValidateRec(string sname,string fname,DateTime dDate)
        {
            try
            {
                var LoanRecord = (from a in uvDb.LoanApplications where a.Surname == sname && a.Firstname == fname && a.DateModified == dDate select a).FirstOrDefault();
                if (LoanRecord != null)
                {
                    return true;




                }
                return false;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

         public bool Validate(string value,string value1)
      //  public bool Validate(string value)
        {
            try
            {
                var user = (from a in uvDb.Users
                            where a.EmailAddress == value 
                            //|| a.PhoneNumber == value1
                            select a).FirstOrDefault();
                if (user != null)
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

        public bool ValidatePage(string value)
        {
            try
            {
                var pag = (from a in uvDb.Pages
                           where a.PageName == value
                           select a).FirstOrDefault();
                if (pag != null)
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

        public User getUser(string email)
        {
            try
            {
                var users = (from a in uvDb.Users where a.EmailAddress == email select a).FirstOrDefault();

                if (users == null)
                {
                    return null;
                }

                return users;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }



        public dynamic LoanApRep()
        {
            try
            {
                dynamic MyDynamic = new System.Dynamic.ExpandoObject();
                var AppLoan = (from a in uvDb.LoanApplications
                               
                               where a.AccomodationType_FK == 1
                               select new
                               {

                                   AccountName = a.AccountName,
                                   LoanRefNum = a.LoanRefNumber,

                               }).ToList();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool loggedIn(string username, string password,out int? value)
        {
            try
            {
                var Loggedin = (from a in uvDb.Users
                                where a.EmailAddress == username && a.PaswordVal == password
                                select a).FirstOrDefault();

                if (Loggedin != null )
                {
                    value = Loggedin.ReferralLevel;
                    return true;
                }
                 value = Loggedin.ReferralLevel;
                //value = 0;
                return false;
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
               
            }

        }


        public bool ValidateRole(string value)
        {
            try
            {
                var role = (from a in uvDb.Roles
                            where a.RoleName == value
                            select a).FirstOrDefault();
                if (role != null)
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


        public EmployerLoanDetail GetSalary(int value)
        {
            try
            {
                var salary = (from a in uvDb.EmployerLoanDetails
                            where a.LoanApplication_FK == value
                            select a).FirstOrDefault();
                if (salary == null)
                {
                    return null;
                }
                return salary;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        public List<User> getAllUser()
        {
            try
            {

                var user = (from a in uvDb.Users select a);

                if (user == null)
                {
                    return null;
                }
                return user.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<InstitutionType> getAllInstitutionType()
        {
            try
            {

                var InstType = (from a in uvDb.InstitutionTypes select a);

                if (InstType == null)
                {
                    return null;
                }
                return InstType.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public AppLoan getStudentRecord(int val1,string val2,string val3)
        {
            try
            {
                var Record = (from a in uvDb.StudentRecords
                              join b in uvDb.Institutions on a.Institution_FK equals
                              b.ID
                              where a.MatriculationNumber == val2 && a.Institution_FK == val1 && a.PhoneNumber == val3

                              select new AppLoan
                              {
                               Firstname = a.Firstname,
                               Surname = a.Lastname,
                               Othernames = a.Othernames,
                               PhoneNumber = a.PhoneNumber,
                               EmailAddress = a.EmailAddress,
                               faculty = a.Faculty,
                               Department = a.Deparment,
                               IdentficationNumber = a.MatriculationNumber,
                               Organization = b.Name,
                               DateOfBirth = a.DateOfBirth,
                               Gender = a.Gender == 1 ? "male" : "female",
                               InstitutionAddress = b.InstitutionAddress,
                               LoanAmount = (double)a.CreditLimit,
                               ID = b.ID,
                              }).FirstOrDefault();

                //if(Record.Count > 0)
                //{
                //    return Record.ToList();
                //}
                if (Record == null)
                {
                    return null;
                }

                return Record;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public Institution getInstitutionById(int id)
        {
            try
            {
                var institution = (from a in uvDb.Institutions where a.ID == id select a).FirstOrDefault();
                if(institution == null)
                {
                    return null;
                }
                return institution;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public double GetAmount(string id)
        {
            try
            {
                var ids = Convert.ToInt16(id);
                double Amount = (from a in uvDb.LoanLedgers where a.ID == ids select a.Debit).FirstOrDefault().Value;
                if(Amount == 0)
                {
                    return 0;
                }
                return Amount;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }



      

        public int getInstitutionByLedgerID(string valu)
        {
            try
            {
                int val = Convert.ToInt32(valu);
                var value = (from a in uvDb.LoanLedgers where a.ID == val select a.Institution_FK).FirstOrDefault().Value;

            if(value == 0)
                {
                  return 0;
                }
                return value;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        public List<Institution> getAllInstitution()
        {
            try
            {

                var Institution = (from a in uvDb.Institutions select a);

                if (Institution == null)
                {
                    return null;
                }
                return Institution.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<Institution> getAllSchools()
        {
            try
            {

                var Institution = (from a in uvDb.Institutions where a.InstitutionType_FK == 1 select a);

                if (Institution == null)
                {
                    return null;
                }
                return Institution.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public Institution getInstitutionProfile(int id)
        {
            try
            {

                var users = (from a in uvDb.Institutions where a.ID == id select a).FirstOrDefault();
                if (users == null)
                {
                    return null;
                }
                return users;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public User getUserProfile(int id)
        {
            try
            {

                var users = (from a in uvDb.Users where a.ID == id select a).FirstOrDefault();
                if (users == null)
                {
                    return null;
                }
                return users;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string UpdateProfiles(User users)
        {
            try
            {
                var original = uvDb.Users.Find(users.EmailAddress);

                if (original != null)
                {
                    original.Firstname = users.Firstname;
                    original.Lastname = users.Lastname;
                    //original.EmailAddress = users.EmailAddress;
                    original.PhoneNumber = users.PhoneNumber;


                    uvDb.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        public string UpdateInstitution(Institution inst)
        {
            try
            {
                // var original = uvDb.Institutions.Find(inst.ID);
                var original = (from a in uvDb.Institutions where a.ID == inst.ID select a).FirstOrDefault();
                if (original != null)
                {
                    original.InstitutionAddress = inst.InstitutionAddress;
                    original.HeadOfInstition = inst.HeadOfInstition;
                    original.ContactEmailAddress = inst.ContactEmailAddress;
                    original.ContactPhoneNo = inst.ContactPhoneNo;
                    original.InstitutionAddress = inst.InstitutionAddress;
                    original.InstitutionCode = inst.InstitutionCode;
                    original.InstitutionEmailAddress = inst.InstitutionEmailAddress;
                    original.InstitutionPhoneNo = inst.InstitutionPhoneNo;
                    original.InstitutionType_FK = inst.InstitutionType_FK;
                   

                    uvDb.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<Page> getPag(int id)
        {
            try
            {

                var pag = (from a in uvDb.Pages where a.PageID == id select a).ToList();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public Page getPage(int id)
        {
            try
            {

                var pag = (from a in uvDb.Pages where a.PageID == id select a).FirstOrDefault();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public Page getPageByName(string pageName)
        {
            try
            {

                var pag = (from a in uvDb.Pages where a.PageName == pageName select a).FirstOrDefault();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public User getUserByEmails(string Email)
        {
            try
            {
              var pag = (from a in uvDb.Users where a.EmailAddress == Email select a).FirstOrDefault();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<Page> getAllPage()
        {
            try
            {

                var Pag = (from a in uvDb.Pages select a);

                if (Pag == null)
                {
                    return null;
                }
                return Pag.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        // I Added This Today
        public List<Page> getAllPages()
        {
            try
            {

                var Pag = (from a in uvDb.Pages select a).ToList();

                if (Pag == null)
                {
                    return null;
                }
                return Pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<getPagesx> getAllPag()
        {
            try
            {

                var Pag = (from a in uvDb.Pages 
                            select new getPagesx
                            { PageID = a.PageID, PageName = a.PageName.ToString(), IsVisible = a.IsVisible, ValueDate = a.ValueDate , PageDescription  = a.PageDescription, PageUrl = a.PageUrl}
                          ).ToList();
                
                if (Pag == null)
                {
                    return null;
                }
                return Pag;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string UpdatePages(Page pages)
        {
            try
            {
                var original = uvDb.Pages.Find(pages.PageID);
                //var original = (from a in uvDb.Pages where a.PageID == pages.PageID select a).FirstOrDefault();
                if (original != null)
                {
                    original.PageUrl = pages.PageUrl;
                    original.PageDescription = pages.PageDescription;

                    uvDb.SaveChanges();
                }
                return original.PageID.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<LoanApplication> GetMyLoans(int AppStatFk, int userid)
        {
            try
            {
              // string user = Convert.ToString(userid);
            var Loans = (from a in uvDb.LoanApplications
                         join b in uvDb.LoanApprovals  on a.ID equals b.LoanApplication_FK 
                 where b.ApplicationStatus_FK == AppStatFk && b.CommentBy == userid
                         select a).ToList();

                if(Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<AppLoan> GetApplications()
        {
            try
            {

                var Loans = (from a in uvDb.LoanApplications

                             join b in uvDb.ApplicationStatus on a.ApplicationStatus_FK equals b.ID
                             where a.DateCreated != null && a.DateModified != null
                             select new AppLoan
                             {
                                 Surname = a.Surname,
                                 Firstname = a.Firstname,
                                 Organization = a.Organization,
                                 LoanRefNumber = a.LoanRefNumber,
                                 PhoneNumber = a.PhoneNumber,
                                 LoanAmount = (double)a.LoanAmount,
                                 EmailAddress = a.EmailAddress,
                                 ContactAddress = a.ContactAddress,
                                 ApplicationDate = (DateTime)a.DateCreated,
                                 //ApplicationDate = null ? DateTime.Today : (DateTime)a.DateCreated,
                                 //string.IsNullOrEmpty(a.Signature1) ? "none" : a.Signature1,
                                 LoanTenure = (int)a.LoanTenure,
                                 ApplicationStatus = b.Name,
                             })
                             .ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<IndividualBorrower> GetIndividualBorrower()
        {
            try
            {

                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.EmployerLoanDetails on a.ID equals b.LoanApplication_FK
                             join c in uvDb.NigerianStates on a.StateofResidence_FK equals c.ID
                             join d in uvDb.LGAs on a.LGA_FK equals d.ID
                             join e in uvDb.EmploymentStatus on b.EmploymentStatus_FK  equals e.ID 
                             join f in uvDb.MaritalStatus on a.MaritalStatus_FK equals f.ID
                             join g in uvDb.Institutions on a.Institution_FK equals g.ID
                             join h in uvDb.Titles on a.Title_FK equals h.ID
                             join i in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals i.ID
                            // join j in uvDb.Guarantors on a.ID equals j.LoanApplication_FK
                            // join b in uvDb.ApplicationStatus on a.ApplicationStatus_FK equals b.ID
                            // where a.DateCreated != null && a.DateModified != null
                             select new IndividualBorrower { 
                             CustomerID =  a.LoanRefNumber,
                             BranchCode = g.InstitutionCode,
                             Surname= a.Surname,
                             FirstName = a.Firstname,
                             MiddleName = a.Othernames,
                             DateOfBirth  = a.DateOfBirth,
                             NationalIdentityNumber = i.ID == 3 ? a.IdentficationNumber : "NULL", 
                             DriversLicenseNo = i.ID == 1 ? a.IdentficationNumber : "NULL",
                             BVNNo = a.BVN,
                             PassportNo =  i.ID == 2 ? a.IdentficationNumber : "NULL",
                             Genders = a.Gender_FK == 1 ? "Male" : "Female", 
                             Nationality = "NIGERIA",
                             MaritalStatus  = f.Name,
                             MobileNumber = a.PhoneNumber,
                             PrimaryAddressLine1 = a.ContactAddress,
                             PrimarycityLGA = d.Name,
                             PrimaryState = c.Name,
                             PrimaryCountry = "Nigeria",
                             EmploymentStatus = e.Name,
                             Occupation = b.Occupation,
                             BusinessCategory = "",
                             BusinessSector = "",
                             BorrowerType = "",
                             OtherID = "",
                             TaxID = "",
                             EmailAddress = a.EmailAddress,
                             EmployerName = a.Organization,
                             EmployerAddressLine1 = g.InstitutionAddress,
                             EmployerAddressLine2 = "",
                             EmployerCity = "",
                             EmployerState = "",
                             EmployerCountry = "",
                             Titles = h.Name,
                             PlaceOfBirth = "",
                             WorkPhone = g.ContactPhoneNo,
                             HomePhone = a.PhoneNumber,
                            // SecondaryAddressLine1 = j.ContactAddress ,
                             SecondaryAddressLine1 = "",
                             SecondaryAddressLine2 = "",
                             SecondaryAddressCityLGA = "",
                             SecondaryAddressState = "",
                             SecondaryAddressCountry = "",
                             SpouseSurname = "",
                             SpouseFirstName = "",
                             SpouseMiddleName = "",

                             })
                             .ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<LoanApplication> GetMyApplications(string userid)
        {
            try
            {
                
                var Loans = (from a in uvDb.LoanApplications
                            
                             where  a.CreatedBy == userid 
                             select a).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<AppLoans> GetMyApplicationsWithComment(string userid)
        {
            try
            {
                List<int> Appsta = new List<int>();
                Appsta.Add(8);
                Appsta.Add(9);
                Appsta.Add(10);
                Appsta.Add(11);
                Appsta.Add(5);

                var Loans = (from a in uvDb.LoanApplications
                            // join b in uvDb.LoanApprovals on a.ID equals b.LoanApplication_FK
                            join c in uvDb.ApplicationStatus on a.ApplicationStatus_FK equals c.ID
                             where a.CreatedBy == userid && Appsta.Contains((int)a.ApplicationStatus_FK)
                             select new AppLoans
                             {
                                 Firstname =a.Firstname + a.Surname,
                                 Organization = a.Organization,
                                 PhoneNumber = a.PhoneNumber,
                                 LoanAmount = (double)a.LoanAmount,
                                 EmailAddress = a.EmailAddress,
                                 ContactAddress = a.ContactAddress,
                                 ApplicationDate = (DateTime)a.DateCreated,
                                 DisburseLoanComment = c.Name,
                                 Loantenure = (int)a.ApplicationStatus_FK,
                                 LoanTenure = (int)a.LoanTenure,
                                 LoanRefNumber = a.LoanRefNumber,
                                 LoanComment = a.LoanComment
                                 
                             }).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<AppLoans> GetMyApplicationsStatus(string userid)
        {
            try
            {

                var Loans = (from a in uvDb.LoanApplications
                             join b  in uvDb.ApplicationStatus  on a.ApplicationStatus_FK equals b.ID
                             where a.CreatedBy == userid
                           

                             select new AppLoans
                             {
                                 Firstname = a.Firstname ,
                                 Surname = a.Surname,
                                 Organization = a.Organization,
                                 PhoneNumber = a.PhoneNumber,
                                 LoanAmount = (double)a.LoanAmount,
                                 EmailAddress = a.EmailAddress,
                                 ContactAddress = a.ContactAddress,
                                 ApplicationDate = (DateTime)a.DateCreated,
                                 Loantenure = (int)a.ApplicationStatus_FK,
                                 LoanTenure = (int)a.LoanTenure,
                                 LoanRefNumber = a.LoanRefNumber,
                                 ApplicationStatus = b.Name

                             }).ToList();



                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<LoanApplication> GetDocAppStat(int instFK)
        {
            try
            {

                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID
                           //  where b.InstitutionType_FK == 2 &&
                           where a.ApplicationStatus_FK == instFK
                             select a).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<AppLoan> GetAppStat(int instFK)
        {
            try
            {

                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID
                             join c in uvDb.ApplicationStatus on a.ApplicationStatus_FK equals c.ID
                             where  b.InstitutionType_FK == 2 && a.Institution_FK == instFK
                             select new AppLoan
                             {
                                 Surname = a.Surname,
                                 Firstname = a.Firstname,
                                 Organization = a.Organization,
                                 PhoneNumber = a.PhoneNumber,
                                 LoanAmount = (double)a.LoanAmount,
                                 EmailAddress = a.EmailAddress,
                                 ContactAddress = a.ContactAddress,
                                 ApplicationDate = a.DateCreated.HasValue == true ? (DateTime)a.DateCreated : (DateTime)DateTime.Now ,
                                 LoanTenure = (int)a.LoanTenure ,
                                 ApplicationStatus = c.Name,
                             }
                             
                             
                             ).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


      

        public  List<AppLoan> GetRemitaTransact(string UserFk)
        {
            try
            {
                string User = Convert.ToString(UserFk);
                var RemitTransact = (from a in uvDb.PatnerTransactLogs
                                     join b in uvDb.Users on a.EmailAddress equals b.EmailAddress
                                     join c in uvDb.LoanApplications on a.RefNum equals c.LoanRefNumber
                                     where a.EmailAddress == User

                                     select new AppLoan
                                     {
                                         Surname = c.Surname,
                                         Firstname = c.Firstname,
                                         Organization = c.Organization,
                                         PhoneNumber = c.PhoneNumber,
                                         LoanAmount = (double)c.LoanAmount,
                                         EmailAddress = c.EmailAddress,
                                         ContactAddress = c.ContactAddress,
                                         ApplicationDate = (DateTime)c.DateCreated,
                                         LoanTenure = (int)c.LoanTenure,
                                         LoanRefNumber = c.LoanRefNumber,
                                         PatnerUrl = a.PatnerUrl,
                                         // ApplicationStatus = c.Name,
                                     }).ToList();

                if (RemitTransact == null)
                {
                    return null;
                }

                return RemitTransact;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<AppLoan> GetRemitaTransacts(string UserFk)
        {
            try
            {
                string User = Convert.ToString(UserFk);
                var RemitTransact = (from a in uvDb.PatnerTransactLogs
                                     join b in uvDb.Users on a.EmailAddress equals b.EmailAddress
                                     join c in uvDb.LoanApplications on a.RefNum equals c.LoanRefNumber
                                     //where a.EmailAddress == User

                                     select new AppLoan
                                     {
                                         Surname = c.Surname,
                                         Firstname = c.Firstname,
                                         Organization = c.Organization,
                                         PhoneNumber = c.PhoneNumber,
                                         LoanAmount = (double)c.LoanAmount,
                                         EmailAddress = c.EmailAddress,
                                         ContactAddress = c.ContactAddress,
                                         ApplicationDate = (DateTime)c.DateCreated,
                                         LoanTenure = (int)c.LoanTenure,
                                         LoanRefNumber = c.LoanRefNumber,
                                         PatnerUrl = a.PatnerUrl,
                                         // ApplicationStatus = c.Name,
                                     }).ToList();

                if (RemitTransact == null)
                {
                    return null;
                }

                return RemitTransact;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<LoanApplication> GetLoans(int instFK)
        {
            try
            {
               
                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID  where a.IsVisible == 1 && a.ApplicationStatus_FK == 3 && b.InstitutionType_FK == 2 && a.Institution_FK == instFK   select a ).ToList();

                if(Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<LoanApplication> GetAllLoans(int instFK)
        {
            try
            {

                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID
                             where a.ApplicationStatus_FK == instFK && b.InstitutionType_FK == 2
                             select a).OrderByDescending(c=>c.ID).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        //public List<Classes.LoanApplication> GetAllRemitaTrans(int instFK)
        //{
        //    try
        //    {

        //        var Loans = (from a in uvDb.LoanApplications
        //                     join b in uvDb.Institutions on a.Institution_FK equals b.ID
        //                     join c in uvDb.PatnerTransactLogs on a.LoanRefNumber equals c.RefNum
        //                     where a.ApplicationStatus_FK == instFK && b.InstitutionType_FK == 2 && c.PatnerCode == "Remita"
        //                     select new Classes.LoanApplication
        //                     {
        //                         Surname = a.Surname,
        //                         Firstname = a.Firstname,
        //                         ApplicantID = a.ApplicantID,
        //                         PhoneNumber = a.PhoneNumber,
        //                         EmailAddress = a.EmailAddress,
        //                         ContactAddress = a.ContactAddress,
        //                         LoanAmount = (double)a.LoanAmount,
        //                         ValueDate = a.ValueDate,
        //                         IdentficationNumber = c.mandateID,
        //                         LoanRefNumber = a.LoanRefNumber


        //                     }).ToList();//OrderByDescending(c => c.ID).ToList();

        //        if (Loans == null)
        //        {
        //            return null;
        //        }
        //        return Loans;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        public List<Classes.LoanApplication> GetAllRemitaTrans(int instFK)
        {
            try
            {

                var Loans = (from a in uvDb.PatnerTransactLogs where a.PatnerCode == "Remita"
                             select new Classes.LoanApplication
                             {
                                 Surname = a.ContactPerson,
                                 //BankCode = a.Ba
                                 ApplicantID = a.PatnerReference,
                                 PhoneNumber = a.PhoneNumber,
                                 EmailAddress = a.EmailAddress,
                                 ContactAddress = a.PatnerUrl,
                                 //LoanAmount = (double)a.LoanAmount,
                                 DateCreated = a.DateCreated,
                                 IdentficationNumber = a.mandateID,
                                 LoanRefNumber = a.RefNum


                             }).ToList();//OrderByDescending(c => c.ID).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<LoanApplication> GetChartReports(int InstFK, int Appstat)
        {
            try
            {
                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID
                             where  b.InstitutionType_FK == 2 && a.Institution_FK == InstFK
                             select a).ToList();
               // a.ApplicationStatus_FK == Appstat

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<string> GetChartReportAmount(int InstFK, int Appstat)
        {
            try
            {
                List<string> vals = new List<string>();
                List<Chart> chart = new List<Chart>();
                List<AppLoan> app = new List<AppLoan>();
                var val = (from a in uvDb.LoanApplications
                           join b in uvDb.Institutions on a.Institution_FK equals b.ID
                           join c in uvDb.LoansLedgers on a.LoanRefNumber equals c.RefNumber
                           where b.InstitutionType_FK == 2 && a.Institution_FK == InstFK
                           select new AppLoan
                           {
                               institutionFk = (int)a.Institution_FK,
                               AppStat = (int)a.ApplicationStatus_FK,
                               LoanAmount = (double)c.Debit,
                           }).GroupBy(x => x.AppStat).ToList();
                //group  a by a.ApplicationStatus_FK into g
                //select new
                //{
                //   // (c.RoleId ?? 0),
                //    institutionFk = (int)g.Key,
                //   app = g.Select(x=>x),

                //}).ToList();

                foreach (var b in val)
                {
                    vals.Add(b.Count().ToString());
                    vals.Add(b.Key.ToString());
                    vals.Add(Convert.ToString(b.Sum(x => x.LoanAmount)));
                   
                }
                List<AppLoan> apps = vals.Cast<AppLoan>().ToList();
                if (val == null)
                {
                    return null;
                }
                return vals;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<string> GetChartReportCount(int InstFK, int Appstat)
        {
            try
            {
                List<string> vals = new List<string>();
                List<Chart> chart = new List<Chart>();
                List<AppLoan> app = new List<AppLoan>();
                var val = (from a in uvDb.LoanApplications
                            join b in uvDb.Institutions on a.Institution_FK equals b.ID
                           // join c in uvDb.LoansLedgers on a.LoanRefNumber equals c.RefNumber
                            where b.InstitutionType_FK == 2 && a.Institution_FK == InstFK
                            select new AppLoan
                            {
                             institutionFk = (int)a.Institution_FK,
                             AppStat = (int)a.ApplicationStatus_FK,
                           
                            }).GroupBy(x => x.AppStat).ToList();

               

                foreach (var b in val)
                {
                    vals.Add(b.Count().ToString());
                    vals.Add(b.Key.ToString());
                  
                 
                }
                if (val == null)
                {
                   return null;
                }
                return vals;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

      
        public List<GetLoanOwnedAndCount> GetLoanOwnedAndCount()
        {
            try
            {

                var orders = from a in uvDb.LoanLedgers
                             group a by new
                             {

                                 a.TranxDate.Value.Year,//OfPayment.Value.Year,
                                 a.TranxDate.Value.Month
                             }
                             into g
                             select new GetLoanOwnedAndCount
                             {
                                 Month = g.Select(n => n.TranxDate.Value.Month).FirstOrDefault(),
                                 TotalAmount = g.Sum(n => n.Debit.Value),
                                 CountOrders = g.Count()
                             };

                if(orders == null)
                {
                    return null;
                }

                return orders.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        // working On
        public List<AppLoan> ApproveLoans(int AppstatFk)
        {
            try
            {
                List<int> vals = new List<int>();
                vals = (from a in uvDb.LoanApplications
                                       join b in uvDb.LoanApprovals on a.ID equals b.LoanApplication_FK
                                       where a.ApplicationStatus_FK == AppstatFk
                                       select b.LoanApplication_FK.Value).Distinct().ToList();
                //select new AppLoan
                //{
                //    LID = (int)b.LoanApplication_FK,

                //}).Distinct().ToList();
                //IList<AppLoan> vas = vals;
                //va 

                //var vall = (from v in uvDb.LoanApprovals select v).Distinct().ToList();


                 
                //Commented Out on 10-Nov-2020 
                 var Loans = (from a in uvDb.LoanApplications
                              join b in uvDb.Institutions on a.Institution_FK equals b.ID
                              join c in uvDb.EmployerLoanDetails on a.ID equals c.LoanApplication_FK
                                join d in uvDb.Banks on a.BankCode equals d.FlutterWaveBankCode
                    // 11-05-2020       
                    //  join d in uvDb.Banks on a.BankCode equals d.Code
                               // 10-Nov-2020
                              //  where a.ApplicationStatus_FK == AppstatFk && b.InstitutionType_FK == 2 && vals.Contains(a.ID)
                              where a.ApplicationStatus_FK == AppstatFk  && vals.Contains(a.ID)
                              select new AppLoan
                              {
                                  Surname = a.Surname,
                                  Firstname = a.Firstname,
                                  PhoneNumber = a.PhoneNumber,
                                  ContactAddress = a.ContactAddress,
                                  EmailAddress = a.EmailAddress,
                                  LoanAmount = (double)a.LoanAmount,
                                  ValueDate = a.ValueDate,
                                  Salary = (double)c.NetMonthlyIncome,
                                  LoanTenure = (int)a.LoanTenure,
                                  Organization = a.Organization,
                                  BankCode = d.Name,
                                  LoanRefNumber = a.LoanRefNumber,
                                  ApplicationDate = (DateTime)a.DateCreated,
                                  ExistingLoan = (bool)a.ExistingLoan,
                                  DateRecommended = (DateTime)a.DateCreated,
                              }).ToList();
                              
               /* 10-Nov-2020
               var Loans = (from a in uvDb.LoanApplications
                             join c in uvDb.EmployerLoanDetails on a.ID equals c.LoanApplication_FK
                             join d in uvDb.Banks on a.BankCode equals
                              d.Code where a.ApplicationStatus_FK == AppstatFk
                             select new AppLoan
                             {
                                 Surname = a.Surname,
                                 Firstname = a.Firstname,
                                 PhoneNumber = a.PhoneNumber,
                                 ContactAddress = a.ContactAddress,
                                 EmailAddress = a.EmailAddress,
                                 LoanAmount = (double)a.LoanAmount,
                                 ValueDate = a.ValueDate,
                                 Salary = (double)c.NetMonthlyIncome,
                                 LoanTenure = (int)a.LoanTenure,
                                 Organization = a.Organization,
                                 BankCode = d.Name,
                                 LoanRefNumber = a.LoanRefNumber,
                                 ApplicationDate = (DateTime)a.DateCreated,
                                 ExistingLoan = (bool)a.ExistingLoan,
                                 DateRecommended = (DateTime)a.DateCreated,
                             }).ToList();
                             */
                if (Loans == null)
                {
                    return null;
                }
                
                return Loans.ToList();
               // return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<AppLoan> ChartForApproveLoans(int AppstatFk, int InstFK)
        {
            try
            {
                List<int> vals = new List<int>();
                vals = (from a in uvDb.LoanApplications
                        join b in uvDb.LoanApprovals on a.ID equals b.LoanApplication_FK
                        where a.ApplicationStatus_FK == AppstatFk && a.Institution_FK == InstFK
                        select b.LoanApplication_FK.Value).Distinct().ToList();
              

                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID
                             join c in uvDb.EmployerLoanDetails on a.ID equals c.LoanApplication_FK
                             join d in uvDb.Banks on a.BankCode equals
d.Code
                             where a.ApplicationStatus_FK == AppstatFk && a.Institution_FK == InstFK && b.InstitutionType_FK == 2 && vals.Contains(a.ID)
                             select new AppLoan
                             {
                                 Surname = a.Surname,
                                 Firstname = a.Firstname,
                                 PhoneNumber = a.PhoneNumber,
                                 ContactAddress = a.ContactAddress,
                                 EmailAddress = a.EmailAddress,
                                 LoanAmount = (double)a.LoanAmount,
                                 ValueDate = a.ValueDate,
                                 Salary = (double)c.NetMonthlyIncome,
                                 LoanTenure = (int)a.LoanTenure,
                                 Organization = a.Organization,
                                 BankCode = d.Name,
                                 LoanRefNumber = a.LoanRefNumber,
                                 ApplicationDate = (DateTime)a.DateCreated,
                                 ExistingLoan = (bool)a.ExistingLoan,
                                 DateRecommended = (DateTime)a.DateCreated,
                             }).ToList();
               
                if (Loans == null)
                {
                    return null;
                }

                return Loans.ToList();
              
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        //The Real One
        /*    public List<AppLoan> ApproveLoans(int AppstatFk)
             {
                 try
                 {

                     var Loans = (from a in uvDb.LoanApplications
                                  join b in uvDb.Institutions on a.Institution_FK equals b.ID
                                  join c in uvDb.EmployerLoanDetails on a.ID equals c.LoanApplication_FK
                                  join d in uvDb.Banks on a.BankCode equals
     d.Code
                                  join L in uvDb.LoanApprovals on a.ID equals L.LoanApplication_FK
                                  where a.ApplicationStatus_FK == AppstatFk && b.InstitutionType_FK == 2
                                  select new AppLoan
                                  {
                                      Surname = a.Surname,
                                      Firstname = a.Firstname,
                                      PhoneNumber = a.PhoneNumber,
                                      ContactAddress = a.ContactAddress,
                                      EmailAddress = a.EmailAddress,
                                      LoanAmount = (double)a.LoanAmount,
                                      ValueDate = a.ValueDate,
                                      Salary = (double)c.NetMonthlyIncome,
                                      LoanTenure = (int)a.LoanTenure,
                                      Organization = a.Organization,
                                      BankCode = d.Name,
                                      LoanRefNumber = a.LoanRefNumber,
                                      ApplicationDate = (DateTime)a.DateCreated,
                                      ExistingLoan = (bool)a.ExistingLoan,
                                      DateRecommended = (DateTime)L.DateCreated,
                                  }).ToList();
                    //  var Loans = (from a in uvDb.LoanApplications where a.ApplicationStatus_FK == AppstatFk select a).ToList();

                     if (Loans == null || Loans.Count == 0)
                     {
                         return null;
                     }

                     return Loans.ToList();
                 }
                 catch (Exception ex)
                 {
                     WebLog.Log(ex.Message.ToString());
                     return null;
                 }
             }
             */


      
        public List<AppLoan> LastThirtyDaysLoan(DateTime dt,DateTime dt1, int AppstatFk, int InstFk)
        {
            try
            {
                try
                {
                    List<int> vals = new List<int>();
                    vals = (from a in uvDb.LoanApplications
                            join b in uvDb.LoanApprovals on a.ID equals b.LoanApplication_FK
                            where a.ApplicationStatus_FK == AppstatFk && b.DateCreated >= dt1 && b.DateCreated <= dt && a.Institution_FK == InstFk
                            select b.LoanApplication_FK.Value).Distinct().ToList();
                   
                    var Loans = (from a in uvDb.LoanApplications
                                 join b in uvDb.Institutions on a.Institution_FK equals b.ID
                                 join c in uvDb.EmployerLoanDetails on a.ID equals c.LoanApplication_FK
                                 join d in uvDb.Banks on a.BankCode equals
    d.Code
                                 where a.ApplicationStatus_FK == AppstatFk && a.Institution_FK == InstFk && b.InstitutionType_FK == 2 && vals.Contains(a.ID) 
                                 select new AppLoan
                                 {
                                     Surname = a.Surname,
                                     Firstname = a.Firstname,
                                     PhoneNumber = a.PhoneNumber,
                                     ContactAddress = a.ContactAddress,
                                     EmailAddress = a.EmailAddress,
                                     LoanAmount = (double)a.LoanAmount,
                                     ValueDate = a.ValueDate,
                                     Salary = (double)c.NetMonthlyIncome,
                                     LoanTenure = (int)a.LoanTenure,
                                     Organization = a.Organization,
                                     BankCode = d.Name,
                                     LoanRefNumber = a.LoanRefNumber,
                                     ApplicationDate = (DateTime)a.DateCreated,
                                     ExistingLoan = (bool)a.ExistingLoan,
                                     DateRecommended = (DateTime)a.DateCreated,
                                 }).ToList();
                  
                    if (Loans == null)
                    {
                        return null;
                    }

                    return Loans.ToList();
                   
                }
                catch (Exception ex)
                {
                    WebLog.Log(ex.Message.ToString());
                    return null;
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public double GetLoanAmountowed(int instfk)
        {
            try
            {
                var result = (from a in uvDb.LoanLedgers where a.Institution_FK == instfk where a.PaymentFlag_FK == 0 select a.Debit).Sum();

                if(result == 0)
                {
                    return 0;
                }

                return result.Value;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public PatnerTransactLog getMandate(string Refid)
        {
            try
            {
                var rec = (from a in uvDb.PatnerTransactLogs where a.RefNum == Refid && a.PatnerCode == "Remita" select a).FirstOrDefault();

                if (rec == null)
                {
                    return null;
                }

                return rec;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
       public double GetAmountPaid(int idv)
        {
            try
            {
                var AmountPaid = (from a in uvDb.LedgerTransacts where a.colss == idv  select a.Credit).Sum();

                if (AmountPaid == 0)
                {
                    return 0;
                }

                return AmountPaid.Value;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        public double GetCountOfLoanOwed(int instfk)
        {
            try
            {
                var result = (from a in uvDb.LoanLedgers where a.Institution_FK == instfk && a.PaymentFlag_FK == 0 select a).GroupBy(x=>x.RefNumber).Count();

                if (result == 0)
                {
                    return 0;
                }

                return result;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public List<LoanApplication> ApproveStudentLoans(int AppstatFk)
        {
            try
            {
                var Loans = (from a in uvDb.LoanApplications
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID
                             where a.ApplicationStatus_FK == AppstatFk && a.Institution_FK == 1
                             select a).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public Role getRole(int id)
        {
            try
            {

                var role = (from a in uvDb.Roles where a.RoleId == id select a).FirstOrDefault();
                if (role == null)
                {
                    return null;
                }
                return role;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<int> buildNamesList(User users)
        {
            try
            {

                List<int> roleids = new List<int>();
                roleids = (from a in uvDb.UserRoles where a.User_FK == users.ID select a.Role_FK).ToList();

                if (roleids == null)
                {
                    return null;
                }
                return roleids;


            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        public List<int> buildAllRoleList()
        {
            try
            {

                List<int> roleids = new List<int>();
                roleids = (from a in uvDb.Roles select a.RoleId).ToList();

                if (roleids == null)
                {
                    return null;
                }
                return roleids;


            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        public List<Role> getAllRole()
        {
            try
            {

                var role = (from a in uvDb.Roles select a);

                if (role == null)
                {
                    return null;
                }
                return role.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<GetAssignRoles> getUserRoles(int userid)
        {
            try
            {
                 var result = (from a in uvDb.UserRoles
                 join c in uvDb.Roles on a.Role_FK equals c.RoleId
                 join b in uvDb.Users on a.User_FK equals b.ID
                 where a.User_FK == userid
                 select new GetAssignRoles { userid = b.ID, Roleid = c.RoleId.ToString(), Rolename = c.RoleName, email = b.EmailAddress }).ToList();
                return result;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<GetAssignPages> getPageRoles(int pageid)
        {
            try
            {
                var result = (from a in uvDb.PageAuthentications
                              join c in uvDb.Roles on a.Role_FK equals c.RoleId
                              join b in uvDb.Pages on a.PageName equals b.PageName
                              //  where b.id == pages.id
                              where b.PageID == pageid
                              select new GetAssignPages { pageid = b.PageID, Roleid = c.RoleId.ToString(), Rolename = c.RoleName }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<int> buildPagesList(Page pages)
        {
            try
            {

                List<int> pageids = new List<int>();
                pageids = (from a in uvDb.PageAuthentications where a.PageName == pages.PageName select a.Role_FK.Value).ToList();

                if (pageids == null)
                {
                    return null;
                }
                return pageids;


            }
            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        public List<UnGetAssignRoles> getPageUrl(IEnumerable<int> newList){

            try
            {
                var pageurl = (from p in uvDb.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();
                return pageurl;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            }


        public List<getAllUserAndRoles> getAllUsersAndRoles()
        {
            //---- Create object of our entities class.
            try
            {
                User users = new DataAccessLayerT.DataManager.User();
                LoanViewModel lvm = new LoanViewModel();
                string user = "tolutl@yahoo.com";
                users.EmailAddress = user;
                users.ID = selectUserID(users);

                var results = (from a in uvDb.UserRoles
                               join c in uvDb.Roles on a.Role_FK equals c.RoleId
                               join b in uvDb.Users on a.User_FK equals b.ID
                               //where a.UserId == users.id
                               select new getAllUserAndRoles { userid = b.ID, roleid = c.RoleId, rolename = c.RoleName, email = b.EmailAddress, id = a.ID }).ToList();
                var model = new LoanViewModel
                {
                    getAllUserAndRoless = results.ToList(),

                };
                lvm.getAllUserAndRoless = results;
                return lvm.getAllUserAndRoless.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int selectRolesByName(UserRole userrole)
        {

            try
            {
                var rol = (from a in uvDb.UserRoles where a.Role_FK == userrole.Role_FK && a.User_FK == userrole.User_FK select a).FirstOrDefault();

                if (rol == null)
                {
                    return 0;
                }


                int user = Convert.ToInt32(rol.Role_FK);
                int roleid = user;
                userrole.Role_FK = roleid;
                return user;
            }
            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
                return 0;
            }

        }


       
        public int selectUserID(User users)
        {

            try
            {
                var emails = (from a in uvDb.Users where a.EmailAddress == users.EmailAddress select a).FirstOrDefault();

                if (emails == null)
                {
                    return 0;
                }
                users.ID = Convert.ToInt16(emails.ID);
                return users.ID;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public string getSelectedPage(int value)
        {
            try
            {
                var page = (from a in uvDb.Pages
                            where a.PageID == value
                            select a).FirstOrDefault();

                if (page != null)
                {
                    return page.PageName;
                }
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string getSelectedEmail(int value)
        {
            try
            {
                var page = (from a in uvDb.Users
                            where a.ID == value
                            select a).FirstOrDefault();

                if (page != null)
                {
                    return page.EmailAddress;
                }
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<getAllPagesAndRoles> getAllPagesAndRoles()
        {
            //---- Create object of our entities class.
            try
            {
                LoanViewModel lvm = new LoanViewModel();
                var results = (from a in uvDb.PageAuthentications
                               join c in uvDb.Roles on a.Role_FK equals c.RoleId
                                
                               select new getAllPagesAndRoles

                               {
                                   id = a.ID,
                                   rolename = c.RoleName,
                                   roleid = c.RoleId,
                                   pageName = a.PageName,
                                  


                               }).ToList();


                var model = new LoanViewModel
                {
                    getAllPagesAndRoless = results.ToList(),

                };
                lvm.getAllPagesAndRoless = results;
                return lvm.getAllPagesAndRoless.ToList();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int selectPageRolesByName(PageAuthentication _pa)
        {

            try
            {
                var rol = (from a in uvDb.PageAuthentications where a.Role_FK == _pa.Role_FK && a.PageName == _pa.PageName select a).FirstOrDefault();

                if (rol == null)
                {
                    return 0;
                }


                int user = Convert.ToInt32(rol.Role_FK);
                int roleid = user;
                userrole.Role_FK = roleid;
                return user;
            }
            catch (Exception ex)
            {

                 WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public int getUserID(string email)
        {

            try
            {
         var userid = (from a in uvDb.Users where a.EmailAddress == email select a.ID).FirstOrDefault();

                if (userid == 0)
                {
                    return 0;
                }

                return userid;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public bool ValidateRole(List<Menus> menus ,string value)
        {
            try
            {
                var sirec = menus.Find(x => x.pageurl == value);

                if (sirec != null)
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public User getUserByID(string Id)
        {
            try
            {
                var Ids = Convert.ToInt16(Id);
                var user = (from a in uvDb.Users where a.ID == Ids select a).FirstOrDefault();

                if(user == null)
                {
                    return null;
                }
                return user;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<AccountsModel> getUserByEmail(int id)
        {
            try
         {
                var user = (from a in uvDb.Users
                            where a.ID == id
                            select new AccountsModel
                            {
                                id = a.ID,
                                firstname = a.Firstname,
                                lastname = a.Lastname,
                                Phone = a.PhoneNumber,
                                Email = a.EmailAddress,
                            }).ToList();

                return user.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<int> getUserRols(int id)
        {
            try
            {
           var userroles = (from a in uvDb.UserRoles where a.User_FK == id select a.Role_FK).ToList();
                return userroles.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
           
        }

        public List<Role> getUserRoles(List<int> roleids)
        {
            //  here i am 
            try
            {
                var roles = (from p in uvDb.Roles where roleids.Contains((int)(p.RoleId)) select p).ToList();
                if (roles == null)
                {
                    return null;
                }

                return roles;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }


        public LoanInterestRate GetInterestRate(int tenure)
        {
            try
            {

                var IntRate = (from a in uvDb.LoanInterestRates where 
                               a.LoanTenure == tenure select a).FirstOrDefault();
                if (IntRate == null)
                {

                    return null;
                }

                return IntRate;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return null;
            }
        }

        public List<Menus> getResults(List<string> rol)
        {
            try
            {
                var results = (from p in uvDb.Pages
                               join pa in uvDb.PageAuthentications on p.PageName equals pa.PageName
                               join ph in uvDb.pageHeaders on p.PageHeader  equals ph.id
                               join r in uvDb.Roles on pa.Role_FK equals r.RoleId 

                               where rol.Contains(r.RoleName)
                               select new Menus
                               {
                                   pageName = pa.PageName,
                                   roleid = (int)pa.Role_FK,
                                   pageheader = ph.page_header,
                                   pageurl = p.PageUrl,

                               }).Distinct();

                return results.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool validateInstCode(string code,out int DebitType)
        {
            try
            {
                var valid = (from a in uvDb.Institutions where a.InstitutionCode == code select a).FirstOrDefault();
                if(valid == null)
                {
                    DebitType = 0;
                    return false;
                }
                DebitType = valid.ID;
                return true;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                DebitType = 0;
                return false;
            }
        }



        public bool validateEmail(string code)
        {
            try
            {
                var valid = (from a in uvDb.LoanApplications where a.EmailAddress == code select a).FirstOrDefault();
                if (valid == null)
                {
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


        public string validateEmpName(string code)
        {
            try
            {
                var valid = (from a in uvDb.LoanApplications where a.LoanRefNumber == code select a).FirstOrDefault();
                if (valid == null)
                {
                    return null;
                }
                return valid.Firstname + valid.Surname + '0' +valid.Institution_FK;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public Institution getInstParam(string code)
        {
            try
            {
                var Inst = (from a in uvDb.Institutions where a.InstitutionCode == code select a).FirstOrDefault();
                if(Inst == null)
                {
                    return null;
                }
                return Inst;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<Menus> GetAllMenu()
        {
            try
            {
                var results = (from p in uvDb.Pages
                               select new Menus
                 {
                  pageName = p.PageName,
                  roleid = (int)p.PageID,
                  pageurl = p.PageUrl,
                 }).Distinct();

                return results.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<pageHeader> getAllPageHeader()
        {
            try
            {

                var PagHeader = (from a in uvDb.pageHeaders select a);

                if (PagHeader == null)
                {
                    return null;
                }
                return PagHeader.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic PayrollDisbursedLoans()
        {
            try
            {

                var Rec = uvDb.PayrollDisbursedLoans().ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic PayrollBorroweredLoans()
        {
            try
            {

                var Rec = uvDb.PayrollBorroweredLoans().ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        
        public dynamic PayrollOutstandingLoan(DateTime from)
        {
            try
            {
                var Rec = uvDb.PayrollOutstandingLoan(from).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic PayrollLoanDueForDebit(DateTime from)
         {
            try
            {
                 var Rec = uvDb.PayrollLoanDueForDebit(from).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
         }


        
         public dynamic PayrollRevenueReceived(DateTime from)
           {
            try
            {
                var Rec = uvDb.PayrollRevenueReceived(from).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic PayrollRevenueEarned(DateTime from)
        {
            try
            {
                var Rec = uvDb.PayrollRevenueEarned(from).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public dynamic PayrollregistrationRelated(DateTime from, DateTime to)
        {
            try
            {

                var Rec = uvDb.PayrollregistrationRelated(from, to).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public dynamic PayrollRepayment(DateTime from, DateTime to)
        {
            try
            {

                var Rec = uvDb.PayrollRepayment(from, to).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public dynamic PayrollApplicationRelated(DateTime from, DateTime to)
        {
            try
            {

                var Rec = uvDb.PayrollApplicationRelated(from, to).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public dynamic PayrollUserRelated(DateTime from, DateTime to)
        {
            try
            {

                var Rec = uvDb.PayrollUserRelated(from, to).ToList();

                if (Rec == null)
                {
                    return null;
                }
                return Rec.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Bank> GetBanks()
        {
            try
            {
                var Services = (from a in uvDb.Banks select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }
        public List<LGA> GetLGAsByStateFK(int StateFK)
        {
            try
            {
                var Services = (from a in uvDb.LGAs select a)
                 .Where(x => x.State_FK == StateFK).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<RepaymentMethod> GetRepaymentMethods()
        {
            try
            {
                var Services = (from a in uvDb.RepaymentMethods select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<Title> GetTitles()
        {
            try
            {
                var Services = (from a in uvDb.Titles select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        
       public List<LoanLedger> GetFirstThree()
        {
            try
            {
                var LoanLed = (from a in uvDb.LoanLedgers select a).Distinct().ToList().Take(1);

                if (LoanLed == null)
                {
                    return null;
                }
                return LoanLed.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<LoanLedger> getNewCustomer(string Email)
        {
            try
            {

              
                var InstEmail = new List<LoanLedger>();
                using (var context = new UvlotEntities())
                {
                    InstEmail = uvDb.Database.SqlQuery<LoanLedger>("SELECT * from [LoanLedger] u  WHERE u.RefNumber = '" + Email + "'  ").ToList();
                }
                if (InstEmail == null)
                {
                    return null;
                }
                return InstEmail.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<LoanLedger> GetLoanLedgerr(string RefNumber)
        {
            try
            {
                var LoanLed = (from a in uvDb.LoanLedgers
                                  where a.RefNumber == RefNumber select a

                                 
                                  ).Distinct().ToList();

                if (LoanLed == null || LoanLed.Count == 0)
                {
                    return null;
                }
                return LoanLed.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<LoansLeger> GetLoanLedger(string RefNumber)
        {
            try
            {
                var LoanLedger = (from a in uvDb.LoanLedgers join b in uvDb.LoanApplications on a.RefNumber equals b.LoanRefNumber
              // join c in uvDb.Institutions on b.Institution_FK equals c.ID
                                  where a.RefNumber == RefNumber && b.ApplicationStatus_FK == 6 

                                   select new LoansLeger
                                   {
                                    Credit =(float)a.Credit,
                                    Debit = (float)a.Debit,
                                    TransactionDate = (DateTime)a.TranxDate,
                                    ApplicantID = a.ApplicantID,
                                    LoanAmount = (float)b.LoanAmount,
                                    LoanTenure = (int)b.LoanTenure,
                                    ReferenceNumber = (string)a.RefNumber,
                                    ID = (Int32)a.ID, //(Int32)b.Institution_FK,
                                    orgCode = (int)a.PaymentFlag_FK,
                                   }).Distinct().ToList();
                                  
                if(LoanLedger == null || LoanLedger.Count == 0)
                {
                    return null;
                }
                return LoanLedger.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<MaritalStatu> GetMaritalStatus()
        {
            try
            {
                var Services = (from a in uvDb.MaritalStatus select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<AppLoan> getAllLoanRate()
        {
            try
            {
                var rates = (from a in uvDb.LoanInterestRates
                             join b in uvDb.Institutions on a.Institution_FK equals b.ID
                             select new AppLoan
                             {
                                 interestRate = (float)a.InterestRate,
                                 LoanTenure = (int)a.LoanTenure,
                                 Organization = b.Name,

                             }).ToList();
                if(rates == null)
                {
                    return null;
                }
                return rates.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
             public bool ValidateInstitution(Institution INST)
            {
            try
            {
                var response = (from a in uvDb.Institutions where a.InstitutionCode == INST.InstitutionCode  select a.ID).First();
                if (response == 0)
                {
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



        public Institution ValidatePayrollInstitution(Institution INST)
        {
            try
            {
                var response = (from a in uvDb.Institutions where a.InstitutionCode == INST.InstitutionCode select a).First();
                if (response == null)
                {
                    return null;
                }
                return response;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public bool Validate(LoanInterestRate LIR)
        {
            try
            {
                var response = (from a in uvDb.LoanInterestRates where a.LoanTenure == LIR.LoanTenure && a.Institution_FK == LIR.Institution_FK select a.ID).FirstOrDefault();
                if(response == 0)
                {
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }
        // Work On this today
        public List<LoanInterestRate> GetTenureByInstCode(string InstCode)
        {
            try
            {
                var Services = (from a in uvDb.LoanInterestRates
                                join b in uvDb.Institutions on a.Institution_FK equals b.ID
                                where b.InstitutionCode == InstCode  select a).ToList().Distinct();

                if (Services == null)
                {
                    return null;
                }
                return Services.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<LoanInterestRate> GetAllTenure()
        {
            try
            {
                var Services = (from a in uvDb.LoanInterestRates select a).ToList().Distinct();

                if (Services == null)
                {
                    return null;
                }
                return Services.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<LGA> GetAllLGAs()
        {
            try
            {
                var Services = (from a in uvDb.LGAs select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<AccomodationType> GetAccomodationTypes()
        {
            try
            {
                var Services = (from a in uvDb.AccomodationTypes select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<MeansOfIdentification> GetMeansOfIdentifications()
        {
            try
            {
                var Services = (from a in uvDb.MeansOfIdentifications select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<NigerianState> GetNigerianStates()
        {
            try
            {
                var Services = (from a in uvDb.NigerianStates select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public static string GetBankNameByCode(string bankCode)
        {
            try
            {
                UvlotEntities dbU = new UvlotEntities();
                var bankObj = dbU.Banks.Where(x => x.IsActive == true && x.Code == bankCode).FirstOrDefault();
                return bankObj.Name;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return "";
            }

        }


        public int GetLocalGovt(string LGA)
        {
            try
            {

                var Govt = (from a in uvDb.LGAs
                            where a.Name == LGA
                            select a).FirstOrDefault();
                if (Govt == null)
                {

                    return 0;
                }

                return Govt.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }


        

             public int GetMaritalStatus(string status)
           {
            try
            {

                var idname = (from a in uvDb.MaritalStatus
                              where a.Name == status
                              select a).FirstOrDefault();
                if (idname == null)
                {

                    return 0;
                }

                return idname.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public int getApplicationStatus(string status)
        {
            try
            {
                var AppStat = (from a in uvDb.ApplicationStatus where a.Name == status select a.ID).FirstOrDefault();
                if(AppStat == 0)
                {
                    return 0;
                }
                return AppStat;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        public int GetMeansofIdbyname(string MeansOfID)
        {
          try
            {
           List<MeansOfIdentification> MeansOfId = new List<MeansOfIdentification>();
                MeansOfId = uvDb.MeansOfIdentifications.ToList();
               
                foreach(MeansOfIdentification mn in MeansOfId)
                {
                    if (mn.Name.Contains(MeansOfID))
                    {
                        return mn.ID;
                    }
                }
                return 0;
               
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public string getBankName(string value)
        {
            var Bank = (from a in uvDb.Banks where a.Code == value select a.Name).FirstOrDefault();
            if (Bank == null)
            {
                return null;
            }
            return Bank;
        }

        public string getTitleByID(int value)
        {
            try
            {
                var Title = (from a in uvDb.Titles where a.ID == value select a.Name).First();

                 if(Title == null)
                {
                    return null;
                }
                return Title;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string getStateByID(int value)
        {
            try
            {
                var Title = (from a in uvDb.NigerianStates where a.ID == value select a.Name).First();

                if (Title == null)
                {
                    return null;
                }
                return Title;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string getRepaymentByID(int value)
        {
            try
            {
                var Repay = (from a in uvDb.RepaymentMethods where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string getMeanOfId(int value)
        {
            try
            {
                var Repay = (from a in uvDb.MeansOfIdentifications where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string MaritalStatus(int value)
        {
            try
            {
                var Repay = (from a in uvDb.MaritalStatus where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string Loantenure(int value)
        {
            try
            {
                var Repay = (from a in uvDb.LoanTenures where a.ID == value select a.Name).First();

                if (Repay == null)
                {
                    return null;
                }
                return Repay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public int GetTitleIDByName(string Tittle)
        {
            try
            {
                var TitleName = (from a in uvDb.Titles
                                 where a.Name == Tittle
                                 select a).FirstOrDefault();
                if (TitleName == null)
                {
                    return 0;
                }
                return TitleName.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public int GetEmpoStatus(string Empostatus)
        {
            try
            {
                var Status = (from a in uvDb.EmploymentStatus
                              where a.Name == Empostatus
                              select a).FirstOrDefault();
                if (Status == null)
                {

                    return 0;
                }

                return Status.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public int GetStatus(string MARITALSTATUS)
        {
            try
            {
                var Status = (from a in uvDb.MaritalStatus
                              where a.Name == MARITALSTATUS
                              select a).FirstOrDefault();
                if (Status == null)
                {

                    return 0;
                }

                return Status.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

        public int GetAccomType(string ACCOMMODATIONTYPE)
        {
            try
            {
                var Accomodation = (from a in uvDb.AccomodationTypes
                                    where a.Name == ACCOMMODATIONTYPE
                                    select a).FirstOrDefault();
                if (Accomodation == null)
                {

                    return 0;
                }

                return Accomodation.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetState(string STATEOFRESIDENCE)
        {
            try
            {
                var state = (from a in uvDb.NigerianStates
                             where a.Name == STATEOFRESIDENCE
                             select a).FirstOrDefault();
                if (state == null)
                {

                    return 0;
                }

                return state.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }


        // Amaka Region

        public dynamic ApprovedloanReport(int AppStatusFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join b in uvDb.ApplicationStatus
                               on a.ApplicationStatus_FK equals b.ID
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               join i in uvDb.Banks on a.BankCode equals i.Code

                               where a.ApplicationStatus_FK == AppStatusFk
                               select new AppLoans
                               {
                                   MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   NigerianStates = f.Name,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus = d.Name,
                                   LoanProduct = h.LoanProduct1,
                                   ApplicationStatus = b.Description,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = i.Name,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.Value,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,
                                   Organization = a.Organization,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname
                               }).ToList();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

       public List<DataAccessLayerT.DataManager.AppLoan> getOfferDetails(string Refid)
            {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join t in uvDb.Titles on a.Title_FK equals
                               t.ID
                               join L in uvDb.LoansLedgers on a.LoanRefNumber
                               equals L.RefNumber join g in uvDb.Guarantors 
                               on a.ID equals g.LoanApplication_FK
                             //  join s in uvDb.Signatures on a.LoanRefNumber equals s.LoanRefNum
                               where a.LoanRefNumber == Refid && L.IsVisible == 1

                               select new DataAccessLayerT.DataManager.AppLoan
                               {
                                   Title = t.Name,
                                   LoanRefNumber = a.LoanRefNumber,
                                   ContactAddress = a.ContactAddress,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   ApplicationDate = a.DateCreated.Value,
                                   OutstandingBalance = (float)L.Debit,
                                   DisburseDate = L.trnDate.Value,
                                   GuarSurname = g.Surname,
                                   GuarOthernames = g.Othernames,
                                   GuarEmail = g.EmailAddress,
                                   GuarPhone = g.PhoneNumber,
                                   GuarRelationship = g.RelationShipWithApplicant,
                                 //  signature = string.IsNullOrEmpty(s.Signature1) ? "none" : s.Signature1,
                                   }).ToList();

                if(AppLoan == null || AppLoan.Count == 0)
                {
                    return null;
                }

                return AppLoan.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<DataAccessLayerT.DataManager.AppLoan> getSignature(string Refid)
        {
            try
            {
                var AppLoan = (from a in uvDb.Signatures where a.LoanRefNum == Refid
                               select new DataAccessLayerT.DataManager.AppLoan
                               {
                                   LoanRefNumber = a.LoanRefNum,
                                   signature = string.IsNullOrEmpty(a.Signature1) ? "none" : a.Signature1,
                               }).ToList();

                if (AppLoan == null || AppLoan.Count == 0)
                {
                    return null;
                }

                return AppLoan.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<DataAccessLayerT.DataManager.AppLoan> getSignedOfferDetails(string Refid)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join t in uvDb.Titles on a.Title_FK equals
                               t.ID
                               join L in uvDb.LoansLedgers on a.LoanRefNumber
                               equals L.RefNumber
                               join g in uvDb.Guarantors
            on a.ID equals g.LoanApplication_FK
                               join s in uvDb.Signatures on a.LoanRefNumber equals s.LoanRefNum
                               where a.LoanRefNumber == Refid

                               select new DataAccessLayerT.DataManager.AppLoan
                               {
                                   Title = t.Name,
                                   LoanRefNumber = a.LoanRefNumber,
                                   ContactAddress = a.ContactAddress,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   ApplicationDate = a.DateCreated.Value,
                                   OutstandingBalance = (float)L.Debit,
                                   DisburseDate = L.trnDate.Value,
                                   GuarSurname = g.Surname,
                                   GuarOthernames = g.Othernames,
                                   GuarEmail = g.EmailAddress,
                                   GuarPhone = g.PhoneNumber,
                                   GuarRelationship = g.RelationShipWithApplicant,
                                   signature = string.IsNullOrEmpty(s.Signature1) ? "none" : s.Signature1,
                               }).ToList();

                if (AppLoan == null || AppLoan.Count == 0)
                {
                    return null;
                }

                return AppLoan.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        public List<DataAccessLayerT.DataManager.AppLoan> getOfferDetailsAfterDisbursement(string Refid)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join t in uvDb.Titles on a.Title_FK equals
                               t.ID
                               join L in uvDb.LoansLedgers on a.LoanRefNumber
                               equals L.RefNumber
                               join g in uvDb.Guarantors
                               on a.ID equals g.LoanApplication_FK
                               where a.LoanRefNumber == Refid && L.IsVisible == 1
                               select new DataAccessLayerT.DataManager.AppLoan
                               {
                                   Title = t.Name,
                                   LoanRefNumber = a.LoanRefNumber,
                                   ContactAddress = a.ContactAddress,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   ApplicationDate = a.DateCreated.Value,
                                   OutstandingBalance = (float)L.Debit,
                                   DisburseDate = L.trnDate.Value,
                                   GuarSurname = g.Surname,
                                   GuarOthernames = g.Othernames,
                                   GuarEmail = g.EmailAddress,
                                   GuarPhone = g.PhoneNumber,
                                   GuarRelationship = g.RelationShipWithApplicant,
                               }).ToList();

                if (AppLoan == null || AppLoan.Count == 0)
                {
                    return null;
                }

                return AppLoan.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        public AppLoans CheckAppStatus(string AppStatusFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join b in uvDb.ApplicationStatus
                               on a.ApplicationStatus_FK equals b.ID
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               join i in uvDb.Banks on a.BankCode equals i.Code
                               //join j in uvDb.StudentRecords on a.Institution_FK equals j.Institution_FK
                               //join k in uvDb.Institutions on a.Institution_FK equals k.InstitutionType_FK
                               where a.LoanRefNumber == AppStatusFk
                               select new AppLoans
                               {
                                   //Institutions = k.Name,
                                   //MatriculationNumber = j.MatriculationNumber,
                                   MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   NigerianStates = f.Name,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus = d.Name,
                                   LoanProduct = h.LoanProduct1,
                                   ApplicationStatus = b.Description,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = i.Name,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.Value,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,
                                   Organization = a.Organization,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   
                               }).FirstOrDefault();

                if (AppLoan == null)
                {

                    return null;
                }


                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());

                return null;
            }
        }

        //public AppLoans CheckAppStatus(string AppStatusFk)
        //{
        //    try
        //    {
        //        var AppLoan = (from a in uvDb.LoanApplications
        //                       join b in uvDb.ApplicationStatus
        //                       on a.ApplicationStatus_FK equals b.ID
        //                       join c in uvDb.Titles on a.Title_FK equals c.ID
        //                       join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
        //                       join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
        //                       join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
        //                       join g in uvDb.LGAs on a.LGA_FK equals g.ID
        //                       join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
        //                      join i in uvDb.Banks on a.BankCode equals i.Code
        //                       where a.LoanRefNumber == AppStatusFk
        //                       select new AppLoans
        //                       {
        //                           MeansOfIdentifications = e.Name,
        //                           AccountName = a.AccountName,
        //                           Title = c.Name,
        //                           NigerianStates = f.Name,
        //                           LGAs = g.Name,
        //                           Gender = a.Gender_FK == 1 ? "Male" : "Female",
        //                           LoanRefNumber = a.LoanRefNumber,
        //                           MaritalStatus = d.Name,
        //                           LoanProduct = h.LoanProduct1,
        //                           ApplicationStatus = b.Description,
        //                           AccountNumber = a.AccountNumber,
        //                           ApplicantID = a.ApplicantID,
        //                           BankCode = i.Name,
        //                           BVN = a.BVN,
        //                           ClosestBusStop = a.ClosestBusStop,
        //                           ContactAddress = a.ContactAddress,
        //                           DateOfBirth = a.DateOfBirth,
        //                           EmailAddress = a.EmailAddress,
        //                           ExistingLoan = a.ExistingLoan.Value,
        //                           ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
        //                           ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value,
        //                           Firstname = a.Firstname,
        //                           IdentficationNumber = a.IdentficationNumber,
        //                           LoanAmount = a.LoanAmount.Value,
        //                           LoanTenure = a.LoanTenure.Value,
        //                           NOK_EmailAddress = a.NOK_EmailAddress,
        //                           NOK_FullName = a.NOK_FullName,
        //                           NOK_HomeAddress = a.NOK_HomeAddress,
        //                           NOK_PhoneNumber = a.NOK_PhoneNumber,
        //                           NOK_Relationship = a.NOK_Relationship,
        //                           Organization = a.Organization,
        //                           PhoneNumber = a.PhoneNumber,
        //                           ValueDate = a.ValueDate,
        //                           Landmark = a.Landmark,
        //                           Othernames = a.Othernames,
        //                           Surname = a.Surname
        //                       }).FirstOrDefault();

        //        if (AppLoan == null)
        //        {

        //            return null;
        //        }


        //        return AppLoan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());

        //        return null;
        //    }
        //}



        public dynamic LoanTransactionbyDate(DateTime to, DateTime from)
        {
            try
            {
                var Apploan = uvDb.LoanTransactionbyDate(from, to).ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic LoanAppReport()
        {
            try
            {
                
                var LoanApp = (from a in uvDb.LoanApplications
                               join b in uvDb.ApplicationStatus
                               on a.ApplicationStatus_FK equals b.ID
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               //where a.ApplicationStatus_FK == 1
                               select new AppLoans
                               {
                                   MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   NigerianStates = f.Name,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus = d.Name,
                                   LoanProduct = h.LoanProduct1,
                                   ApplicationStatus = b.Description,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = a.BankCode,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.Value,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                      ExistingLoan_OutstandingAmount =  a.ExistingLoan_OutstandingAmount.Value  ,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,
                                   Organization = a.Organization,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname
                               }).ToList();

                if (LoanApp == null)
                {

                    return null;


                }
                return LoanApp;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic LoanRepayment(DateTime to, DateTime from)
        {
            try
            {
                var Apploan = uvDb.LoanRepayment(from, to).ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int GetInstitution(string Institution)
        {
            try
            {
                var InstitutionName = (from a in uvDb.Institutions
                                       where a.InstitutionCode == Institution
                                       select a).FirstOrDefault();
                if (InstitutionName == null)
                {
                    return 0;
                }
                return InstitutionName.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }

      

        public string GetBankCode(string BANKNAME)
        {
            try
            {
                var Bank = (from a in uvDb.Banks
                            where a.Name == BANKNAME
                            select a.Code).FirstOrDefault();
                if (Bank == null)
                {

                    return null;
                }

                return Bank;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return null;
            }
        }


        public List<LoansLedger> getCommisioRecords(int userfk)
        {
            try
            {
                string ss = "9080";
                var recoreds = (from a in uvDb.LoansLedgers where a.ApplicantID == ss select a).OrderBy(a => a.ID).ToList();

                var newRec = (from a in uvDb.LoansLedgers
                              group a by a.ApplicantID into g
                              where g.Sum(s => s.Credit) - g.Sum(s => s.Debit) > 0
                              from LoansLedger in g
                              select LoansLedger);

                // var Credit = recoreds.Where(q => q.Credit != 0).Select(x => x.Credit).Count();

                // var DebitSum = recoreds.Sum(x => x.Debit);

                if (recoreds == null)
                {
                    return null;
                }

                return recoreds.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<LoansLedger> getCommisioRecordsExact(int userfk,int Num)
        {
            try
            {
                string ss = "9080";
                var recoreds = (from a in uvDb.LoansLedgers where a.ApplicantID == ss && a.Debit == 0 select a).OrderBy(a=>a.ID).ToList().Take(Num);

                /*var newRec = (from a in uvDb.LoansLedgers
                              group a by a.ApplicantID into g
                              where g.Sum(s => s.Credit) - g.Sum(s => s.Debit) > 0
                              from LoansLedger in g select LoansLedger);
                */
               

                if (recoreds == null)
                {
                    return null;
                }

                return recoreds.ToList();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public Bank GetFlutterwaveCode(string BankCode)
        {
            try
            {
                var Bank = (from a in uvDb.Banks
                            where a.FlutterWaveBankCode == BankCode
                            select a).FirstOrDefault();
                if (Bank == null)
                {

                    return null;
                }

                return Bank;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return null;
            }
        }

        

        public AppLoans CheckLoanStatus(string AppStatusFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join b in uvDb.StudentRecords
                               on a.IdentficationNumber equals b.MatriculationNumber
                               join c in uvDb.Institutions on b.Institution_FK equals c.InstitutionType_FK
                               join d in uvDb.ApplicationStatus on a.ApplicationStatus_FK equals d.ID

                               where a.LoanRefNumber == AppStatusFk
                               select new AppLoans
                               {
                                   Institutions = c.Name,
                                   MatriculationNumber = b.MatriculationNumber,
                                   Surname = a.Surname,
                                   Firstname = a.Firstname,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanRefNumber = a.LoanRefNumber,
                                   ApplicationStatus = d.Description,
                                   PhoneNumber = a.PhoneNumber,
                               }).ToList().Take(1).FirstOrDefault();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public AppLoans CheckInstitution(StudentRecord StdRec)
        {
            try
            {
                var ChkInst = (from a in uvDb.StudentRecords
                               join b in uvDb.Institutions on
                               a.Institution_FK equals b.InstitutionType_FK
                               where a.PhoneNumber == StdRec.PhoneNumber &&
                               a.MatriculationNumber == StdRec.MatriculationNumber
                               select new AppLoans
                               {
                                   PhoneNumber = a.PhoneNumber,
                                   Firstname = a.Firstname,
                                   Surname = a.Lastname,
                                   Faculty = a.Faculty,
                                   Department = a.Deparment,
                                   CreditLimit = a.CreditLimit,
                                   MatriculationNumber = a.MatriculationNumber,
                                   Institutions = b.Name,

                               }
                              ).FirstOrDefault();


                if (ChkInst == null)
                {

                    return null;


                }
                return ChkInst;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


       
        // Amaka Region

        /*  public List<pageHeader> getAllPageHeader()
          {
              try
              {

                  var PagHeader = (from a in uvDb.pageHeaders select a);

                  if (PagHeader == null)
                  {
                      return null;
                  }
                  return PagHeader.ToList();
              }
              catch (Exception ex)
              {
                  return null;
              }
          }*/


    }

    public class UnGetAssignRoles
    {

        public int Roleid { get; set; }
        public string Rolename { get; set; }



    }

    public class GetAssignPages
    {
        public int pageid { get; set; }
        public string Roleid { get; set; }
        public string Rolename { get; set; }



    }

    public class getAllUserAndRoles
    {
        public int userid { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string email { get; set; }
        public int id { get; set; }
    }


    public class getAllPagesAndRoles
    {
        // public int pageid { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string pageName { get; set; }
        public int id { get; set; }

        public string pageUrl { get; set; }






    }


    public class GetAssignRoles
    {
        public int userid { get; set; }
        public string Roleid { get; set; }
        public string Rolename { get; set; }
        public string email { get; set; }


    }


    public class LoanViewModel
    {
        //Amaka Region

        public ApplicationStatu ApplicationStatu
        {
            get;
            set;
        }

        public StudentRecord StudentRecord
        {
            get;
            set;
        }

        //Amaka Region 

        public User UserModel
        { get;
          set;
        }
        public Page PageModel
        {
            get;
            set;
        }

        public UserRole UserRole
        {
            get;
            set;

        }

       
        public IEnumerable<GetAssignRoles> GetAssignRoless
        {
            get;
            set;
        }
        public IEnumerable<UnGetAssignRoles> UnGetAssignRoless
        {
            get;
            set;
        }

       
        public IEnumerable<GetAssignPages> GetAssignPagess
        {
            get;
            set;
        }

        public IEnumerable<getAllUserAndRoles> getAllUserAndRoless
        {
            get;
            set;
        }

        public IEnumerable<getAllPagesAndRoles> getAllPagesAndRoless
        {
            get;
            set;
        }


        public IEnumerable<getPagesx> getPagesx
        {
            get;
            set;
        }

    }

    public class AccountsModel
    {

        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string Phone { get; set; }
        public string pasword { get; set; }
        public string confirmPassword { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string ResetPassword { get; set; }
        public DateTime DateTim { get; set; }
        public int isVissibles { get; set; }

        public string Referal { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }

     
    }

    public class Menus
    {
        public string pageName { get; set; }

        public int roleid { get; set; }

        public string pageheader { get; set; }

        public string pageurl { get; set; }


    }

    public class getPagesx
    {
      //  public IEnumerable<getPagesx> comp { get; set; }
        public int PageID { get; set; }
        public string PageName { get; set; }
        public Nullable<int> IsVisible { get; set; }
        public string ValueDate { get; set; }
        public string PageDescription { get; set; }
        public string PageUrl { get; set; }
    }


    public  class LoanApplications
    {
        public int ID { get; set; }
        public string LoanRefNumber { get; set; }
        public int Title_FK { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public int Gender_FK { get; set; }
        public int MaritalStatus_FK { get; set; }
        public int MeansOfID_FK { get; set; }
        public string IdentficationNumber { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ContactAddress { get; set; }
        public string Landmark { get; set; }
        public string ClosestBusStop { get; set; }
        public int StateofResidence_FK { get; set; }
        public int LGA_FK { get; set; }
        public int AccomodationType_FK { get; set; }
        public string Organization { get; set; }
        public string ApplicantID { get; set; }
        public string NOK_FullName { get; set; }
        public string NOK_Relationship { get; set; }
        public string NOK_PhoneNumber { get; set; }
        public string NOK_EmailAddress { get; set; }
        public string NOK_HomeAddress { get; set; }
        public Nullable<double> LoanAmount { get; set; }
        public int LoanTenure { get; set; }
        public int RepaymentMethod_FK { get; set; }
        public bool ExistingLoan { get; set; }
        public Nullable<double> ExistingLoan_OutstandingAmount { get; set; }
        public int ExistingLoan_NoOfMonthsLeft { get; set; }
        public int Bank_FK { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BVN { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public int ApplicationStatus_FK { get; set; }
        public string LoanComment { get; set; }
        public int IsVisible { get; set; }
        public string CreatedBy { get; set; }

        public int Recommend { get; set; }

        public string StateofResidence { get; set; }
    }


    public class IndividualBorrower
    {
        public string CustomerID { get; set; }
        public string BranchCode { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string DateOfBirth { get; set; }
        public string NationalIdentityNumber { get; set; }
        public string DriversLicenseNo { get; set; }
        public string BVNNo { get; set; }
        public string PassportNo { get; set; }
        public string Genders { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string MobileNumber {get; set;}
        public string PrimaryAddressLine1 { get; set; }
        public string PrimaryAddressLine2	{ get; set; }
        public string PrimarycityLGA { get; set; }
        public string PrimaryState { get; set; }
        public string PrimaryCountry { get; set; }
        public string EmploymentStatus { get; set; }
        public string Occupation { get; set; }
        public string BusinessCategory { get; set; }
        public string BusinessSector { get; set; }
        public string BorrowerType { get; set; }
        public string OtherID { get; set; }
        public string TaxID { get; set; }
        public string PictureFilePath { get; set; }
        public string EmailAddress { get; set; }
        public string EmployerName { get; set; }
        public string EmployerAddressLine1 { get; set; }
        public string EmployerAddressLine2 { get; set; }
        public string EmployerCity { get; set; }
        public string EmployerState { get; set; }
        public string EmployerCountry { get; set; }
        public string Titles { get; set; }
        public string PlaceOfBirth { get; set; }
        public string WorkPhone { get; set; }
        public string HomePhone { get; set; }
        public string SecondaryAddressLine1 { get; set; }
        public string SecondaryAddressLine2 { get; set; }
        public string SecondaryAddressCityLGA { get; set; }
        public string SecondaryAddressState { get; set; }
        public string SecondaryAddressCountry { get; set; }
        public string SpouseSurname { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseMiddleName { get; set; }

    }

    public class AppLoan
    {
        public int LID { get; set; }
        public int LengthOfService { get; set; }

        public int LengthOfServiceMonth { get; set; }


        public string LengthOfServices { get; set; }

        public string LengthOfServiceMonths { get; set; }
        public string Designation { get; set; }
        public string AmountInWords { get; set; }
        public DateTime DisburseDate { get; set; }

        public string DisburseDateValue { get; set; }
        public string DisburseLoanComment { get; set; }
        public string ConvertedSalary {get; set;}
        public double Salary { get; set; }
        public string Department { get; set; }

        public string Occupation { get; set; }
        public string AccountName { get; set; }
        public string LoanRefNumber { get; set; }
        public string LoanProduct { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string MeansOfIdentifications { get; set; }
        public string IdentficationNumber { get; set; }

        public string DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }
        public int AppStat { get; set; }
        public string ContactAddress { get; set; }

        public string Landmark { get; set; }

        public string ClosestBusStop { get; set; }

        public string NigerianStates { get; set; }

        public string LGAs { get; set; }

        public string Organization { get; set; }

        public string ApplicantID { get; set; }

        public string NOK_FullName { get; set; }

        public string NOK_Relationship { get; set; }

        public string NOK_PhoneNumber { get; set; }
        public string NOK_EmailAddress { get; set; }
        public string NOK_HomeAddress { get; set; }
        public Double LoanAmount { get; set; }
        public int LoanTenure { get; set; }
        public bool ExistingLoan { get; set; }
        public double ExistingLoan_OutstandingAmount { get; set; }
        public int ExistingLoan_NoOfMonthsLeft { get; set; }

        public string BankCode { get; set; }
        public string AccountNumber { get; set; }

        public string ValueTime { get; set; }
        public string ValueDate { get; set; }
        public string BVN { get; set; }
        public string ApplicationStatus { get; set; }

        public string Repayment { get; set; }

        public int ID { get; set; }
        public string LoanComment { get; set; }

        public string EmployeeStatus { get; set; }

        public float OutstandingBalance { get; set; }

        public string faculty { get; set; }

        public string InstitutionAddress { get; set; }

        public string ConvertedAmount { get; set; }

        public DateTime ApplicationDate { get; set; }

        public DateTime ApplicationApprove { get; set; }
        public string ApplicationDatevalue { get; set; }

        public float interestRate { get; set; }

        public string ConvertedLoanAmt { get; set; }

        public int Title_FK { get; set; }

        public int MaritalStatus_FK { get; set; }

        public int Gender_FK { get; set; }

        public int MeansOfID_FK { get; set; }

        public int StateofResidence_FK { get; set; }

        public int AccomodationType_FK { get; set; }

        public int RepaymentMethod_FK { get; set; }

        public int bank_code { get; set; }

        public string bank_codes { get; set; }

        public string Contract { get; set; }


        public string GuarSurname { get; set; }

        public string GuarOthernames { get; set; }

        public string GuarContact { get; set; }

        public string GuarPhone { get; set; }

        public string GuarEmail { get; set; }

        public string GuarRelationship { get; set; }

        public string IdentficationNumberImage { set; get; }

        public string InstitutionCode { get; set; }

        public int institutionFk { get; set; }

        public DateTime DateRecommended { get; set; }

        public string InstitutionEmailAddress { get; set; }

        public string GuaContract { get; set; }

        public byte[] files { get; set; }

        public string DocumentPath { get; set; }
        
        public string DocumentName { get; set; }

        public string RepaymentAmount { get; set; }

        public string imageName { get; set; }

        public string signature { get; set; }

        public double TotalRepayment { get; set; }

        public string PatnerUrl { get; set; }

        public string flutterwaveBanCode { get; set; }

        public string ConvertedExistingAmount { get; set; }
    }

    public class LoansLeger
    {
        public int ID { get; set; }
        public float Credit { get; set; }

        public float Debit { get; set; }

        public string ReferenceNumber { get; set; }

        public string convertedCredit { get; set; }

        public string convertedDebit { get; set; }

        public DateTime TransactionDate { get; set; }

        public string ApplicantID { get; set; }

        public float LoanAmount { get; set; }

        public int LoanTenure { get; set; }

        public int orgCode { get; set; }
    }



    public class Invoice
    {
        public int LID { get; set; }
        public string Company { set; get; }
        public string Location { set; get; }

        public string ReferenceNum { set; get; }
        public string Fullname { set; get; }

        public DateTime DisburseDate { set; get; }

        public DateTime RepaymentDate {set; get;}
        
        public double Interestrate { set; get; }

        public double Credit { set; get; }

        public double Debit { set; get; }

        public int Tenure { get; set; }
        
        public double InterestPricipalDue { get; set; }                            
                                       
        public string EmployeeID { get; set; }     
        
        public double AmountPaid { get; set; }      
        
        
        public string MandateId { get; set; }     
        
        public string requestId { get; set; }  

        public string BankCode { get; set; }


        public string BankAcct { get; set; }
    }


    public class Invoices
    {
        public string Company { set; get; }
        public string Location { set; get; }

      
        public string Fullname { set; get; }

        public DateTime DisburseDate { set; get; }

        public DateTime RepaymentDate { set; get; }

        
        public int Tenure { get; set; }

        public double InterestPricipalDue { get; set; }

        public string EmployeeID { get; set; }
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
    }

    public class Chart
    {
        public string AppStatName { get; set; }

        public int AppStatCode { get; set; }
    }

    public class GetLoanOwnedAndCount
    {
        public int Month { get; set; }

        public double TotalAmount { get; set; }

        public int CountOrders { get; set; }

        public string Months { get; set; }

        public string TotalAmounts { get; set; }
    }
}
