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