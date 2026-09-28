using System.ComponentModel.DataAnnotations;
namespace LapTrinhWeb2_API.Models.DTO
{
    public class AddPublisherRequestDTO
    {
        [Required(ErrorMessage = "Publisher name không được để trống")]
        public string Name { set; get; }
    }
}
