using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class ProfileData
  {
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public int BoundRoleId { get; set; }
    public bool IsActive { get; set; }
  }
}