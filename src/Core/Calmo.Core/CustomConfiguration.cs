namespace Calmo.Core
{
    public class CustomConfiguration
    {
        private CustomConfiguration() { }

        private static CustomConfiguration _settings;
        public static CustomConfiguration Settings
        {
            get 
            { 
                if (_settings == null)
                    _settings = new CustomConfiguration();

                return _settings;
            }
        }
    }
}
