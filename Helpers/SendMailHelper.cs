using Microsoft.Extensions.Logging;

namespace Domarservice.Helpers
{
  public class SendMailHelper : ISendMailHelper
  {
    private static ILogger _logger;
    public SendMailHelper(ILogger<SendMailHelper> logger)
    {
      _logger = logger;
    }
    public bool Send(string text)
    {
      _logger.LogInformation($"Mailhelper: {text}");
      return true;
    }
  }
}