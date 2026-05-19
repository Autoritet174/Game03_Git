#pragma warning disable IDE0130 // Пространство имен (namespace) не соответствует структуре папок.

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute { }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        internal CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }
        public string FeatureName { get; }
        public bool IsOptional { get; init; }
    }

    internal static class IsExternalInit { }
}
