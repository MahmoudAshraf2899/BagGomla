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
    public class ComplaintOrsuggestionPageAPIService
    {
        DatabaseContext db = new DatabaseContext();


        public Response AddComplaintOrsuggestion(AddComplaintOrsuggestionVM model)
        {
            Response response = new Response();
            try
            {
                string userid = HttpContext.Current.User.Identity.GetUserId();
                string senderEmail = db.AspNetUsers.Where(u=>u.Id == userid).Select(u => u.Email).FirstOrDefault();
                string rcieverEmail = db.FWYContect.Select(c => c.Email).FirstOrDefault();
                db.FWYComplaintOrsuggestion.Add(new FWYComplaintOrsuggestion
                {
                   ComplaintOrsuggestion = model.Complaint , 
                   EmailFrom = senderEmail != "" ? senderEmail : "" , 
                   EmailTo = rcieverEmail != "" ? rcieverEmail : "" , 
                   IsRead = false
                });
                db.SaveChanges();
                response.Code = ResponseCode.Success;
            }
            catch (Exception ex)
            {
                return response;
            }

            return response;
        }

        public Response MakeSuggestionRead ( int suggestionId = 0)
        {
            Response response = new Response();

            FWYComplaintOrsuggestion complaintOrsuggestion = db.FWYComplaintOrsuggestion.FirstOrDefault(s => s.ID == suggestionId);
            if(complaintOrsuggestion != null)
            {
                complaintOrsuggestion.IsRead = true;
                db.SaveChanges();
                response.Code = ResponseCode.Success;
            }

            return response;
        }

        public Response<List<ComplaintOrsuggestionVM>> getComplaint()
        {
            Response<List<ComplaintOrsuggestionVM>> responseData = new Response<List<ComplaintOrsuggestionVM>>();
            responseData.DataResult = new List<ComplaintOrsuggestionVM>();

                List<FWYComplaintOrsuggestion> FWYComplaintOrsuggestion = db.FWYComplaintOrsuggestion.ToList();

                if (FWYComplaintOrsuggestion.Count > 0)
                {
                    foreach (var item in FWYComplaintOrsuggestion)
                    {
                        responseData.DataResult.Add(new ComplaintOrsuggestionVM
                        {
                            Complaint = item.ComplaintOrsuggestion ,
                            EmailFrom = item.EmailFrom , 
                            EmailTo = item.EmailTo,
                        }
                        );
                    }

                    if (responseData.DataResult.Count > 0)
                        responseData.Code = ResponseCode.Success;
                }
            
            return responseData;
        }

    }
}