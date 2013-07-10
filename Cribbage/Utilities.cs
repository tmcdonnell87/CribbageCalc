using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cribbage
{
    class Utilities
    {

        public static long Combination(int n, int k)
        {
            long result = 1;
            for (long i = Math.Max(k, n - k) + 1; i <= n; ++i)
                result *= i;

            for (long i = 2; i <= Math.Min(k, n - k); ++i)
                result /= i;

            return result;
        }

        private HashSet<HashSet<T>> GetPowerSetRecursive<T>(ICollection<T> set)
        {
            HashSet<HashSet<T>> result = new HashSet<HashSet<T>>(HashSet<T>.CreateSetComparer());
            result.Add(new HashSet<T>(set));
            if (set.Count() < 2)
            {
                return result;
            }

            HashSet<T> subSet;

            foreach (T obj in set)
            {
                subSet = new HashSet<T>(set);
                subSet.Remove(obj);
                if (subSet.Count < 2)
                {
                    result.Add(subSet);
                }
                else
                {
                    result.UnionWith(GetPowerSetRecursive<T>(subSet));
                }
            }
            return result;

        }

        public static IList<string> GetMatchesForEnum(Enum val)
        {
            string name = val.ToString();
            IList<string> matches = new List<string>();
            matches.Add(name);

            FieldInfo fi = val.GetType().GetField(name);
            if (fi != null)
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(fi);
                foreach (Attribute attr in attributes)
                {
                    if(attr is AbbreviationAttribute)
                    {
                        matches.Add(((AbbreviationAttribute)attr).Abbreviation);
                    }
                    else if(attr is SynonymAttribute)
                    {
                         matches.Add(((SynonymAttribute)attr).Synonym);
                    }
                }

            }

            return matches;
        }

    }
}
