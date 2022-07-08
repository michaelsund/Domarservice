using System.ComponentModel.DataAnnotations;

namespace Domarservice.DAL
{
  public class RegisterModel
  {
    [Required(ErrorMessage = "Surname is required")]
    public string? Surname { get; set; }
    
    [Required(ErrorMessage = "Surname is required")]
    public string? Lastname { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }

    public string? Information { get; set; }
  }
}