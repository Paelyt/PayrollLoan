//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccessLayerT.DataManager
{
    using System;
    
    public partial class PayrollLoanPerformance_Result
    {
        public Nullable<int> NoOfPendingApplication { get; set; }
        public Nullable<double> SumOfPendingApplication { get; set; }
        public Nullable<int> NoOfNewDisbursement { get; set; }
        public Nullable<double> SumOfNewDisbursement { get; set; }
        public Nullable<int> NoOfRepaymentToday { get; set; }
        public Nullable<double> AmountOfRepaymentToday { get; set; }
        public Nullable<int> NoOfActiveLoan { get; set; }
        public Nullable<int> NoOfLoansInDefault { get; set; }
        public string PastDuePayment { get; set; }
        public string PastDuePricipal { get; set; }
    }
}
