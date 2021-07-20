using System;
using FluentValidation;
using Test.ModelsProject;

namespace Tests.ValidatorsProject
{
    public class ProductValidator : AbstractValidator<Product>, IValidator
    {
        public ProductValidator()
        {
            RuleFor(m => m.AvailableFrom).GreaterThanOrEqualTo(new DateTime(2000, 1, 1)).WithMessage("Available from must be greater than 1 Jan 2000");
            RuleFor(m => m.AvailableTo)
                .GreaterThanOrEqualTo(m => m.AvailableFrom)
                .WithMessage("Available to must be greater than availble from")
                .Unless(m => m.AvailableTo == null);
            RuleFor(m => m.Name).NotEmpty()
                    .Length(3, 5);
            RuleFor(m => m.ProductId).GreaterThanOrEqualTo(0);
            RuleFor(m => m.RRP).GreaterThan(0);
        }
    }
}