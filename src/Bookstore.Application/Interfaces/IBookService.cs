using Bookstore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.Interfaces
{
    public interface IBookService
    {
        Task<BookResponse?> GetBookByIdAsync(int id);
        Task<IEnumerable<BookResponse>> GetAllBooksAsync();
        Task<BookResponse> CreateBookAsync(CreateBookRequest request);
        Task<bool> UpdateBookAsync(int id, CreateBookRequest request);
        Task<bool> DeleteBookAsync(int id);
        Task<PagedResponse<BookResponse>> SearchBooksAsync(BookSearchRequest request);
    }
}
