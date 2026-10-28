using Bookstore.Domain.Interfaces;
using Bookstore.Infrastructure.Data;
using Bookstore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Infrastructure.DependencyInjection
{
    public static class ConfigureInfrastructure
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<BookstoreDbContext>(options =>
                options.UseSqlServer(connectionString, b =>
                    b.MigrationsAssembly(typeof(BookstoreDbContext).Assembly.FullName)));

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();

            return services;
        }
    }
}
