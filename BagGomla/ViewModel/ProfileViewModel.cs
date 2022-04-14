using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool IsVerified { get; set; }
        public string Image { get; set; }
        public string ImageExtension { get; set; }
        public string WebsiteUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
    }
    public class UpdateProfileViewModel
    {
        public string FullName { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string WebsiteUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
    }
    public class UpdateProfilePictureViewModel
    {
        public string ProfileImage { get; set; }
        public string ImageExtension { get; set; }
    }
    public class UpdateNationalPictureViewModel
    {
        public string NationalIdImage { get; set; }
        public string ImageExtension { get; set; }
    }
    public class UpdateCommercialCertificateViewModel
    {
        public string CommercialCertificateImage { get; set; }
        public string ImageExtension { get; set; }
    }
}