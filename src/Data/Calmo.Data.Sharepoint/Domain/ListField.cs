namespace Calmo.Data.Sharepoint.Domain
{
    public class ListField
    {
        public string DisplayName { get; set; }
        public string InternalName { get; set; }
        public ListAddFieldOptions AddFieldOptions { get; set; }
        public bool AddToDefaultView { get; set; }
        public bool? ReadOnly { get; set; }
        public ListFieldType FieldType {get; set; }
    }
}