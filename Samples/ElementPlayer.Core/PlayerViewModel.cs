using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaManager;

namespace ElementPlayer.Core
{
    public class PlayerViewModel
    {
        private IMediaManager _mediaManager;

        public PlayerViewModel()
        {
            _mediaManager = CrossMediaManager.Current;
        }

        public async Task Play ()
        {
            var item = await _mediaManager.MediaExtractor.CreateMediaItem("https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3");
            await _mediaManager.Play(item);
        }
    }
}
