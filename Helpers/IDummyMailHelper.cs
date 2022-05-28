using System;

namespace Domarservice.Helpers
{
  interface IDummyMailHelper
  {
    bool SendMail(string text);
  }
}