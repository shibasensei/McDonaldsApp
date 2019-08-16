namespace Casestudy.Models
{
    public class TrayViewModel
    {
        public int TrayId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string DateCreated { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalAll { get; set; }
        public int QtyS { get;  set; }
        public int QtyO { get;  set; }
        public int QtyB { get;  set; }
        public decimal Price { get;  set; }
        public decimal TotalOne { get;  set; }
    }
}