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
    
    public partial class A_Revoke_Items
    {
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductModel { get; set; }
        public string ProductName { get; set; }
        public string WarrantiCode { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
        public int Total { get; set; }
        public string Note { get; set; }
        public Nullable<long> RevokeId { get; set; }
        public int Status { get; set; }
    }
}
