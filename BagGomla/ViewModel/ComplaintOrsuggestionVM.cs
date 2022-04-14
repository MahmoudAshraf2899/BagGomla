using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class ComplaintOrsuggestionVM
    {
        public string Complaint { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
    }
    public class AddComplaintOrsuggestionVM
    {
        public string Complaint { get; set; }
    }

}