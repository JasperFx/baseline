using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using BaselineTypeDiscovery;

[assembly: IgnoreAssembly]

namespace BaselineTypeDiscovery
{
    /// <summary>
    /// Use to walk up the execution stack and "find" the assembly
    /// that originates the call. Ignores system assemblies and any
    /// assembly marked with the [IgnoreOnScanning] attribute
    /// </summary>
    public class CallingAssembly
    {
        /// <summary>
        /// Method is used to get the stack trace in english
        /// </summary>
        /// <returns>Stack trace in english</returns>
        private static string GetStackTraceInEnglish()
        {
            var currentUiCulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            string trace = Environment.StackTrace;
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;
            return trace;
        }

        
        public static Assembly Find()
        {
            string trace = GetStackTraceInEnglish();



            var parts = trace.Split('\n');

            for (int i = 0; i < parts.Length; i++)
            {
                var line = parts[i];
                var assembly = findAssembly(line);
                if (assembly != null && !isSystemAssembly(assembly))
                {
                    return assembly;
                }
            }

            return Assembly.GetEntryAssembly();
        }
        
        private static string[] _prefixesToIgnore = new []{"System.", "Microsoft."};

        private static bool isSystemAssembly(Assembly assembly)
        {
            if (assembly == null) return false;

            if (assembly.GetCustomAttributes<IgnoreAssemblyAttribute>().Any()) return true;

            var assemblyName = assembly.GetName().Name;
            
            return isSystemAssembly(assemblyName);
        }

        private static bool isSystemAssembly(string assemblyName)
        {
            return _prefixesToIgnore.Any(x => assemblyName.StartsWith(x));
        }

        private static readonly IList<string> _misses = new List<string>();

        private static Assembly findAssembly(string stacktraceLine)
        {
            var candidate = stacktraceLine.Trim().Substring(3);

            // Short circuit this
            if (isSystemAssembly(candidate)) return null;

            Assembly assembly = null;
            var names = candidate.Split('.');
            for (var i = names.Length - 2; i > 0; i--)
            {
                var possibility = string.Join(".", names.Take(i).ToArray());

                if (_misses.Contains(possibility)) continue;

                try
                {

                    assembly = Assembly.Load(new AssemblyName(possibility));
                    break;
                }
                catch
                {
                    _misses.Add(possibility);
                }
            }

            return assembly;
        }

        /// <summary>
        /// Finds the calling assembly from the specified type
        /// </summary>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static Assembly DetermineApplicationAssembly(object registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            
            var assembly = registry.GetType().Assembly;
            return isSystemAssembly(assembly) ? Find() : assembly;
        }
    }
}
