namespace ResourceIT.Forms.Controls.VideoPlayer.ExtensionMethods
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        public static bool ContainsManifestResource(this Assembly assembly, string resourceName) =>
            (assembly != null) && assembly.GetManifestResourceNames().Any(x => x.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase));

        public static byte[] GetEmbeddedResourceBytes(this Assembly assembly, string resourceFileName)
        {
            var embeddedResourceStream = assembly.GetEmbeddedResourceStream(resourceFileName);
            using (var stream2 = new MemoryStream())
            {
                embeddedResourceStream.CopyTo(stream2);
                return stream2.ToArray();
            }
        }

        public static Stream GetEmbeddedResourceStream(this Assembly assembly, string resourceFileName)
        {
            var strArray = (from x in assembly.GetManifestResourceNames() select x).ToArray();
            if (!strArray.Any())
            {
                object[] objArray1 = { resourceFileName };
                throw new Exception($"Resource ending with {objArray1} not found.");
            }
            if (strArray.Count() > 1)
            {
                object[] objArray2 = { resourceFileName, Environment.NewLine, string.Join(Environment.NewLine, strArray) };
                throw new Exception(string.Format("Multiple resources ending with {0} found: {1}{2}", objArray2));
            }
            return assembly.GetManifestResourceStream(strArray.Single());
        }

        public static string GetEmbeddedResourceString(this Assembly assembly, string resourceFileName)
        {
            using (var reader = new StreamReader(assembly.GetEmbeddedResourceStream(resourceFileName)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

