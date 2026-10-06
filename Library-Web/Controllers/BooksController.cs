using Microsoft.AspNetCore.Mvc;
using Library_Web.Models.DTO;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text;
using System.Net.Mime;
using System.Net.Http.Headers;


namespace Library_Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true)
        {
            try
            {
                var loginCheck = CheckLogin();

                if (loginCheck != null)
                {
                    return loginCheck;
                }

                var client = CreateAuthenticatedClient();

                var url =
                    $"https://localhost:7045/api/Books/get-all-books" +
                    $"?filterOn={filterOn}" +
                    $"&filterQuery={filterQuery}" +
                    $"&sortBy={sortBy}" +
                    $"&isAscending={isAscending}";

                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var books = await response.Content
                    .ReadFromJsonAsync<List<BookDTO>>();

                return View(books);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
        [HttpGet]
        public async Task<IActionResult> addBook()
        {
            var loginCheck = CheckLogin();

            if (loginCheck != null)
            {
                return loginCheck;
            }

            if (!IsWriteUser())
            {
                return Forbid();
            }

            var client = CreateAuthenticatedClient();

            List<authorDTO> responseAu = new List<authorDTO>();

            var httpResponseAu = await client.GetAsync(
                "https://localhost:7045/api/Authors/get-all-author");

            httpResponseAu.EnsureSuccessStatusCode();

            responseAu.AddRange(
                await httpResponseAu.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());

            ViewBag.listAuthor = responseAu;

            List<publisherDTO> responsePu = new List<publisherDTO>();

            var httpResponsePu = await client.GetAsync(
                "https://localhost:7045/api/Publishers/get-all-publisher");

            httpResponsePu.EnsureSuccessStatusCode();

            responsePu.AddRange(
                await httpResponsePu.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());

            ViewBag.listPublisher = responsePu;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> addBook(addBookDTO addBookDTO)
        {

            try
            {
                var loginCheck = CheckLogin();

                if (loginCheck != null)
                {
                    return loginCheck;
                }
                if (!IsWriteUser())
                {
                    return Forbid();
                }

                var client = CreateAuthenticatedClient();

                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(
                        "https://localhost:7045/api/Books/add-book"),

                    Content = new StringContent(
                        JsonSerializer.Serialize(addBookDTO),
                        Encoding.UTF8,
                        MediaTypeNames.Application.Json)
                };

                var httpResponseMess =
                    await client.SendAsync(httpRequestMess);

                httpResponseMess.EnsureSuccessStatusCode();

                var response =
                    await httpResponseMess.Content
                        .ReadFromJsonAsync<addBookDTO>();

                if (response != null)
                {
                    return RedirectToAction("Index", "Books");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }
        public async Task<IActionResult> listBook(int id)
        {
            var loginCheck = CheckLogin();

            if (loginCheck != null)
            {
                return loginCheck;
            }

            BookDTO response = new BookDTO();

            try
            {
                var client = CreateAuthenticatedClient();

                var httpResponseMess =
                    await client.GetAsync(
                        "https://localhost:7045/api/Books/get-book-by-id/" + id);

                httpResponseMess.EnsureSuccessStatusCode();

                response = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> editBook(int id)
        {
            var loginCheck = CheckLogin();

            if (loginCheck != null)
            {
                return loginCheck;
            }

            if (!IsWriteUser())
            {
                return Forbid();
            }
            BookDTO responseBook = new BookDTO();
            var client = CreateAuthenticatedClient();
            var httpResponseMess = await client.GetAsync("https://localhost:7045/api/Books/get-book-by-id/" + id);
            httpResponseMess.EnsureSuccessStatusCode();
            responseBook = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>();
            ViewBag.Book = responseBook;
            List<authorDTO> responseAu = new List<authorDTO>();
            var httpResponseAu = await client.GetAsync("https://localhost:7045/api/Authors/get-all-author");
            httpResponseAu.EnsureSuccessStatusCode();
            responseAu.AddRange(await httpResponseAu.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());
            ViewBag.listAuthor = responseAu;
            List<publisherDTO> responsePu = new List<publisherDTO>();
            var httpResponsePu = await client.GetAsync("https://localhost:7045/api/Publishers/get-all-publisher");
            httpResponsePu.EnsureSuccessStatusCode();
            responsePu.AddRange(await httpResponsePu.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());
            ViewBag.listPublisher = responsePu;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> editBook([FromRoute] int id, editBookDTO bookDTO)
        {
            try
            {
                var loginCheck = CheckLogin();

                if (loginCheck != null)
                {
                    return loginCheck;
                }

                if (!IsWriteUser())
                {
                    return Forbid();
                }
                var client = CreateAuthenticatedClient();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri("https://localhost:7045/api/Books/update-book-by-id/" + id),
                    Content = new StringContent(JsonSerializer.Serialize(bookDTO), Encoding.UTF8, MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();
                var response = await httpResponseMess.Content.ReadFromJsonAsync<addBookDTO>();
                if (response != null)
                {
                    return RedirectToAction("Index", "Books");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> delBook([FromRoute] int id)
        {
            try
            {
                var loginCheck = CheckLogin();

                if (loginCheck != null)
                {
                    return loginCheck;
                }

                if (!IsWriteUser())
                {
                    return Forbid();
                }
                var client = CreateAuthenticatedClient();
                var httpResponseMess = await client.DeleteAsync("https://localhost:7045/api/Books/delete-book-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("Index");
        }
        private IActionResult? CheckLogin()
        {
            var token = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }

            return null;
        }

        private HttpClient CreateAuthenticatedClient()
        {
            var client = httpClientFactory.CreateClient();

            var token = HttpContext.Session.GetString("JWT");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
        private bool IsWriteUser()
        {
            return HttpContext.Session.GetString("Role") == "Write";
        }
        
    }
}
