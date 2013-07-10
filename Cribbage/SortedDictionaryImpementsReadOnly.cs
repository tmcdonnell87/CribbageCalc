using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cribbage
{
    class SortedDictionaryImpementsReadOnly<TKey,TValue> : SortedDictionary<TKey,TValue>, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
    {

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return base.Values; }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return base.Keys; }
        }

    }
}
