namespace Domarservice.Helpers
{
  public class RoleBody
  {
    public string Email { get; set; }
    public string Role { get; set; }
    // Used when adding the user role (company/referee) and specifying which id to bind to.
    public int BindToModelId { get; set; }
  }
}