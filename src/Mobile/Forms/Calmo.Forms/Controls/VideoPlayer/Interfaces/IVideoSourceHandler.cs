namespace ResourceIT.Forms.Controls.VideoPlayer.Interfaces
{
    using Controls.VideoPlayer;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IVideoSourceHandler
    {
        Task<string> LoadVideoAsync(VideoSource source, CancellationToken cancellationToken);
    }
}

