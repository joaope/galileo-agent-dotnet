using System;
using System.Reflection;

namespace GalileoAgentNet.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetAssemblyVersion(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var typeInfo = type.GetTypeInfo();

            var infoVersion =
                    typeInfo
                    .Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;

            if (!infoVersion.HasValue())
            {
                infoVersion = typeInfo.Assembly.GetName().Version.ToString();
            }

            return infoVersion;
        }
    }
}
