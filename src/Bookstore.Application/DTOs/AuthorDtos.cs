using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.DTOs
{
    public record CreateAuthorRequest(string Name);

    public record AuthorResponse(int AuthorId, string Name);
}
