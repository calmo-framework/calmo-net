namespace System
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DataKeyAttribute : Attribute
    {
        public string DataKey { get; set; }

        public DataKeyAttribute(string dataKey)
        {
            DataKey = dataKey;
        }
    }
}
