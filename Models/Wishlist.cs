using System;
using System.ComponentModel.DataAnnotations;

namespace BootS.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public DateTime DateAdded { get; set; }
        
        public Product Product { get; set; }
    }
} 