using BagGomla.Utils;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Helper
{
    public class NotificationHelper
    {
        public static void SendProductVisitedNotificationToTheSupplier(string supplierUserId, string visitorUserId, string productName)
        {
            DatabaseContext db = new DatabaseContext();
            List<PushToken> tokens = db.PushTokens.Where(it => it.UserId == supplierUserId).ToList();
            AspNetUsers user = db.AspNetUsers.FirstOrDefault(u => u.Id == visitorUserId && u.IsDeleted == false);
            foreach (var token in tokens)
            {
                string notificationMessage = string.Empty;
                if(user != null)
                {
                    notificationMessage = $"{user.Name ?? user.UserName} Just visited your product : {productName}";
                }
                else
                {
                    notificationMessage = $"Someone Just visited your product : {productName}";
                }
                if (token.OS == OS.Android)
                {
                    PushManager.pushToAndroidDevice(token.Token, "Product Visited", notificationMessage);
                }
                else if(token.OS == OS.Apple)
                {
                    PushManager.pushToIphoneDevice(token.Token, "Product Visited", notificationMessage);
                }
            }
        }
    }
}