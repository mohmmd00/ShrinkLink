namespace ShrinkLink.Auth.Service.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public Guid RoleId { get; set; } //foreign key
        public Role Role { get; set; } //navigation prop

    }
}
