using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }
        [Required]
        public int BrandId { get; set; }

        [StringLength(50)]
        [Required]
        public string ProductName { get; set; } //
        [StringLength(50)]
        [Required]
        public string GraphicName { get; set; } //
        [Column(TypeName = "money")]
        [Required]
        public decimal CostPrice { get; set; } //
        [Column(TypeName = "money")]
        [Required]
        public decimal MSRP { get; set; } //
        [Required]
        public int QtyOnHand { get; set; }
        [Required]
        public int QtyOnBackOreder { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
    }
}
