using BagGomla.Enums;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BagGomla.Business
{
    public class ProfileAppService
    {
        DatabaseContext db = new DatabaseContext();
        public Response GetMyProfileInfo()
        {
            Response<ProfileViewModel> result = new Response<ProfileViewModel>();
            ProfileViewModel profile = new ProfileViewModel();
            try
            {
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                AspNetUsers user = db.AspNetUsers.Include(u=>u.FWYSupplierCooperation).FirstOrDefault(p => p.Id == currentUserId && !p.IsDeleted);
                profile = new ProfileViewModel
                {
                    Email = user.Email,
                    Address = user.FWYSupplierCooperation != null ? user.FWYSupplierCooperation.FirstOrDefault().Address : "",
                    FacebookUrl = user.FWYSupplierCooperation != null ? user.FWYSupplierCooperation.FirstOrDefault().Facebook : "",
                    InstagramUrl = user.FWYSupplierCooperation != null ? user.FWYSupplierCooperation.FirstOrDefault().Instagram : "",
                    WebsiteUrl = user.FWYSupplierCooperation != null ? user.FWYSupplierCooperation.FirstOrDefault().WebsiteUrl : "",
                    FullName = user.Name,
                    Image = user.Image,
                    ImageExtension = user.ImageExtension,
                    IsVerified = user.IsSupplier,
                    PhoneNumber = user.PhoneNumber,
                    StoreName = user.FWYSupplierCooperation != null ? user.FWYSupplierCooperation.First().Name : "",
                };
                result.DataResult = profile;
                result.Code = ResponseCode.Success;
            }
            catch(Exception ex)
            {

            }
            return result;
        }

        public Response UpdateMyProfileInfo(UpdateProfileViewModel model)
        {
            Response<bool> result = new Response<bool>();
            try
            {
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                if (db.AspNetUsers.Any(u => u.IsDeleted == false && u.PhoneNumber == model.PhoneNumber && u.Id != currentUserId))
                {
                    result.Code = ResponseCode.PhoneNumberIsAlreadyTaken;
                    result.Message = "Phone Number Is Already Taken";
                    return result;
                }
                AspNetUsers user = db.AspNetUsers.Include(u => u.FWYSupplierCooperation).FirstOrDefault(p => p.Id == currentUserId && !p.IsDeleted);
                IQueryable<FWYSupplierCooperation> supplierStore = db.FWYSupplierCooperation.Where(s => s.SupplierID == currentUserId && s.IsDeleted == false);
                user.Name = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                foreach (var item in supplierStore)
                {
                    item.Name = model.StoreName;
                    item.ArName = model.StoreName;
                    item.WebsiteUrl = model.WebsiteUrl;
                    item.Facebook = model.FacebookUrl;
                    item.Instagram = model.InstagramUrl;
                }
                if (db.SaveChanges() > 0)
                {
                    result.DataResult = true;
                    result.Code = ResponseCode.Success;
                }
                else
                {
                    result.DataResult = false;
                    result.Code = ResponseCode.Error;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public Response UpdateMyNationalId(UpdateNationalPictureViewModel model)
        {
            Response<bool> result = new Response<bool>();
            try
            {
                Response valid = ValidateRequest(model);
                if (!valid.IsSccuessCode)
                {
                    return valid;
                }
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                FWYSupplierCooperation supplier = db.FWYSupplierCooperation.FirstOrDefault(p => p.IsDeleted == false && p.AspNetUsers.Id == currentUserId);
                supplier.NationalCopyID = model.NationalIdImage;
                supplier.NationalCopyIDExtension = model.ImageExtension;
                if (db.SaveChanges() > 0)
                {
                    result.DataResult = true;
                    result.Code = ResponseCode.Success;
                }
                else
                {
                    result.DataResult = false;
                    result.Code = ResponseCode.Error;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        

        public Response UpdateMyCommercialCertificate(UpdateCommercialCertificateViewModel model)
        {
            Response<bool> result = new Response<bool>();
            try
            {
                Response valid = ValidateRequest(model);
                if (!valid.IsSccuessCode)
                {
                    return valid;
                }
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                FWYSupplierCooperation supplier = db.FWYSupplierCooperation.FirstOrDefault(p => p.IsDeleted == false && p.AspNetUsers.Id == currentUserId);
                supplier.CommericialRegister = model.CommercialCertificateImage;
                supplier.CommericialRegisterExtension = model.ImageExtension;
                if (db.SaveChanges() > 0)
                {
                    result.DataResult = true;
                    result.Code = ResponseCode.Success;
                }
                else
                {
                    result.DataResult = false;
                    result.Code = ResponseCode.Error;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public Response UpdateMyProfilePicture(UpdateProfilePictureViewModel model)
        {
            Response<bool> result = new Response<bool>();
            try
            {
                Response valid = ValidateRequest(model);
                if (!valid.IsSccuessCode)
                {
                    return valid;
                }
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();
                AspNetUsers user = db.AspNetUsers.FirstOrDefault(p => p.Id == currentUserId && !p.IsDeleted);
                user.Image= model.ProfileImage;
                user.ImageExtension= model.ImageExtension;
                if (db.SaveChanges() > 0)
                {
                    result.DataResult = true;
                    result.Code = ResponseCode.Success;
                }
                else
                {
                    result.DataResult = false;
                    result.Code = ResponseCode.Error;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        private Response ValidateRequest(UpdateProfilePictureViewModel model)
        {
            Response result = new Response();
            try
            {
                if (model.ProfileImage == null)
                {
                    result.Code = ResponseCode.ImageIsEmpty;
                    result.Message = "Profile Is Not Provided";
                    return result;
                }
                if (model.ImageExtension == null)
                {
                    result.Code = ResponseCode.ImageExtensionIsNotProvided;
                    result.Message = "ImageExtension Is Not Provided";
                    return result;
                }
                result.Code = ResponseCode.Success;
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        private Response ValidateRequest(UpdateCommercialCertificateViewModel model)
        {
            Response result = new Response();
            try
            {
                if (model.CommercialCertificateImage == null)
                {
                    result.Code = ResponseCode.ImageIsEmpty;
                    result.Message = "CommercialCertificate Is Not Provided";
                    return result;
                }
                if (model.ImageExtension == null)
                {
                    result.Code = ResponseCode.ImageExtensionIsNotProvided;
                    result.Message = "ImageExtension Is Not Provided";
                    return result;
                }
                result.Code = ResponseCode.Success;
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        private Response ValidateRequest(UpdateNationalPictureViewModel model)
        {
            Response result = new Response();
            try
            {
                if (model.NationalIdImage == null)
                {
                    result.Code = ResponseCode.ImageIsEmpty;
                    result.Message = "NationalId Image Is Empty";
                    return result;
                }
                if (model.ImageExtension == null)
                {
                    result.Code = ResponseCode.ImageExtensionIsNotProvided;
                    result.Message = "ImageExtension Is Not Provided";
                    return result;
                }
                result.Code = ResponseCode.Success;
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}