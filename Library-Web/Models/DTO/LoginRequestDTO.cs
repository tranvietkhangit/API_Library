using System.ComponentModel.DataAnnotations;

namespace Library_Web.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Username không được để trống")]
        [EmailAddress(ErrorMessage = "Username phải là email")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password không được để trống")]
        public string Password { get; set; }
    }
}
