using Calmo.Data.Forms.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataInitializer_Droid))]
namespace Calmo.Data.Forms.Droid
{
    using System;
    using PCLAppConfig;

    public class DataInitializer_Droid : IDataInitializer
    {
        public DataInitializer_Droid()
        {
            
        }

        public void InitConfig()
        {
            ConfigurationManager.Initialise(PCLAppConfig.FileSystemStream.PortableStream.Current);
        }
    }
}