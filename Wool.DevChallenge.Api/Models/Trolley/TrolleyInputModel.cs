using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Wool.DevChallenge.Api.Models.Trolley
{
    public class TrolleyInputModel
    {
        public IEnumerable<TrolleyProductInputModel> Products { get; set; }
        public IEnumerable<TrolleySpecialInputModel> Specials { get; set; }
        public IEnumerable<TrolleyProductQuantityInputModel> Quantities { get; set; }

        public class TrolleyInputModelValidator : AbstractValidator<TrolleyInputModel>
        {
            public TrolleyInputModelValidator()
            {
                RuleFor(x => x.Products)
                    .NotNull()
                    .NotEmpty().WithMessage("No Products in the trolley");

                RuleFor(x => x.Quantities)
                    .NotNull()
                    .NotEmpty().WithMessage("Products Quantities are invalid");

                RuleFor(x => x)
                    .Must(x => !x.Quantities.Select(q => q.Name).Except(x.Products.Select(x => x.Name)).Any())
                    .WithName("Quantities").WithMessage("Products and quantities are out of sync");

                RuleFor(x => x)
                    .Must(x => !x.Specials.SelectMany(q => q.Quantities.Select(x => x.Name))
                        .Except(x.Products.Select(x => x.Name)).Any())
                    .WithName("Specials").WithMessage("Products and Specials are out of sync");

            }

        }

    }
}
