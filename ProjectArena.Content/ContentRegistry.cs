using System;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content
{
    public static class ContentRegistry
    {
        private static bool IsNativeInterfaceRealization(Type type)
        {
            if (type != null)
            {
                var configuration = type.GetInterfaces().Any(x => x == typeof(INative));
                if (configuration)
                {
                    return true;
                }
            }

            if (type == null)
            {
                return false;
            }

            return IsNativeInterfaceRealization(type.BaseType);
        }

        public static void FillNatives(this INativeManager nativeManager)
        {
            var nativeTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => IsNativeInterfaceRealization(type))
                .ToList();
            foreach (var native in nativeTypes)
            {
                var instance = Activator.CreateInstance(native);
                native.GetMethod("Fill").Invoke(instance, new[] { nativeManager });
            }
        }
    }
}