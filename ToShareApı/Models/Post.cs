namespace ToShareApı.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Apply>? Applies { get; set; }
    }
}
