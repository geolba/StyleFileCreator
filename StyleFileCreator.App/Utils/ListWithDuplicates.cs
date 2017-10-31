using System;
using System.Collections.Generic;

namespace StyleFileCreator.App.Utils
{
    public class ListWithDuplicates<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            var element = new KeyValuePair<TKey, TValue>(key, value);
            this.Add(element);
        }
    }
}
