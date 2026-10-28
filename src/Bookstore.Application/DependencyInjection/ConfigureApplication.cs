using Bookstore.Application.Interfaces;
using Bookstore.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bookstore.Application.DependencyInjection
{
    public static class ConfigureApplication
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthorService, AuthorService>();

            return services;
        }
    }
}
