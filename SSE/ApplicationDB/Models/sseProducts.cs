using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace SSE.ApplicationDB.Models
{
    [Table("sseProducts")]
    [Index(nameof(_name), IsUnique = false)]
    [Index(nameof(price), IsUnique = false)]
    public class sseProducts
    {
        [Key]
        [Editable(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Product Code")]
        public int Id { get; set; }
        [Required]
        [StringLength(127)]
        [Display(Name = "Product Name")]
        public string _name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Product Price")]
        public decimal price { get; set; }
    }

    [Table("sseProducts_v")]
    public class sseProducts_v
    {
        [Required]
        [Display(Name = "Product Code")]
        public int id { get; set; }
        [Required]
        [StringLength(127)]
        [Display(Name = "Product Name")]
        public string name { get; set; }
        [Required]
        [StringLength(127)]
        [Display(Name = "Product Price")]
        public string price { get; set; }
    }
}
