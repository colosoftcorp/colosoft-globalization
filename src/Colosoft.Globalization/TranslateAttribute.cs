using System;

namespace Colosoft
{
    [AttributeUsage(AttributeTargets.All)]
    public class TranslateAttribute : Attribute
    {
        public Type ProviderType { get; }

        public TranslateAttribute(Type providerType)
        {
            this.ProviderType = providerType;
        }
    }
}
