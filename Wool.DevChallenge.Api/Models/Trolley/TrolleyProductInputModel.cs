using FluentValidation;

namespace Wool.DevChallenge.Api.Models.Trolley
{
    public class TrolleyProductInputModel
    {
        public string Name { get; set; }
        public double Price { get; set; }

        public class TrolleyProductInputModelValidator : AbstractValidator<TrolleyProductInputModel>
        {
            public TrolleyProductInputModelValidator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required");
                RuleFor(x => x.Price).Must(x => x > 0).WithMessage("Price is invalid");
            }
        }
    }
}