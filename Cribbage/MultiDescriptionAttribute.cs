using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cribbage
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SynonymAttribute:Attribute
    {
        public String Synonym
        {
            private set;
            get;
        }

        public SynonymAttribute(string synonym)
        {
            this.Synonym = synonym;
        }
    }
}
