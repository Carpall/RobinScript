using System;
using System.Collections.Generic;
using System.Text;

namespace RobinVM.Models
{
    public struct CacheTable
    {
        Dictionary<string, object> Members;
        public CacheTable(Dictionary<string, object> members) => Members = members;
        public static CacheTable New() => new CacheTable { Members = new Dictionary<string, object>() };
        public object this[string id] { get => Members[id]; set => Members[id] = value; }
        public void Set(string id, object value) => Members[id] = value;
        public void Add(string id, object value) => Members.Add(id, value);
        public bool TryAdd(string id, object value) => Members.TryAdd(id, value);
        public bool TryGetValue(string id, out object value) => Members.TryGetValue(id, out value);
        public CacheTable Clone() => new CacheTable { Members = new Dictionary<string, object>(Members) };
        public override string ToString() => $"Instance[{Members["$"]}]";
    }
}
