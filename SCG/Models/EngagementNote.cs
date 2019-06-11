//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCG.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EngagementNote
    {
        public int Id { get; set; }
        public int EngagementID { get; set; }
        public int BankID { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
    
        public virtual Bank Bank { get; set; }
        public virtual Engagement Engagement { get; set; }
        public virtual User User { get; set; }
    }
}
