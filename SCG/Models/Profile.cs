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
    
    public partial class Profile
    {
        public int pro__ID { get; set; }
        public string pro__First_Name { get; set; }
        public string pro__Last_Name { get; set; }
        public string pro__Creds { get; set; }
        public string pro__Title { get; set; }
        public string pro__Phone { get; set; }
        public string pro__Phone_Ext { get; set; }
        public string pro__Email { get; set; }
        public string pro__Shareholder { get; set; }
        public string pro__Areas { get; set; }
        public string pro__Bio { get; set; }
        public string pro__Education { get; set; }
        public string pro__Certifications { get; set; }
        public string pro__Professional_Affiliations { get; set; }
        public string pro__Community_Affiliations { get; set; }
        public string pro__Spouse { get; set; }
        public string pro__Kids { get; set; }
        public string pro__Anniv_Month { get; set; }
        public Nullable<int> pro__Anniv_Year { get; set; }
        public string pro__Photo { get; set; }
        public string pro__Original_Width { get; set; }
        public string pro__Original_Height { get; set; }
        public string pro__File_Resume { get; set; }
        public string pro__File_VCard { get; set; }
        public string pro__LinkedIn { get; set; }
        public string pro__URL { get; set; }
        public string pro__Meta_Title { get; set; }
        public string pro__Meta_Description { get; set; }
        public string pro__Meta_Keywords { get; set; }
        public string pro__User_ID { get; set; }
        public Nullable<int> pro__Status { get; set; }
        public Nullable<System.DateTime> Date_Created { get; set; }
        public string Date_Created_IP { get; set; }
        public Nullable<System.DateTime> Last_Updated { get; set; }
        public string Last_Updated_IP { get; set; }
        public string pro__Personal_Bio { get; set; }
    
        public virtual User User { get; set; }
    }
}
