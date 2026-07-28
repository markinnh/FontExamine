using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    public static class Param
    {
        public static Selector<T> From<T>() => new Selector<T>();

        public class Selector<T>
        {
            private readonly Dictionary<string, Func<T, object>> _map = new();
            public string[] Keys=>_map.Keys.ToArray();
            public Selector<T> Add<TProp>(string key, Func<T, TProp> selector)
            {

#pragma warning disable CS8603 // Possible null reference return.
                _map[key] = x => selector(x);
#pragma warning restore CS8603 // Possible null reference return.
                return this;
            }

            public object Select(T instance, string key)
            {
                if (_map.ContainsKey(key))
                    return _map[key](instance);
                else
                    throw new ArgumentOutOfRangeException($"key = '{key}' not found in the dictionary, perhaps it was not intialized");
            }
        }
    }
}
