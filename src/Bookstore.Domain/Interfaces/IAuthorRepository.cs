using Bookstore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Domain.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<Author?> GetByIdAsync(int id);
        Task AddAsync(Author author);
        void Delete(Author author);
        Task<bool> SaveChangesAsync();
    }
}
