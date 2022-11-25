using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exam_Prep.Models.ProductsVM
{
    public class customDetails
    {
        public string product_name { get; set; }
        public short model_year { get; set; }
        public decimal list_price { get; set; }
        public string brand_name { get; set; }
        public string category_name { get; set; }
        public List<customStoreQuantity> storeQuantities { get; set; }
    }
}