using LapTrinhWeb2_API.Data;
using LapTrinhWeb2_API.Models.Domain;
using LapTrinhWeb2_API.Models.DTO;
using LapTrinhWeb2_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinhWeb2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorRepository;
        public AuthorsController(AppDbContext dbContext, IAuthorRepository
       authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }
        [HttpGet("get-all-author")]
        //[Authorize(Roles = "Read")]
        [AllowAnonymous]
        public IActionResult GetAllAuthor()
        {
            var allAuthors = _authorRepository.GellAllAuthors();
            return Ok(allAuthors);
        }
        [HttpGet("get-author-by-id/{id}")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAuthorById(int id)
        {
            var authorWithId = _authorRepository.GetAuthorById(id);
            return Ok(authorWithId);
        }
        [HttpPost("add-author")]
        [Authorize(Roles = "Write")]
        public IActionResult AddAuthors(
    [FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authorAdd = _authorRepository.AddAuthor(addAuthorRequestDTO);
            return Ok(authorAdd);
        }
        [HttpPut("update-author-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult UpdateBookById(int id, [FromBody] AuthorNoIdDTO authorDTO)
        {
            var authorUpdate = _authorRepository.UpdateAuthorById(id, authorDTO);
            return Ok(authorUpdate);
        }
        [HttpDelete("delete-author-by-id/{id}")]
        [Authorize(Roles = "Write")]
        public IActionResult DeleteAuthorById(int id)
        {
            var author = _dbContext.Authors
                .FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                return NotFound(new
                {
                    message = "Author không tồn tại"
                });
            }
            bool hasBooks = _dbContext.Books_Authors
                .Any(ba => ba.AuthorId == id);

            if (hasBooks)
            {
                ModelState.AddModelError(
                    "AuthorId",
                    "Không thể xóa Author vì Author đang được gắn với Book"
                );
                return Conflict(ModelState);
            }
            var authorDelete = _authorRepository.DeleteAuthorById(id);
            return Ok(authorDelete);
        }
    }
}
