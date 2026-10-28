using Bookstore.Domain.Entities;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookstoreDbContext>();

            await context.Database.MigrateAsync();

            if (!await context.Authors.AnyAsync())
            {
                var martin = new Author { Name = "Robert C. Martin" };
                var evans = new Author { Name = "Eric Evans" };

                var sampleBooks = new List<Book>
            {
                new Book { Title = "Clean Code", Author = martin },
                new Book { Title = "Clean Architecture", Author = martin },
                new Book { Title = "Domain-Driven Design", Author = evans }
            };

                await context.Authors.AddRangeAsync(martin, evans);
                await context.Books.AddRangeAsync(sampleBooks);
                await context.SaveChangesAsync();
            }
        }
    }
}
