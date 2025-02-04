using System.ComponentModel.DataAnnotations;

namespace BootS.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public bool InStock { get; set; }
    }
} 