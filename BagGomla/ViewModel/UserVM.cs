using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class UserVM
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsImgProfileUploaded { get; set; } = false;
        public string ImageUrl { get; set; }
        public dynamic Token { get; set; }
        public bool VerifiedPerson { get; set; }
        public string Phone { get; set; }
    }
}