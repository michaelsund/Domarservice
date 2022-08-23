using System.Threading.Tasks;
using FluentValidation;
namespace Domarservice.DAL
{
  public class RegisterModelValidation : AbstractValidator<RegisterModel>
  {
    private readonly ICompanyRepository _companyRepository;
    private bool BeUnique(string name)
    {
      if (_companyRepository.CompanyUniqueByName(name))
      {
        return true;
      }
      return false;
    }

    public RegisterModelValidation(ICompanyRepository companyRepository)
    {
      _companyRepository = companyRepository;

      RuleFor(registration => registration.CompanyName)
        .Must(BeUnique)
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
        .WithMessage("Föreningens stad får max vara 50 tecken långt.")
        .When(registration => !registration.RegisterAsReferee);

      RuleFor(registration => registration.CompanyCounty)
        .NotEmpty()
        .WithMessage("Föreningens län är obligatorisk.")
        .When(registration => !registration.RegisterAsReferee);
    }
  }
}