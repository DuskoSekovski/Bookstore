using Bookstore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorResponse>> GetAllAuthorsAsync();
        Task<AuthorResponse?> GetAuthorByIdAsync(int id);
        Task<AuthorResponse> CreateAuthorAsync(CreateAuthorRequest request);
        Task<bool> DeleteAuthorAsync(int id);
    }
}
