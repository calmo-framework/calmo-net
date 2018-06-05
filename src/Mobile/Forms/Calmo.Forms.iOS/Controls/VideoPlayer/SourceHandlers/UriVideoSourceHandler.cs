using System.Threading;
using System.Threading.Tasks;
using ResourceIT.Forms.Controls.VideoPlayer;
using ResourceIT.Forms.Controls.VideoPlayer.Interfaces;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer.SourceHandlers
{
    public sealed class UriVideoSourceHandler : IVideoSourceHandler
    {
        public Task<string> LoadVideoAsync(VideoSource source, CancellationToken cancellationToken = new CancellationToken())
        {
            string result = null;
            UriVideoSource source2 = source as UriVideoSource;
            if (source2?.Uri != null)
            {
                result = source2.Uri.AbsoluteUri;
            }
            return Task.FromResult<string>(result);
        }
    }
}

