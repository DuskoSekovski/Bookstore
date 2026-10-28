using Bookstore.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookstore.Application.Validators
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("A valid Author ID is required.");
        }
    }
}
