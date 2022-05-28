using System;
using Microsoft.Extensions.Logging;

namespace Domarservice.Helpers
{
  public class DummyMailHelper : IDummyMailHelper
  {
    private static ILogger _logger;
    public DummyMailHelper(ILogger<DummyMailHelper> logger)
    {
      _logger = logger;
    }

    public bool SendMail(string text)
    {
      _logger.LogInformation($"Sending mail: {text}");
      return true;
    }
  }
}