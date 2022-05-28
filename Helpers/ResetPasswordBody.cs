namespace Domarservice.Helpers
{
  public class ResetPasswordBody
  {
    public string email { get; set; }
    public string passwordToken { get; set; }
    public string newPassword { get; set; }
  }
}