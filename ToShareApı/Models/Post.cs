namespace ToShareApı.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Count { get; set; }
        public string? Adres { get; set; }

        public string? Image { get; set; }
        public int? ProductId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Apply>? Applies { get; set; }
    }
}
