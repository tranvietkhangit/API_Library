using System.ComponentModel.DataAnnotations;
namespace LapTrinhWeb2_API.Models.DTO
{
    public class addBookRequestDTO
    {
        [Required(ErrorMessage = "Title không được để trống")]
        [MinLength(10, ErrorMessage = "Title phải có ít nhất 10 ký tự")]
        [RegularExpression(
            @"^[a-zA-ZÀ-ỹ0-9\s]+$",
            ErrorMessage = "Title không được chứa ký tự đặc biệt"
        )]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        [Range(0, 5, ErrorMessage = "From 0 to 5")]
        public int? Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public int PublisherID { get; set; }
        public List<int> AuthorIds { get; set; }
    }
}
