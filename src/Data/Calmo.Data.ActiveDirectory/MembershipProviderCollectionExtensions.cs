using System.Collections.Generic;
using System.Configuration.Provider;
using System.Reflection;
using System.Web.Security;

namespace Calmo.Data.ActiveDirectory
{
    public static class MembershipProviderCollectionExtensions
    {
        private static readonly FieldInfo _providerCollectionReadOnlyField;

        static MembershipProviderCollectionExtensions()
        {
            var t = typeof(ProviderCollection);
            _providerCollectionReadOnlyField = t.GetField("_ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static IEnumerable<MembershipProvider> ToList(this MembershipProviderCollection collection)
        {
            var providers = new List<MembershipProvider>();
            foreach (MembershipProvider provider in collection)
                providers.Add(provider);

            return providers;
        }

        public static void AddTo(this ProviderBase provider, ProviderCollection pc)
        {
            var prevValue = (bool)_providerCollectionReadOnlyField.GetValue(pc);
            if (prevValue)
                _providerCollectionReadOnlyField.SetValue(pc, false);

            pc.Add(provider);

            if (prevValue)
                _providerCollectionReadOnlyField.SetValue(pc, true);
        }
    }
}