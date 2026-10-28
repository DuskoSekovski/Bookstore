using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookstoreDbContext _context;

        public AuthorRepository(BookstoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task AddAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Delete(Author author)
        {
            _context.Authors.Remove(author);
        }
    }
}
