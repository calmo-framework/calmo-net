namespace Calmo.Data.ActiveDirectory
{
    public class ActiveDirectoryUserInfo
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] MemberOf { get; set; }
        public string[] Properties { get; set; }
    }
}
