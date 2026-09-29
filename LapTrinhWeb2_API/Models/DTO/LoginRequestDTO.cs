using System.ComponentModel.DataAnnotations;

namespace LapTrinhWeb2_API.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
