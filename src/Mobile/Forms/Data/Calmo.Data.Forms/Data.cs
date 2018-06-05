using System;
using Xamarin.Forms;

namespace Calmo.Data.Forms
{
    public static class Data
    {
        private static IDataInitializer _dataInitializer;

        public static ApiConfiguration Api()
        {
            return new ApiConfiguration(DataInitializer);
        }

        public static void Init()
        {
            if (DataInitializer == null)
                throw new Exception("Não foi possível acessar o DataInitializer. Habilite a opção 'Link SDK assemblies only' e tente novamente.");

            DataInitializer.InitConfig();
        }

        public static IDataInitializer DataInitializer
        {
            get
            {
                return _dataInitializer ?? (_dataInitializer = DependencyService.Get<IDataInitializer>());
            }
        }
    }
}