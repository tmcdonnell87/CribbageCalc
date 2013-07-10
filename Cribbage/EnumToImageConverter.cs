using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cribbage
{
    /// <summary>
    /// Convert an enum to the user-friendly description, if it exists
    /// </summary>
    [ValueConversion(typeof(object), typeof(Image))]
    public class EnumToImageConverter : IValueConverter
    {
        #region IValueConverter implementation

        /// <summary>
        /// Convert from an enum to the friendly description
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type t = value.GetType();
            if (value is Enum)
            {
                return GetEnumImage(value);
            }
            if (value is Array)
            {
                Array arr = (Array)value;
                List<Image> desc = new List<Image>();
                foreach (var val in arr)
                {
                    if (!(val is Enum))
                    {
                        throw new ArgumentException("Value must be Enum or array of Enums");
                    }
                    desc.Add(GetEnumImage(val));
                }
                return desc;
            }
            //what's the plural of enum?
            throw new ArgumentException("Value must be Enum or array of Enums");
        }


        /// <summary>
        /// ConvertBack value from friendly enum description to the source enum
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Gets the image of an enum from the enum itself
        /// </summary>
        /// <param name="enumVal">The enum in question</param>
        /// <returns>The description of the enum, or if none found, the ToString() value of the enum</returns>
        public static Image GetEnumImage(object enumVal)
        {
            if (!(enumVal is Enum)) return null;

            FieldInfo fi = enumVal.GetType().GetField(enumVal.ToString());
            if (fi != null)
            {
                object[] attributes = fi.GetCustomAttributes(typeof(ImageAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    return ((ImageAttribute)attributes[0]).Image;
                }
            }
            return null;
        }
    }

}
