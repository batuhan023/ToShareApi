namespace ToShareApı.Models
{
    public class Apply
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? PostId { get; set; }
        public DateTime? ApplyTime { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAproved { get; set; }
    }
}
