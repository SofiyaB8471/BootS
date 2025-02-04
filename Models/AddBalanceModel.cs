using System.ComponentModel.DataAnnotations;

namespace BootS.Models
{
    public class AddBalanceModel
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        [Required]
        [Range(1, 750000)]
        public decimal Amount { get; set; }
    }
} 