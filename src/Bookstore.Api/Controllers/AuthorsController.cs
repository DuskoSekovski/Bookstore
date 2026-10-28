using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Bookstore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireCrudScope")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuthorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _authorService.GetAllAuthorsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AuthorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            return author == null ? NotFound() : Ok(author);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthorResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAuthorRequest request)
        {
            var author = await _authorService.CreateAuthorAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = author.AuthorId }, author);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _authorService.DeleteAuthorAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"Author with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
