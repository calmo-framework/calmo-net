namespace ResourceIT.Forms.Controls.VideoPlayer.Interfaces
{
    public interface IPlatformFeatures
    {
        void Exit();
        string HashSha1(string value);

        string PackageName { get; }
    }
}

