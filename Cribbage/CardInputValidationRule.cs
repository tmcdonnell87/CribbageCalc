using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cribbage
{
    class CardInputValidationRule : ValidationRule
    {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {

            string input = (value ?? string.Empty).ToString();
            if (String.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(true, null);

            }
            try
            {
                CardToStringConverter.ConvertStringToCard(input);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, ex.Message);
            }

            return new ValidationResult(true, null);
        }

    }
}
