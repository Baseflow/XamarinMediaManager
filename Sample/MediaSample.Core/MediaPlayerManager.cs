using System;
using Plugin.MediaManager;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;

namespace MediaManager.Sample.Core
{
    public class MediaPlayerManager
    {
        public async Task Play()
        {
            //await CrossMediaManager.Current.Play(new MyMediaFile()
            //                                            { Type = MediaFileType.AudioFile,
            //                                              Url = "/storage/emulated/0/Download/test.mp3"
            //                                             });


            await CrossMediaManager.Current.Play("http://www.montemagno.com/sample.mp3");
        }
    }

    class MyMediaFile : IMediaFile
    {
        public MediaFileType Type { get; set; }
        public string Url { get; set; }
    }
}

