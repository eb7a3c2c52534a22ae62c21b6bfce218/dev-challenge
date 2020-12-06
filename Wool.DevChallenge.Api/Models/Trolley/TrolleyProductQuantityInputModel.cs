using FluentValidation;

namespace Wool.DevChallenge.Api.Models.Trolley
{
    public class TrolleyProductQuantityInputModel
    {
        public string Name { get; set; }
        public long Quantity { get; set; }

        public class TrolleyProductQuantityInputModelValidator : AbstractValidator<TrolleyProductQuantityInputModel>
        {
            public TrolleyProductQuantityInputModelValidator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required");
                RuleFor(x => x.Quantity).Must(x => x > 0).WithMessage("Quantity invalid");
            }
        }

    }
}