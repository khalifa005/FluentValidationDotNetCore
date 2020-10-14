using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalModels;
using FluentValidation;

namespace FluentValidationDotNetCore.Validators
{
    public class DeveloperValidator : AbstractValidator<Developer>
    {
        private bool IsValidName(string name)
        {
            return name.All(Char.IsLetter);
        }
        public DeveloperValidator()
        {
            RuleFor(X => X.FirstName)
                .NotEmpty()
                .WithMessage("required_first_name")
                .Length(2,10).WithMessage("first_name_length_should_be_between_2_and_10")
                .Must(x=>x.All(char.IsLetter)).WithMessage("first_name_should_be_characters_only");

            RuleFor(x => x.Email).EmailAddress();
        }
    }
}
