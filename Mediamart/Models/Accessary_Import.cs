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
    
    public partial class Accessary_Import
    {
        public long Id { get; set; }
        public string CodeImport { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public string Note { get; set; }
        public string Model { get; set; }
    }
}
