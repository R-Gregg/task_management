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
    
    public partial class RssFeed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public string UserID { get; set; }
        public bool Display { get; set; }
        public bool Changeable { get; set; }
    
        public virtual User User { get; set; }
    }
}