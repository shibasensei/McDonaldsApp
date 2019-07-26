using System.Collections.Generic;
namespace Casestudy.Models
{
    public class ProductViewModel
    {
        public string BrandName { get; set; }
        public int BrandId { get; set; }
        public int Id { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public decimal CostPrice { get; set; }
        public string Description { get; set; }
        public int QtyOnBackOreder { get; set; }
        public int QtyOnHand { get; set; }
        public int Qty { get; set; }
        public decimal MSRP { get; set; }
        public string GraphicName { get; set; }
        public string ProductName { get; set; }
    }
}