using System.ComponentModel.DataAnnotations;

namespace WebAPI6.Models
    {
    public class SignUpModel
        {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConFirmPassword { get; set; } = null!;
        }
    }
