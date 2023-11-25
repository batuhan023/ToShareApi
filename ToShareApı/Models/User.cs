namespace ToShareApı.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserPhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfilePhoto { get; set; }
        public double? Salary { get; set; }
        public int? FamilySize { get; set; }
        public double? Priority { get; set; }
        public List<Product>? Products { get; set; }
        public List<Apply>? Applies { get; set; }

    }
}
