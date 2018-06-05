namespace Calmo.Data.ActiveDirectory
{
    public class ActiveDirectoryUserInfo
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string[] MemberOf { get; set; }
    }
}
