using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityLibrary.DataModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using PagedList;
using PagedList.Mvc;

namespace BagGomla.Models
{
    public class ProductModel
    {
        public IDictionary<int, FWYProduct> ProList { get; set; }
        public IDictionary<int, double> Dis { get; set; }
        public List<FWYProduct> ProductList { get; set; }
        public IPagedList<FWYProduct> ProductList1 { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public List<FWYCategory> CategoryList { get; set; }
        public List<AspNetUsers> Suppliers { get; set; }
        public int FilterSortBy { get; set; }
        public int FilterPrice { get; set; }
        public int FilterTag { get; set; }
        public int? TypeId { get; set; }
        public string ProductName { get; set; }
        public int? CategoryID { get; set; }
        public decimal? longitude { get; set; }
        public decimal? latitude { get; set; }
        public string location { get; set; }
    }
    

    public class ProductViewModel
    {
        public FWYProduct Product { get; set; }
        public int? ProductSizeID { get; set; }
        public int? ProductColorID { get; set; }
        public List<FWYSize> ProductSizeList { get; set; }
        public List<FWYColor> ProductColorList { get; set; }
        public List<FWYProduct> RelatedProductList { get; set; }
    }

    public class StoreProduct
    {
        public FWYProduct Product { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        [Required]
        public int CateID { get; set; }
        public SelectList UnitList { get; set; }
        public SelectList BrandList { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList CategoryList2 { get; set; }
        public List<FWYSize> SizeList { get; set; }
        public List<FWYColor> ColorList { get; set; }
        public List<FWYStore> StoreList { get; set; }
        public SelectList CountryList { get; set; }
        public List<SelectListItem> ProductTypeList { get; set; }
    }
}