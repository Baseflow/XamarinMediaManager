using System;
using System.Collections.Generic;
using System.Text;

namespace ElementPlayer.Core.Assets
{
    public static class MediaPlaybackAssets
    {
        /// <summary>
        /// A list of remote mp3 files
        /// </summary>
        public static IList<string> Mp3UrlList => new[]{
        "https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3",
        "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/CelineDion-IfICould.mp3",
        "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/Daughtry-Homeacoustic.mp3",
        "https://storage.googleapis.com/uamp/The_Kyoto_Connection_-_Wake_Up/01_-_Intro_-_The_Way_Of_Waking_Up_feat_Alan_Watts.mp3",
        "https://aphid.fireside.fm/d/1437767933/02d84890-e58d-43eb-ab4c-26bcc8524289/d9b38b7f-5ede-4ca7-a5d6-a18d5605aba1.mp3"
        };

        public static string RandomMp3Url => Mp3UrlList.Random();
    }

    public static class IListExtension
    {
        public static string Random(this IList<string> self)
        {
            return self[new Random().Next(0, self.Count - 1)];
        }
    }
}
