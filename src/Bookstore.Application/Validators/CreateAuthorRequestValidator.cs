using Bookstore.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bookstore.Application.Validators
{
    public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
    {
        public CreateAuthorRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Author name is required.")
                .MinimumLength(3).WithMessage("Author name must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Author name cannot exceed 100 characters.");
        }
    }
}
