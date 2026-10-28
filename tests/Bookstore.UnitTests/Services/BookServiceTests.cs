using Bookstore.Application.DTOs;
using Bookstore.Application.Services;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.UnitTests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepo;
        private readonly Mock<IAuthorRepository> _mockAuthorRepo;
        private readonly Mock<IValidator<CreateBookRequest>> _mockValidator;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepo = new Mock<IBookRepository>();
            _mockAuthorRepo = new Mock<IAuthorRepository>();
            _mockValidator = new Mock<IValidator<CreateBookRequest>>();

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateBookRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookService = new BookService(_mockBookRepo.Object, _mockAuthorRepo.Object, _mockValidator.Object);
        }


        [Fact]
        public async Task CreateBookAsync_ShouldCreateBook_WhenDataIsValid()
        {
            //Arrange
            var request = new CreateBookRequest("Clean Architecture", 1);
            var author = new Author { AuthorId = 1, Name = "Robert C. Martin" };

            _mockAuthorRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(author);
            _mockBookRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            //Act
            var result = await _bookService.CreateBookAsync(request);

            //Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Clean Architecture");
            result.Author.Name.Should().Be("Robert C. Martin");

            _mockBookRepo.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
            _mockBookRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookAsync_ShouldThrowException_WhenAuthorDoesNotExist()
        {
            // Arrange
            var request = new CreateBookRequest("Clean Architecture", 99);

            _mockAuthorRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Author?)null);

            //Act
            var act = async () => await _bookService.CreateBookAsync(request);

            //Assert
            await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Author with ID 99 does not exist*");
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Book?)null);

            // Act
            var result = await _bookService.DeleteBookAsync(99);

            // Assert
            result.Should().BeFalse();
            _mockBookRepo.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task SearchBooksAsync_ShouldCalculatePaginationCorrectly()
        {
            // Arrange
            var request = new BookSearchRequest ("Test", 1, 10);
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Test Book 1", AuthorId = 1, Author = new Author { Name = "Author 1" } }
        };

            _mockBookRepo.Setup(r => r.SearchAsync("Test", 1, 10)).ReturnsAsync((books, 25));

            // Act
            var result = await _bookService.SearchBooksAsync(request);

            // Assert
            result.TotalCount.Should().Be(25);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(3);
            result.Data.Should().HaveCount(1);
        }
    }
}
