using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
namespace BagGomla.Enums
{
    public enum ResponseCode
    {
        Success = 0,
        Error = 100,
        AccessDenied = 101,
        UserNotConfirmed = 10,
        UserNotFound = 11,
        UserAlreadyExist = 12,
        InvalidUserOrPassword = 13,
        CannotResetPassword = 14,
        EmailHasSentToUser = 16,
        EmailIsNoSent = 17,
        AccountIsNotActive = 18,
        ForgetPasswordCodeIsInvalid = 103,
        InvalidForgetPasswordCode = 104,
        TryAgain = 105,
        EmailOrPasswordAreInvalid = 106,
        NoSuppliersFound = 107,
        NoCategoryFound = 108,
        PhoneNumberIsAlreadyTaken = 109,
        ImageIsEmpty = 110,
        ImageExtensionIsNotProvided = 111,
        NoSupplierFound = 112,
        CannotCreateProduct = 113,
        ProductAlreadyAdded = 114,
    }
}