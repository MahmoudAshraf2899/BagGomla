using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class NotificationsViewModel
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public string ArDetails { get; set; }
        public string Title { get; set; }
        public string ArTitle { get; set; }
        public string DetailsURL { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateTime { get; set; }
        public string SendFrom { get; set; }
        public string Image { get; set; }
        public string ImageExtension { get; set; }
    }
}