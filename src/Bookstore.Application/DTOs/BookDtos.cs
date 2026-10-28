using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.DTOs
{
    public record CreateBookRequest(string Title, int AuthorId);

    public record BookResponse(int BookId, string Title, AuthorResponse Author);

    public record BookSearchRequest(string? SearchTerm, int PageNumber = 1, int PageSize = 10);

    public record PagedResponse<T>(IEnumerable<T> Data, int PageNumber, int PageSize, int TotalCount, int TotalPages);
}
