using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cribbage
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    class ImageAttribute : Attribute
    {

        public Image Image
        {
            get;
            set;
        }

        public ImageAttribute(string path)
        {
            this.Image = new Image();
            this.Image.Source = new BitmapImage(new Uri(path, UriKind.Relative));
        }

        public ImageAttribute(string path, UriKind uriKind)
        {
            this.Image = new Image();
            BitmapImage source = new BitmapImage(new Uri(path, uriKind));
            this.Image.Source = source;
        }
    }
}
