using System;
using PCLAppConfig;
using Xamarin.Forms;

namespace Calmo.Data.Forms
{
    public static class Data
    {
        private static IDataInitializer _dataInitializer;
        private static bool _isInitialized;

        public static ApiConfiguration Api()
        {
            return new ApiConfiguration(DataInitializer);
        }

        public static void Init()
        {
            /*if (DataInitializer == null)
                throw new Exception("Não foi possível acessar o DataInitializer. Habilite a opção 'Link SDK assemblies only' e tente novamente.");

            DataInitializer.InitConfig();*/

            if (_isInitialized)
                return;

            ConfigurationManager.Initialise(PCLAppConfig.FileSystemStream.PortableStream.Current);
            _isInitialized = true;
        }

        public static IDataInitializer DataInitializer
        {
            get
            {
                if (_dataInitializer == null)
                    _dataInitializer = DependencyService.Get<IDataInitializer>();

                return _dataInitializer;
            }
        }
    }
}