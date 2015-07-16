using System;
using MediaManager.Plugin;
using System.Threading.Tasks;

namespace MediaManager.Sample.Core
{
    public class MediaPlayerManager
    {
        public async Task Play()
        {
            await CrossMediaManager.Current.Play("http://www.montemagno.com/sample.mp3");
        }
    }
}

