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
    
    public partial class B_Process
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CODE { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public Nullable<System.DateTime> Startdate { get; set; }
        public Nullable<System.DateTime> Enddate { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public string Createby { get; set; }
    }
}
