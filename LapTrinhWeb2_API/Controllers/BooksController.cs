using LapTrinhWeb2_API.CustomActionFilter;
using LapTrinhWeb2_API.Data;
using LapTrinhWeb2_API.Models;
using LapTrinhWeb2_API.Models.Domain;
using LapTrinhWeb2_API.Models.DTO;
using LapTrinhWeb2_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LapTrinhWeb2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;
        public BooksController(AppDbContext dbContext, IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
            _logger = logger;
        }
        [HttpGet("get-all-books")]
        //[Authorize(Roles = "Read")]
        [AllowAnonymous]
        public IActionResult GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
 [FromQuery] string? sortBy, [FromQuery] bool isAscending,
 [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            _logger.LogInformation("GetAll Book Action method was invoked");
            _logger.LogWarning("This is a warning log");
            _logger.LogError("This is an error log");
                       var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery, sortBy,
           isAscending, pageNumber, pageSize);
            _logger.LogInformation($"Finished GetAllBook request with data{JsonSerializer.Serialize(allBooks)}");
            return Ok(allBooks);
        }

        [HttpGet]
        [Route("get-book-by-id/{id}")]
        //[Authorize(Roles = "Read")]
        [AllowAnonymous]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            return Ok(bookWithIdDTO);
        }

        [HttpPost("add-book")]
        //[Authorize(Roles = "Write")]
        [AllowAnonymous]
        public IActionResult AddBook([FromBody] addBookRequestDTO addBookRequestDTO)
        {
            if (!ValidateAddBook(addBookRequestDTO))
            {
                return BadRequest(ModelState);
            }
            var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
            return Ok(bookAdd);
        }

        [HttpPut("update-book-by-id/{id}")]
        //[Authorize(Roles = "Write")]
        [AllowAnonymous]
        public IActionResult UpdateBookById(int id, [FromBody] addBookRequestDTO bookDTO)
        {
            var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
            return Ok(updateBook);
        }
        [HttpDelete("delete-book-by-id/{id}")]
        //[Authorize(Roles = "Write")]
        [AllowAnonymous]
        public IActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            return Ok(deleteBook);
        }
        #region Private methods
        private bool ValidateAddBook(addBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO),
                    "Please add book data"
                );

                return false;
            }
            if (string.IsNullOrWhiteSpace(addBookRequestDTO.Description))
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.Description),
                    "Description cannot be null"
                );
            }
            bool publisherExists = _dbContext.Publishers
                .Any(p => p.Id == addBookRequestDTO.PublisherID);

            if (!publisherExists)
            {         
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.PublisherID),
                    "PublisherID không tồn tại"
                );
            }
            int year = addBookRequestDTO.DateAdded.Year;
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1);
            int publisherBookCount = _dbContext.Books
                .Count(b =>
                    b.PublisherID == addBookRequestDTO.PublisherID &&
                    b.DateAdded >= startDate &&
                    b.DateAdded < endDate
                );
            if (publisherBookCount >= 100)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.PublisherID),
                    $"PublisherID {addBookRequestDTO.PublisherID} đã xuất bản tối đa 100 sách trong năm {year}"
                );
            }
            bool duplicateTitle = _dbContext.Books.Any(b => b.PublisherID == addBookRequestDTO.PublisherID && b.Title == addBookRequestDTO.Title);
            if (duplicateTitle)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.Title),
                    "Book Title đã tồn tại trong Publisher này"
                );
            }
            if (addBookRequestDTO.AuthorIds == null ||
                addBookRequestDTO.AuthorIds.Count == 0)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.AuthorIds),
                    "Book phải có ít nhất 1 Author"
                );
            }
            else
            {
                foreach (var authorId in addBookRequestDTO.AuthorIds)
                {
                    bool authorExists = _dbContext.Authors
                        .Any(a => a.Id == authorId);
                    if (!authorExists)
                    {
                        ModelState.AddModelError(
                            nameof(addBookRequestDTO.AuthorIds),
                            $"AuthorID không tồn tại"
                        );
                    }
                    int bookCount = _dbContext.Books_Authors
                        .Count(ba => ba.AuthorId == authorId);
                    if (bookCount >= 20)
                    {
                        ModelState.AddModelError(
                            nameof(addBookRequestDTO.AuthorIds),
                            $"AuthorID {authorId} đã có tối đa 20 sách"
                        );
                    }
                }
                var duplicateAuthorIds = addBookRequestDTO.AuthorIds
            .GroupBy(id => id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
                foreach (var authorId in duplicateAuthorIds)
                {
                    ModelState.AddModelError(
                        nameof(addBookRequestDTO.AuthorIds),
                        $"AuthorID {authorId} bị gán nhiều lần cho Book"
                    );
                }
            }
            return ModelState.IsValid;
        }
        #endregion
    }
}
