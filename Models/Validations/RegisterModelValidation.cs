using Domarservice.DAL;
using FluentValidation;
namespace Domarservice.DAL
{
  public class RegisterModelValidation : AbstractValidator<RegisterModel>
  {
    public RegisterModelValidation()
    {
      RuleFor(registration => registration.CompanyName)
        .NotEmpty()
        .NotEqual("")
        .WithMessage("Föreningens namn är obligatoriskt.")
        .When(registration => !registration.RegisterAsReferee);
      
      RuleFor(registration => registration.CompanyCity)
        .NotEmpty()
        .NotEqual("")
        .WithMessage("Föreningens stad är obligatoriskt.")
        .When(registration => !registration.RegisterAsReferee);
      
      RuleFor(registration => registration.CompanyCounty)
        .NotEmpty()
        .WithMessage("Föreningens län är obligatoriskt.")
        .When(registration => !registration.RegisterAsReferee);
    }
  }
}