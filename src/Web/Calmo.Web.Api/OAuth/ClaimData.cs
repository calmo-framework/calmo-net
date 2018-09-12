namespace Calmo.Web.Api.OAuth
{
    public struct ClaimData
    {
        public ClaimData(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; }
        public string Value { get; }
    }
}
