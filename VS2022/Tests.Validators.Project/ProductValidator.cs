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
                .GreaterThanOrEqualTo(m => m.AvailableFrom).WithMessage("Available to must be greater than availble from")
                .Unless(m => m.AvailableTo == null);
            RuleFor(m => m.Name)
                    .NotEmpty().WithMessage("Name is requried")
                    .Length(3, 5).WithMessage("Name must be between 3 and 5 characters long.");
            RuleFor(m => m.ProductId).GreaterThanOrEqualTo(0).LessThanOrEqualTo(34534);
            RuleFor(m => m.RRP).GreaterThanOrEqualTo(m=>m.CostPrice)
                .LessThan(1234);
                

            RuleFor(m => m.ProductDetails)
                .NotNull().WithMessage("Details are required!");
            RuleFor(m => m.CostPrice).GreaterThan(0);
        }
    }
    public class ProductDetailsValidator : AbstractValidator<ProductDetails>, IValidator
    {
        public ProductDetailsValidator()
        {
            RuleFor(m => m.Name)
                .Length(4, 6);
        }
    }

    public class ProductSupplierValidator : AbstractValidator<ProductSupplier>, IValidator
    {
        public ProductSupplierValidator()
        {
            RuleFor(m => m.SupplierName)
                .Length(9);
        }
    }
}