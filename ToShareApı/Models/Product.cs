namespace ToShareApı.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public string? Image { get; set; }
    }
}
