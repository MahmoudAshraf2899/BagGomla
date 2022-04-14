using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Models
{
    public class ConfirmDeleteModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public int? IntID { get; set; }
        public string StringID { get; set; }
    }
}