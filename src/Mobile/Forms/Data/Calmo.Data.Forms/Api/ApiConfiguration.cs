using System;
using Newtonsoft.Json;

namespace Calmo.Data.Forms
{
    public class ApiConfiguration
    {
        private readonly IDataInitializer DataInitializer;

        internal ApiConfiguration(IDataInitializer dataInitializer)
        {
            this.DataInitializer = dataInitializer;
        }

        internal static JsonSerializerSettings SerializerSettings { get; private set; }
        internal static Action<ApiException> OnErrorAction { get; private set; }

        public ApiConfiguration JsonSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            ApiConfiguration.SerializerSettings = serializerSettings;

            return this;
        }

        public ApiConfiguration OnError(Action<ApiException> onErrorAction)
        {
            ApiConfiguration.OnErrorAction = onErrorAction;
            return this;
        }

        public void Init()
        {
            //this.DataInitializer.InitConfig();
        }
    }
}
