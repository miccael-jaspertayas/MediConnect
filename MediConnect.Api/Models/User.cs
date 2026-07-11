using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediConnect.Api.Models
{
    public class User
    {
        [Key]
        public int UserID { get; private set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; private set; } = string.Empty;

        [Required]
        public string PasswordHash { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // One user has one patient profile
        public Patient? Patient { get; private set; }

        private User() { }

        public User(string email, string passwordHash)
        {
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        // Allows AuthService to update the hash for password changes
        public void SetPasswordHash(string newHash) => PasswordHash = newHash;
    }
}
