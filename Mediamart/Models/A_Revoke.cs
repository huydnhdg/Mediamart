//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mediamart.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class A_Revoke
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public string Createby { get; set; }
        public int Status { get; set; }
        public string WarrantiCode { get; set; }
        public Nullable<long> WarrantiId { get; set; }
        public string Hub { get; set; }
        public string StationId { get; set; }
        public string Accessary_Change { get; set; }
        public string Accessary { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> Successdate { get; set; }
        public string Successby { get; set; }
    }
}