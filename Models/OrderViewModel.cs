namespace BootS.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OrderViewModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Укажите адрес доставки")]
        [MinLength(10, ErrorMessage = "Адрес должен содержать не менее 10 символов")]
        [MaxLength(200, ErrorMessage = "Адрес не может быть длиннее 200 символов")]
        [RegularExpression(@"^[а-яА-Я0-9\s\.,/-]+$", ErrorMessage = "Адрес может содержать только буквы, цифры, пробелы и знаки пунктуации")]
        public string DeliveryAddress { get; set; }
        [Required(ErrorMessage = "Укажите номер телефона")]
        [RegularExpression(@"^\+7\s?\(?[0-9]{3}\)?\s?[0-9]{3}[-\s]?[0-9]{2}[-\s]?[0-9]{2}$", 
            ErrorMessage = "Введите корректный номер телефона в формате +7 (XXX) XXX-XX-XX")]
        public string PhoneNumber { get; set; }
        public string? Comment { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class PlaceOrderModel
    {
        public string DeliveryAddress { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
} 