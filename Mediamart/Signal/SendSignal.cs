using Mediamart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mediamart.Signal
{
    public class SendSignal
    {
        MediaMEntities db = new MediaMEntities();
        public void Save(Notification notif)
        {
            NotificationHub objNotifHub = new NotificationHub();
            db.Configuration.ProxyCreationEnabled = false;
            db.Notifications.Add(notif);
            db.SaveChanges();

            objNotifHub.SendNotification(notif.SentTo);
        }
    }
}