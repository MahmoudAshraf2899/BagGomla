using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Business
{
    public class FacadeImageAppService
    {
        DatabaseContext db = new DatabaseContext();

        public Response<List<FacadeImageVM>> GetAll()
        {
            Response<List<FacadeImageVM>> result = new Response<List<FacadeImageVM>>();
            result.DataResult = new List<FacadeImageVM>();

            result.DataResult = db.FWYAds
                .Where(a => a.IsDeleted != true)
                .Select(a=> new FacadeImageVM { 
                   Id = a.Id , 
                   Image = a.Image , 
                   ImageExtension = a.DetailsURL
                 }).ToList();

            return result;

        }

    }
}