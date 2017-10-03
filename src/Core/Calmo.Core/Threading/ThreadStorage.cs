using System.Threading;

namespace Calmo.Core.Threading
{
    public static class ThreadStorage
    {
        public static void SetData(string key, object value)
        {
            var dataSlot = Thread.GetNamedDataSlot(key);
            Thread.SetData(dataSlot, value);
        }

        public static T GetData<T>(string key)
        {
            var dataSlot = Thread.GetNamedDataSlot(key);
            var data = Thread.GetData(dataSlot);
            if (data == null)
                return default(T);

            return (T)data;
        }

        public static void ClearData(string key)
        {
            Thread.FreeNamedDataSlot(key);
        }
    }
}
