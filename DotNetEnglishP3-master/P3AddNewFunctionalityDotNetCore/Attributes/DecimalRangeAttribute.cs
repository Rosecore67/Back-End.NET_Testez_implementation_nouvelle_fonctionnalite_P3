using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace P3AddNewFunctionalityDotNetCore.Attributes
{
    public class DecimalRangeAttribute : ValidationAttribute
    {
        private readonly double _minimum;
        private readonly double _maximum;

        public DecimalRangeAttribute(double minimum, double maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !(value is string))
            {
                return new ValidationResult("Invalid value type.");
            }

            var stringValue = value as string;

            if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ||
                double.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                if (result >= _minimum && result <= _maximum)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult($"The field {validationContext.DisplayName} must be between {_minimum} and {_maximum}.");
            }

            return new ValidationResult($"The field {validationContext.DisplayName} is not a valid decimal number.");
        }
    }
}
