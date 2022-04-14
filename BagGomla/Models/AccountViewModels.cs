using IdentityLibrary.DataModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BagGomla.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string ID { get; set; }
        [Required]
        public string FullName { get; set; }
        public string ArFullName { get; set; }
        public string Username { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string Phone { get; set; }
        public string Address { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? HowYouKnowUsID { get; set; }
        public int? CompanyTypeID { get; set; }

        public bool IsSupplier { get; set; }
        public FWYSupplierCooperation CompanyModel { get; set; }
        public System.Web.Mvc.SelectList CategoryList { get; set; }
        public List<System.Web.Mvc.SelectListItem> HowYouKnowUsList { get; set; }
        public List<System.Web.Mvc.SelectListItem> CompanyTypeList { get; set; }
    }
    public class APIRegisterViewModel
    {
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string ID { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Username { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string Phone { get; set; }
        public string Address { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? HowYouKnowUsID { get; set; }
        public int? CompanyTypeID { get; set; }

        public bool IsSupplier { get; set; }
        public FWYSupplierCooperation CompanyModel { get; set; }
        public System.Web.Mvc.SelectList CategoryList { get; set; }
        public List<System.Web.Mvc.SelectListItem> HowYouKnowUsList { get; set; }
        public List<System.Web.Mvc.SelectListItem> CompanyTypeList { get; set; }
    }

    public class SupplierRegister
    {
        public RegisterViewModel SupplierModel { get; set; }
        public FWYSupplierCooperation CompanyModel { get; set; }
    }

    public class HttpPostedFileBase
    {
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class ForgotPasswordCodeValidatorViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
