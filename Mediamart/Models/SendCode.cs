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
    
    public partial class SendCode
    {
        public long Id { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public Nullable<int> SendStatus { get; set; }
        public bool CheckStatus { get; set; }
    }
}
