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
    
    public partial class Rsvp
    {
        public int Id { get; set; }
        public int AttendeeID { get; set; }
        public Nullable<int> Year { get; set; }
        public string Attending { get; set; }
        public string Attended { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
    
        public virtual Attendee Attendee { get; set; }
    }
}
