using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cribbage
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AbbreviationAttribute:Attribute
    {
        public String Abbreviation
        {
            private set;
            get;
        }

        public AbbreviationAttribute(string abbreviation)
        {
            this.Abbreviation = abbreviation;
        }
    }
}
