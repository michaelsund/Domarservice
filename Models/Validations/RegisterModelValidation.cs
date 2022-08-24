using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Domarservice.DAL
{
  public class RegisterModelValidation : AbstractValidator<RegisterModel>
  {
    private readonly ICompanyRepository _companyRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private bool CompanyNameUnique(string name)
    {
      if (_companyRepository.CompanyUniqueByName(name))
      {
        return true;
      }
      return false;
    }

    private bool UserEmailUnique(string email)
    {
      var user = _userManager.FindByNameAsync(email);
      if (user == null)
      {
        return true;
      }
      return false;
    }

    public RegisterModelValidation(ICompanyRepository companyRepository, UserManager<ApplicationUser> userManager)
    {
      _companyRepository = companyRepository;
      _userManager = userManager;

      RuleFor(registration => registration.Surname)
        .NotEmpty()
        .WithMessage("Förnamn är obligatorisk.")
        .MaximumLength(40)
        .WithMessage("Förnamn får max vara 40 tecken långt.");

      RuleFor(registration => registration.Lastname)
        .NotEmpty()
        .WithMessage("Efternamn är obligatorisk.")
        .MaximumLength(40)
        .WithMessage("Efternamn får max vara 40 tecken långt.");

      RuleFor(registration => registration.Email)
        .Must(UserEmailUnique)
        .WithMessage("Eposten är redan registrerad.")
        .NotEmpty()
        .WithMessage("Epost är obligatorisk.")
        .EmailAddress()
        .WithMessage("Epost adressen är inte giltig.");

      RuleFor(customer => customer.Password)
        .NotEmpty().WithMessage("Your password cannot be empty")
        .MinimumLength(8).WithMessage("Ditt lösenord måste vara minst 8 tecken långt.")
        .Matches(@"[A-Z]+").WithMessage("Ditt lösenord måste innehålla minst en stor bokstav.")
        .Matches(@"[a-z]+").WithMessage("Ditt lösenord måste innehåll minst en liten bokstav.")
        .Matches(@"[0-9]+").WithMessage("Ditt lösenord måste innehålla minst en siffra.")
        .Equal(customer => customer.PasswordConfirmation)
        .WithMessage("Lösenorden matchar inte.");
       
      // When registering as a company

      RuleFor(registration => registration.CompanyName)
        .Must(CompanyNameUnique)
        .WithMessage("Föreningens namn är redan registrerat.")
        .NotEmpty()
        .WithMessage("Föreningens namn är obligatorisk.")
        .MinimumLength(2)
        .WithMessage("Föreningens namn får som minst vara 2 tecken långt.")
        .MaximumLength(50)
        .WithMessage("Föreningens namn får max vara 50 tecken långt.")
        .When(registration => !registration.RegisterAsReferee);

      RuleFor(registration => registration.CompanyCity)
        .NotEmpty()
        .WithMessage("Föreningens stad är obligatorisk.")
        .MinimumLength(2)
        .WithMessage("Föreningens stad får som minst vara 2 tecken långt.")
        .MaximumLength(40)
        .WithMessage("Föreningens stad får max vara 40 tecken långt.")
        .When(registration => !registration.RegisterAsReferee);

      RuleFor(registration => registration.CompanyCounty)
        .NotEmpty()
        .WithMessage("Föreningens län är obligatorisk.")
        .When(registration => !registration.RegisterAsReferee);
    }
  }
}