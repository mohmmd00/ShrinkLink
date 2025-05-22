namespace ShrinkLink.Auth.Service.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string CreatedAt { get; set; }

        public Guid RoleId { get; set; } //foreign key
        public Role Role { get; set; } //navigation prop

        public User(string username, string email, string passwordHash, string passwordSalt, Guid roleId)
        {
            Id = new Guid();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            RoleId = roleId;
        }
    }
}
