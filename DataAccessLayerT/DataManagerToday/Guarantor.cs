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
    using System.Collections.Generic;
    
    public partial class Guarantor
    {
        public int ID { get; set; }
        public Nullable<int> LoanApplication_FK { get; set; }
        public string Surname { get; set; }
        public string Othernames { get; set; }
        public string ContactAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string RelationShipWithApplicant { get; set; }
        public string Bvn { get; set; }
        public string ValueDate { get; set; }
        public Nullable<int> IsVisible { get; set; }
    }
}