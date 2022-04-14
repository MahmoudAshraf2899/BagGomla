namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("FWYAbout")]
    public partial class FWYAbout
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        public int? TypeID { get; set; }

        public string Image { get; set; }

        public string ImageExtension { get; set; }

        public string ArTitle { get; set; }

        public string ArDescription { get; set; }
        [AllowHtml]
        public string TermsAnConditionsAr { get; set; }
        [AllowHtml]
        public string TermsAnConditionsEn { get; set; }
    }
}
