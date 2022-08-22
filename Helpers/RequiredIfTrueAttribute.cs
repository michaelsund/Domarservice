using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Domarservice.Helpers
{
  public class RequiredIfAttribute : ValidationAttribute
  {
    public string PropertyName { get; set; }
    public object Value { get; set; }


    public RequiredIfAttribute(string propertyName, object value, string errorMessage = "")
    {
      PropertyName = propertyName;
      ErrorMessage = errorMessage;
      Value = value;
    }


    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      var instance = validationContext.ObjectInstance;
      var type = instance.GetType();
      var proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
      if (proprtyvalue != null)
      {
        if (proprtyvalue.ToString() == Value.ToString() && value == null)
        {
          return new ValidationResult(ErrorMessage);
        }
      }
      return ValidationResult.Success;
    }


    // public void AddValidation(ClientModelValidationContext context)
    // {
    //   context.Attributes.Add("data-val", "true");
    //   context.Attributes.Add("data-val-vatNumber", ErrorMessage);
    //   context.Attributes.Add("data-val-vatNumber-businessType", Value.ToString());
    // }
  }
}