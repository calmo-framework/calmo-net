using ResourceIT.Forms.Controls.VideoPlayer;
using ResourceIT.Forms.Controls.VideoPlayer.Interfaces;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer.SourceHandlers
{
    public static class VideoSourceHandler
    {
        public static IVideoSourceHandler Create(VideoSource source)
        {
            if (source is FileVideoSource)
            {
                return new FileVideoSourceHandler();
            }
            if (source is StreamVideoSource)
            {
                return new StreamVideoSourceHandler();
            }
            return new UriVideoSourceHandler();
        }
    }
}

