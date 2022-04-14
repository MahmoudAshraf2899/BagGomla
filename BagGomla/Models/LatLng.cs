using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Models
{
    public class LatLng
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public LatLng()
        {
        }

        public LatLng(decimal lat, decimal lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }
    }
}