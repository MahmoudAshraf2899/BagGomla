using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Business
{
    public class ContactPageAPIService
    {
        DatabaseContext db = new DatabaseContext();
        public Response<ContactVM> getContactInfo()
        {
            Response<ContactVM> responseData = new Response<ContactVM>();
            responseData.DataResult = new ContactVM();

            responseData.DataResult = db.FWYContect.Select(c=> new ContactVM
            {
                Address = c.Address , 
                Email = c.Email , 
                FacebookUrl = c.Facebook , 
                InstgramUrl = c.Instagram , 
                LinkedinUrl = c.Linkedin , 
                Phone = c.Phone
            }).FirstOrDefault();

            if(responseData.DataResult != null)
            {
                responseData.Code = Enums.ResponseCode.Success;
            }
            return responseData;
        }

    }
}