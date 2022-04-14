using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Models
{
    public class Enum
    {
        public enum SweetAlertType
        {
            success = 1,
            error = 2
        }

        public enum UserSessionType
        {
            Favorite=1,
            Cart=2
        }

        public enum OrderType
        {
            Pending = 1,
            Accepted = 2,
            Delivered_To_Shipping_Company = 3,
            Rejected = 4,
            Finished = 5,
            Canceled = 6
        }
        public enum AboutType
        {
            OurStory = 1,
            OurMission = 2,
            OurVission = 3,
        }

        public enum UserRole
        {
            Admin = 1,
            Supplier = 2,
            Customer = 3,

        }

        public enum CompanyType
        {
            Factory = 1,
            Imported = 2,
            Wholesaler = 3
        }

        public enum HowYouKnowUs
        {
            Facebook = 1,
            Google = 2,
            Youtube = 3,
            Friend = 4,
            Other = 5
        }
        public enum ProductType
        {
            Local = 1,
            Imported = 2
        }

    }
}