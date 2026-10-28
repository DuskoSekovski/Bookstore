using Bookstore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(int id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task AddAsync(Book book);
        void Update(Book book);
        void Delete(Book book);
        Task<bool> SaveChangesAsync();
        Task<(IEnumerable<Book> Items, int TotalCount)> SearchAsync(string? searchTerm, int pageNumber, int pageSize);
    }
}
