using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Domain.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Book> Books { get; set; } = new List<Book>();

    }
}
