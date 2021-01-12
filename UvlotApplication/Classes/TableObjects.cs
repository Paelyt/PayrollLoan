using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UvlotApplication.Classes
{
    public class TableObjects
    {

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

        public class Remitaresp
        {
            
            public string respCode { get; set; }

            public string respDescription { get; set; }

            public float Amount { get; set; }

            public string requestID { get; set; }

            public string mandateID { get; set; }

            public string transactionRef { get; set; }

            public string RRR { get; set; }

            public string refNumber { get; set; }

            public string fundingBankCode { get; set; }

            public string fundingAccount { get; set; }

            public DateTime TransactDate { get; set; }
            public string RespExtra { get; set; }

            public string RespExtra1 { get; set; }

            public string RespExtra2 { get; set; }

            public string Response { get; set; }

          

        }
        public class RepayLoanRec
        {
            public string respCode { get; set; }
            public string respDescription { get; set; }
            public string institutionCode { get; set; }
            public string fullname { get; set; }
            public string matricNumber { get; set; }
            public string refNumber { get; set; }
            public string outstandingBalance { get; set; }
        }

        public class InstitutionRec
        {
            public string respCode { get; set; }
            public string respDescription { get; set; }           
            public List<TheInstitution> Institutions { get; set; }
        }
        public class TheInstitution
        {
            public string InstitutionName { get; set; }
            public string InstitutionAddress { get; set; }
            public string InstitutionCode { get; set; }
            public string ContactEmail { get; set; }
            public string ContactPhone { get; set; }
        }
        public partial class PartnerModel
        {
            public int ID { get; set; }
            public  int User_FK { get; set; }
            public string PartnerID { get; set; }
            public string PartnerKey { get; set; }
            public Nullable<System.DateTime> DateCreated { get; set; }
            public string ValueDate { get; set; }
            public string ValueTime { get; set; }
            public string EmailAddress { get; set; }
            public string PhoneNumber { get; set; }
            public string ContactPerson { get; set; }
            public string ContactAddress { get; set; }
            public string TokenVal { get; set; }
            public Nullable<double> myTokenTime { get; set; }
        }
   
        public class AccountEnq
        {
            public  string account_number { get; set; }
            public  string bank_code { get; set; }
        }
        public class ValidateResp
        {
            public string respCode { get; set; }
            public string respDescription { get; set; }
            public string sessionid { get; set; }
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
            public string ConvertedSalary { get; set; }
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


        public partial class LoanApplication
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
            public string LoanAmount { get; set; }
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
            // public Nullable<System.DateTime> DateCreated { get; set; }
            // public Nullable<System.DateTime> DateModified { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DateModified { get; set; }
            public int ApplicationStatus_FK { get; set; }
            public string LoanComment { get; set; }
            public int IsVisible { get; set; }
            public string CreatedBy { get; set; }

            public int Recommend { get; set; }

            public string StateofResidence { get; set; }

            public string SalaryAmount { get; set; }
            public string faculty
            {
                get; set;
            }
            public string Department { get; set; }
            public string Gender { get; set; }

            public string InstitutionAddress { get; set; }

            public string Designation { get; set; }

            public string Occupation { get; set; }

            public string LOS { get; set; }

            public string OfficialEmail { get; set; }

            public int Contract { get; set; }

            public string BankCode { get; set; }

            public int institutionFk { get; set; }

            public double SalAmt { get; set; }

            public string GuarSurname { get; set; }

            public string GuarOthernames { get; set; }

            public string GuarContact { get; set; }

            public string GuarPhone { get; set; }

            public string GuarEmail { get; set; }

            public string GuarRelationship { get; set; }

            public string GuarBvn { get; set; }
            public HttpPostedFileBase IdentficationNumberImage { set; get; }

            public string pasword { set; get; }

            public string confirmPassword {set; get;}


        }
    }


}