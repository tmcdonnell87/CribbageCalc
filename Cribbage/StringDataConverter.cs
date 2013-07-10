using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cribbage
{
    [ValueConversion(typeof(string), typeof(bool))]
    class StringDataConverter:IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a string to a boolean based on whether the string contains data.
        /// </summary>
        /// <param name="value">The value produced by the binding target.</param>
        /// <param name="targetType">The type to convert to (should be bool).</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>The pleuralized phrase</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                throw new InvalidOperationException("Base value must be a string");

            return (!String.IsNullOrEmpty((String)value));

        }

        /// <summary>
        /// This method is not supported and will throw a NotSupportedException.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
