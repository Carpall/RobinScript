using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RobinVM.Models
{
    public partial struct Image
    {
        public static Image New(string manifestName, ref Function entryPoint)
        {
            return new Image(manifestName, ref entryPoint);
        }

        readonly public string ManifestName;
        readonly public Function EntryPointPointer;

        readonly CacheTable CacheTable;
        public Image(string manifestName, ref Function entryPoint)
        {
            this.ManifestName = manifestName;
            this.EntryPointPointer = entryPoint;
            this.CacheTable = new CacheTable();

            AddFunction(manifestName, entryPoint);
        }
        public Function FindFunction(string id)
        {
            if (CacheTable.TryGetValue(id, out object value))
                return (Function)value;
            BasePanic.Throw($"Undefined function `{id}`", 24, "Runtime");
            return (Function)new object();
        }
        public Obj FindObj(string id)
        {
            if (CacheTable.TryGetValue(id, out object value))
                return (Obj)value;
            BasePanic.Throw($"Undefined obj `{id}`", 23, "Runtime");
            return (Obj)new object();
        }
        public object FindGlobal(string id)
        {
            if (CacheTable.TryGetValue(id, out object value))
                return (Function)value;
            BasePanic.Throw($"Undefined global variable `{id}`", 22, "Runtime");
            return null;
        }
        public void AddFunction(string id, Function function)
        {
            if (function.UninstantiatedLabels())
                BasePanic.Throw($"Uninstantiated labels table in function `{id}`, instantiate it in this way: `new Function(instructions: null) {{/*body*/}}", 21, "PreRuntime");
            if (!this.CacheTable.TryAdd(id, function))
                BasePanic.Throw($"Already defined function `{id}`", 20, "Runtime");
        }
        public void AddObj(string id, Obj @object)
        {
            @object.CacheTable.TryAdd("$", id);
            if (@object.Ctor == null)
                BasePanic.Throw($"Obj `{id}` does not contain a definition for ctor function", 19, "PreRuntime");
            if (!this.CacheTable.TryAdd(id, @object))
                BasePanic.Throw($"Already defined obj `{id}`", 18, "Runtime");
        }
        public void AddGlobal(string id, object global)
        {
            if (!this.CacheTable.TryAdd(id, global))
                BasePanic.Throw($"Already defined global variable `{id}`", 18, "Runtime");
        }

        public CacheTable GetCacheTable()
        {
            return CacheTable;
        }
    }
}