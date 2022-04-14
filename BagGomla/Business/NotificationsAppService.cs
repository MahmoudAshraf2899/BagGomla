using BagGomla.Enums;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Business
{
    public class NotificationsAppService
    {
        DatabaseContext db = new DatabaseContext();
        public Response GetMyNotifications()
        {
            Response<List<NotificationsViewModel>> result = new Response<List<NotificationsViewModel>>();
            try
            {
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                List<NotificationsViewModel> myNotifications = db.FWYNotification.Where(n => n.IsDeleted == false && n.SendTo == currentUserId)
                    .Select(n=> new NotificationsViewModel 
                    {
                        Details = n.Details,
                        ArDetails = n.ArDetails,
                        ArTitle = n.ArTitle,
                        Title = n.Title,
                        IsRead = n.IsRead,
                        SendFrom = n.SendFrom,
                        Image = n.Image,
                        ImageExtension = n.ImageExtension,
                        DetailsURL = n.DetailsURL,
                        DateTime = n.DateTime
                    })
                    .ToList();
                result.DataResult = myNotifications;
                result.Code = ResponseCode.Success;
            }
            catch(Exception ex)
            {

            }
            return result;
        }
    }
}