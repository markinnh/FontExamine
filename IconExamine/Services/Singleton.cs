using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Services
{
    public static class Singleton<T>
        where T : new()
    {
        private static readonly ConcurrentDictionary<Type, T> _instances = new();

        public static T Instance => _instances.GetOrAdd(typeof(T), (t) => new T());
    }
}
