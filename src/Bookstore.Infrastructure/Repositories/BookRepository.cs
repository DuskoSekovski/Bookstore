using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookstoreDbContext _context;

        public BookRepository(BookstoreDbContext context)
        {
            _context = context;
        }


        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.Include(b => b.Author).ToListAsync();
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
        }

        public void Delete(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(IEnumerable<Book> Items, int TotalCount)> SearchAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Books.Include(b => b.Author).AsQueryable();

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(lowerSearch) || b.Author.Name.ToLower().Contains(lowerSearch));
            }

            var totalCount = await query.CountAsync();

            var items = await query.OrderBy(b => b.Title).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
    }
}
