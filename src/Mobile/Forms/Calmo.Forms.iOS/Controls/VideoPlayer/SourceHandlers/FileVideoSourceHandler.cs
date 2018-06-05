using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ResourceIT.Forms.Controls.VideoPlayer;
using ResourceIT.Forms.Controls.VideoPlayer.Interfaces;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer.SourceHandlers
{
    public sealed class FileVideoSourceHandler : IVideoSourceHandler
    {
        public Task<string> LoadVideoAsync(VideoSource source, CancellationToken cancellationToken = new CancellationToken())
        {
            string result = null;
            FileVideoSource source2 = source as FileVideoSource;
            if (!string.IsNullOrEmpty(source2?.File) && File.Exists(source2.File))
            {
                result = source2.File;
            }
            return Task.FromResult<string>(result);
        }
    }
}

