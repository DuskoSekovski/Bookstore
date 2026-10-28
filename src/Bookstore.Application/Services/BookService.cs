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
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<CreateBookRequest> _validator;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IValidator<CreateBookRequest> validator)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _validator = validator;
        }


        public async Task<BookResponse?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if(book == null)
            {
                return null;
            }

            return MapToResponse(book);
        }

        public async Task<IEnumerable<BookResponse>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(MapToResponse);
        }

        public async Task<BookResponse> CreateBookAsync(CreateBookRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var author = await _authorRepository.GetByIdAsync(request.AuthorId);
            if (author == null)
            {
                throw new ValidationException($"Author with ID {request.AuthorId} does not exist.");
            }

            var book = new Book
            {
                Title = request.Title,
                AuthorId = request.AuthorId,
                Author = author
            };

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            return MapToResponse(book);
        }

        public async Task<bool> UpdateBookAsync(int id, CreateBookRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                return false;
            }

            existingBook.Title = request.Title;
            existingBook.AuthorId = request.AuthorId;

            _bookRepository.Update(existingBook);
            return await _bookRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id); 
            if (book == null)
            {
                return false; 
            }

            _bookRepository.Delete(book);
            return await _bookRepository.SaveChangesAsync();
        }

        private static BookResponse MapToResponse(Book book)
        {
            if(book.Author == null)
            {
                throw new InvalidOperationException($"Book with ID {book.BookId} is missing its Author data.");
            }

            return new BookResponse(
                book.BookId,
                book.Title,
                new AuthorResponse(book.Author.AuthorId, book.Author.Name));
        }

        public async Task<PagedResponse<BookResponse>> SearchBooksAsync(BookSearchRequest request)
        {
            int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize > 50 ? 50 : request.PageSize;

            var (books, totalCount) = await _bookRepository.SearchAsync(request.SearchTerm, pageNumber, pageSize);

            var mappedBooks = books.Select(MapToResponse);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResponse<BookResponse>(mappedBooks, pageNumber, pageSize, totalCount, totalPages);
        }
    }
}
