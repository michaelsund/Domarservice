using System.ComponentModel.DataAnnotations;

namespace Domarservice.DAL
{
  public class RegisterModel
  {
    [StringLength(30, ErrorMessage = "Ditt förnamn får inte vara längre än 30 tecken.")]
    [Required(ErrorMessage = "Förnamn är obligatoriskt.")]
    public string? Surname { get; set; }
    
    [StringLength(30, ErrorMessage = "Ditt Efternamn får inte vara längre än 30 tecken.")]
    [Required(ErrorMessage = "Efternamn är obligatoriskt.")]
    public string? Lastname { get; set; }

    [EmailAddress(ErrorMessage = "Epost-adressen du angav är inte giltig.")]
    [StringLength(50, ErrorMessage = "Din epost får inte vara längre än 50 tecken.")]
    [Required(ErrorMessage = "Epost är obligatoriskt.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Lösenord är obligatoriskt")]
    public string? Password { get; set; }

    public string? Information { get; set; }
    public bool RegisterAsReferee { get; set; }


    // Used if it's a user registering for a company, basic info. The rest can be edited once the account is active.
    public string? CompanyName { get; set; }
    public string? CompanyCity { get; set; }
    public CountyType CompanyCounty { get; set; }
  }
}