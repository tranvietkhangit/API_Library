using System.ComponentModel.DataAnnotations;

namespace LapTrinhWeb2_API.Models.DTO
{
    public class AddAuthorRequestDTO
    {
        [Required(ErrorMessage = "Author name không được để trống")]
        [MinLength(3, ErrorMessage = "Author name phải có ít nhất 3 ký tự")]
        public string FullName { set; get; }
    }
}
