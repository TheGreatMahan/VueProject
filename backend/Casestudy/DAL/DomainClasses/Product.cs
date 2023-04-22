using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class Product
    {
        [Key] public string Id { get; set; }
        
        [ForeignKey("BrandId")] public Brand Brand { get; set; }
        
        [Required] public int BrandId { get; set; }

        [Required] [StringLength(50)] public string ProductName { get; set; }
        
        [Required] [StringLength(20)] public string GraphicName { get; set; }
        
        [Required] [Column(TypeName = "money")] public float CostPrice { get; set; }

        [Required] [Column(TypeName = "money")] public decimal MSRP { get; set; }
        
        [Required] public int QtyOnHand { get; set; }
        
        [Required] public int QtyOnBackOrder{ get; set;}
        
        [StringLength(2000)] public string Description { get; set; }

        [Column(TypeName = "timestamp")] [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)] public byte[] Timer { get; set; }
    }
}
