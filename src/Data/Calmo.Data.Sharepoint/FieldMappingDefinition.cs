namespace Calmo.Data.Sharepoint
{
    public class FieldMappingDefinition
    {
        public string Name { get; private set; }
        public bool IsUserField { get; private set; }
        public bool IsLookupField { get; private set; }
        public string LookupListName { get; private set; }
        public string LookupFieldKey { get; private set; }

        private FieldMappingDefinition()
        {

        }

        public static FieldMappingDefinition GetField(string name)
        {
            return new FieldMappingDefinition { Name = name };
        }

        public static FieldMappingDefinition GetUserField(string name)
        {
            return new FieldMappingDefinition { Name = name, IsUserField = true };
        }

        public static FieldMappingDefinition GetLookupField(string name, string list, string fieldKey)
        {
            return new FieldMappingDefinition { Name = name, IsLookupField = true, LookupListName = list, LookupFieldKey = fieldKey };
        }
    }
}