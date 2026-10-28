using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/books
        [Authorize(Policy = "RequireCrudScope")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        // GET: api/books/{id}
        [Authorize(Policy = "RequireCrudScope")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if(book == null)
            {
                return NotFound(new { Message = $"Book with ID {id} was not found." });
            }

            return Ok(book);
        }

        // POST: api/books
        [Authorize(Policy = "RequireCrudScope")]
        [HttpPost]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
        {
            var createdBook = await _bookService.CreateBookAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdBook.BookId }, createdBook);
        }

        // PUT: api/books/{id}
        [Authorize(Policy = "RequireCrudScope")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBookRequest request)
        {
            var updated = await _bookService.UpdateBookAsync(id, request);
            if (!updated)
            {
                return NotFound(new { Message = $"Book with ID {id} was not found." });
            }

            return NoContent();
        }

        // DELETE: api/books/{id}
        [Authorize(Policy = "RequireCrudScope")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"Book with ID {id} was not found." });
            }
            return NoContent();
        }

        // GET: api/books/search?searchTerm=Clean&pageNumber=1&pageSize=10
        [Authorize(Policy = "RequireSearchScope")]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResponse<BookResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] BookSearchRequest request)
        {
            var result = await _bookService.SearchBooksAsync(request);
            return Ok(result);
        }
    }
}
