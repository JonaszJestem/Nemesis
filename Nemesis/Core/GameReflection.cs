using System.Collections.Generic;
using System.Reflection;
using MimicAPI.GameAPI;

namespace Nemesis.Core
{
    /// <summary>
    /// Helpers for common reflection patterns against game assemblies.
    /// Eliminates boilerplate in Harmony TargetMethods() implementations.
    /// </summary>
    internal static class GameReflection
    {
        private static readonly BindingFlags AllInstance =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static readonly BindingFlags AllInstanceAndStatic =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// Find a method on a game type by name. Returns null if not found.
        /// </summary>
        public static MethodInfo? FindMethod(string typeName, string methodName, bool includeStatic = false)
        {
            var assembly = ServerNetworkAPI.GetGameAssembly();
            if (assembly == null) return null;

            var type = assembly.GetType(typeName);
            if (type == null) return null;

            var flags = includeStatic ? AllInstanceAndStatic : AllInstance;
            return type.GetMethod(methodName, flags);
        }

        /// <summary>
        /// Collect methods into a list for Harmony TargetMethods().
        /// Tries each (typeName, methodName) pair in order, stops after first match if stopOnFirst is true.
        /// </summary>
        public static List<MethodBase> FindTargetMethods(params (string typeName, string methodName)[] candidates)
        {
            return FindTargetMethods(false, candidates);
        }

        /// <summary>
        /// Collect methods into a list for Harmony TargetMethods().
        /// If stopOnFirst is true, returns as soon as one method is found (useful for fallback chains).
        /// </summary>
        public static List<MethodBase> FindTargetMethods(bool stopOnFirst, params (string typeName, string methodName)[] candidates)
        {
            var methods = new List<MethodBase>();
            foreach (var (typeName, methodName) in candidates)
            {
                var method = FindMethod(typeName, methodName);
                if (method != null)
                {
                    methods.Add(method);
                    if (stopOnFirst) break;
                }
            }
            return methods;
        }

        /// <summary>
        /// Check if an object is the local player's avatar (client-side check).
        /// </summary>
        public static bool IsLocalAvatar(object instance)
        {
            var result = ReflectionHelper.InvokeMethod(instance, GameMethodNames.ProtoActor_AmIAvatar);
            return result is bool b && b;
        }

        /// <summary>
        /// Get a game type from Assembly-CSharp by name.
        /// </summary>
        public static System.Type? GetGameType(string typeName)
        {
            return ServerNetworkAPI.GetGameAssembly()?.GetType(typeName);
        }
    }
}
