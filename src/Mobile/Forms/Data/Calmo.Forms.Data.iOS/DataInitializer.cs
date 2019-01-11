using Calmo.Data.Forms.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataInitializer_iOS))]
namespace Calmo.Data.Forms.iOS
{
    using System;
    using PCLAppConfig;

    public class DataInitializer_iOS : IDataInitializer
    {
        public DataInitializer_iOS()
        {
            
        }

        public void InitConfig()
        {
            ConfigurationManager.Initialise(PCLAppConfig.FileSystemStream.PortableStream.Current);
        }
    }
}