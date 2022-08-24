using System.ComponentModel.DataAnnotations;
using Domarservice.Helpers;

namespace Domarservice.DAL
{
  public class RegisterModel
  {
    public string? Surname { get; set; }
    public string? Lastname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PasswordConfirmation { get; set; }
    public string? Information { get; set; }
    public bool RegisterAsReferee { get; set; }

    // Used if it's a user registering for a company, basic info. The rest can be edited once the account is active.
    public string CompanyName { get; set; }
    public string CompanyCity { get; set; }
    public CountyType CompanyCounty { get; set; }
  }
}