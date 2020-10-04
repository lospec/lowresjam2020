using System;
using System.Collections.Generic;

namespace HeroesGuild.data
{
    public interface IAudioCollection<T> where T : IDataRecord
    {
        public Dictionary<string, Func<T>> KeyValue { set; }
        public bool TryGetValue(string key, out T record);
    }
}