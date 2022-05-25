namespace Domarservice.Helpers
{
  public class RefreshToken
  {
    // Just used to handle the incoming JSON to the Authentication controller refresh-token endpoint.
    public string? AccessToken { get; set; }
  }
}