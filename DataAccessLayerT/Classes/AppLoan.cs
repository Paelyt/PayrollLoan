using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerT.Classes
{

   
    public class AppLoans
        {
            public string LoanComment { get; set; }
            public string AccountName { get; set; }
            public string LoanRefNumber { get; set; }
            public string LoanProduct { get; set; }
            public string Title { get; set; }
            public string Surname { get; set; }
            public string Firstname { get; set; }
            public string Othernames { get; set; }
            public string Gender { get; set; }

        public Nullable<double> CreditLimit { get; set; }
        public string MaritalStatus { get; set; }
            public string MeansOfIdentifications { get; set; }
            public string IdentficationNumber { get; set; }

            public string DateOfBirth { get; set; }

            public string PhoneNumber { get; set; }

            public string EmailAddress { get; set; }

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
            public Boolean ExistingLoan { get; set; }
            public Double ExistingLoan_OutstandingAmount { get; set; }
            public int ExistingLoan_NoOfMonthsLeft { get; set; }

            public string BankCode { get; set; }
            public string AccountNumber { get; set; }
            public string ValueDate { get; set; }
            public string BVN { get; set; }
            public string ApplicationStatus { get; set; }
            public string MatriculationNumber { get; set; }
            public string Institutions { get; set; }
           public string Faculty { get; set; }
           public string Department { get; set; }
        public string Institution { get; set; }

        public DateTime ApplicationDate { get; set; }

        public string DisburseLoanComment { get; set; }
        public string ConvertedSalary { get; set; }
        public double Salary { get; set; }

        public int Loantenure { get; set; }

        public double Interestrate { get; set; }

        public string ConvertedLoanAmt { get; set; }

        public double Repayment { get; set; }
    }

    public class Gender
    {
        public int ID { get; set; }
        public int Name { get; set; }
    }

    public enum Appartment
    {
        Owned, Rented
    }

    public class LoanApplication
    {
        public string instCode { get; set; }
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
        public double LoanAmount { get; set; }
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

        public double SalaryAmount { get; set; }
        public string faculty
        {
            get; set;
        }
        public string Department { get; set; }
        public string Gender { get; set; }

        public string InstitutionAddress { get; set; }

        public string Designation { get; set; }

        public string Occupation { get; set; }

        public int LOS { get; set; }

        public int LOSMnth { get; set; }

        public string OfficialEmail { get; set; }

        public int Contract { get; set; }

        public string BankCode { get; set; }

        public int institutionFk { get; set; }

        public string EmployeeStatus { get; set; }

       public int EmployeeStatusFK { get; set; }

      public string MeanOFIDFIlePath { get; set; }

    }
}

