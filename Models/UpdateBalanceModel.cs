using System.ComponentModel.DataAnnotations;

namespace BootS.Models
{
    public class UpdateBalanceModel
    {
        [Required(ErrorMessage = "Введите номер карты")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Номер карты должен содержать 16 цифр")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Введите срок действия карты")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Неверный формат даты (MM/YY)")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "Введите CVV")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV должен содержать 3 цифры")]
        public string CVV { get; set; }

        [Required(ErrorMessage = "Введите сумму")]
        [Range(1, 100000, ErrorMessage = "Сумма должна быть от 1 до 100000")]
        public decimal Amount { get; set; }
    }
} 