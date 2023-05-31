using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mediamart.Models;

namespace Mediamart.Utils
{
    public class ConfigControl
    {
        static MediaMEntities db = new MediaMEntities();

        public static bool isBonusActive()
        {
            var config = db.Config_Bonus.SingleOrDefault(a => a.Code == "AGENTBONUS");
            if (config != null && config.Status == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string DOMAIN = System.Configuration.ConfigurationManager.AppSettings["DOMAIN"];
    }
}