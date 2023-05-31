using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.Areas.Admin.Data
{
    public class WarrantiDetail : Warranti
    {
        public string CusName { get;set;}

        //sản phẩm
        public DateTime? Activedate { get; set; }
        public int? Limited { get; set; }
        public DateTime? Enddate { get; set; }

        public List<Warranti_Accessary> Warranti_Accessaries { get; set; }
        public List<Warranti_Image> Image { get; set; }
        public List<Warranti_TypeError> Type_Errors { get; set; }

        //ten day du
        public string Station { get; set; }
        public string Zone_Station { get; set; }
        public string Zone_KTV { get; set; }
        public string KTV { get; set; }

        //chuyen tram
        public string FirstChange { get; set; }
        public string ChangeStation { get; set; }

        public IEnumerable<Warranti_Station> WarrantiStation { get; set; }
    }
}