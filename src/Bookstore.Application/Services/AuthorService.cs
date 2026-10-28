using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<CreateAuthorRequest> _validator;

        public AuthorService(IAuthorRepository authorRepository, IValidator<CreateAuthorRequest> validator)
        {
            _authorRepository = authorRepository;
            _validator = validator;
        }

        public async Task<IEnumerable<AuthorResponse>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            return authors.Select(a => new AuthorResponse(a.AuthorId, a.Name));
        }

        public async Task<AuthorResponse?> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            return author == null ? null : new AuthorResponse(author.AuthorId, author.Name);
        }

        public async Task<AuthorResponse> CreateAuthorAsync(CreateAuthorRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var author = new Author { Name = request.Name };
            await _authorRepository.AddAsync(author);
            await _authorRepository.SaveChangesAsync();

            return new AuthorResponse(author.AuthorId, author.Name);
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                return false;
            }

            _authorRepository.Delete(author);
            return await _authorRepository.SaveChangesAsync();
        }
    }
}
