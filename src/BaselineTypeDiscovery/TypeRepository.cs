using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BaselineTypeDiscovery
{
    // Really only tested in integration with other things
    /// <summary>
    /// Mechanism to analyze and scan assemblies for exported types
    /// </summary>
    public static class TypeRepository
    {
        private static ImHashMap<Assembly, Task<AssemblyTypes>> _assemblies = ImHashMap<Assembly, Task<AssemblyTypes>>.Empty;
        
        public static void ClearAll()
        {
            _assemblies = ImHashMap<Assembly, Task<AssemblyTypes>>.Empty;
        }

        /// <summary>
        /// Use to assert that there were no failures in type scanning when trying to find the exported types
        /// from any Assembly
        /// </summary>
        public static void AssertNoTypeScanningFailures()
        {
            var exceptions =
                FailedAssemblies().Select(x => x.Record.LoadException);


            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Query for all assemblies that could not be scanned, usually because
        /// of missing dependencies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AssemblyTypes> FailedAssemblies()
        {
            var tasks = _assemblies.Enumerate().Select(x => x.Value).ToArray();
            Task.WaitAll(tasks);

            return tasks.Where(x => x.Result.Record.LoadException != null).Select(x => x.Result);
        }

        /// <summary>
        /// Scan a single assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Task<AssemblyTypes> ForAssembly(Assembly assembly)
        {
            if (_assemblies.TryFind(assembly, out var types))
            {
                return types;
            }

            types = Task.Factory.StartNew(() => new AssemblyTypes(assembly));
            _assemblies = _assemblies.AddOrUpdate(assembly, types);

            return types;
        }

        /// <summary>
        /// Find types matching a certain criteria from an assembly
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Task<TypeSet> FindTypes(IEnumerable<Assembly> assemblies, Func<Type, bool> filter = null)
        {
            var tasks = assemblies.Select(ForAssembly).ToArray();
            return Task.Factory.ContinueWhenAll(tasks, assems =>
            {
                return new TypeSet(assems.Select(x => x.Result).ToArray(), filter);
            });
        }

        
        /// <summary>
        /// Find types matching a certain criteria and TypeClassification from an Assembly
        /// </summary>
        /// <param name="assemblies"></param>
        /// <param name="classification"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Type>> FindTypes(IEnumerable<Assembly> assemblies,
            TypeClassification classification, Func<Type, bool> filter = null)
        {
            var query = new TypeQuery(classification, filter);

            var tasks = assemblies.Select(assem => ForAssembly(assem).ContinueWith(t => query.Find(t.Result))).ToArray();
            return Task.Factory.ContinueWhenAll(tasks, results => results.SelectMany(x => x.Result));
        }

        
        public static Task<IEnumerable<Type>> FindTypes(Assembly assembly, TypeClassification classification,
            Func<Type, bool> filter = null)
        {
            if (assembly == null) return Task.FromResult((IEnumerable<Type>)new Type[0]);

            var query = new TypeQuery(classification, filter);

            return ForAssembly(assembly).ContinueWith(t => query.Find(t.Result));
        }
    }
}
