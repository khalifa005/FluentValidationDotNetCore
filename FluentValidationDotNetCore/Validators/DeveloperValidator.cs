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
        public DeveloperValidator()
        {
            RuleFor(X => X.FirstName).NotEmpty().WithMessage("required_first_name");
        }
    }
}
