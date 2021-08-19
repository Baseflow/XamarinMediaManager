namespace MediaManager.Media
{
    public static class MimeTypeConverterExtensions
    {
        public static string ToMimeTypeString(this MimeType mimeType)
        {
            switch (mimeType)
            {
                case MimeType.AudioAiff:
                    return "audio/aiff";
                case MimeType.AudioBasic:
                    return "audio/basic";
                case MimeType.AudioMid:
                    return "audio/mid";
                case MimeType.AudioMidi:
                    return "audio/midi";
                case MimeType.AudioMp3:
                    return "audio/mp3";
                case MimeType.AudioMpeg:
                    return "audio/mpeg";
                case MimeType.AudioMpegUrl:
                    return "audio/mpegurl";
                case MimeType.AudioMpg:
                    return "audio/mpg";
                case MimeType.AudioWav:
                    return "audio/wav";
                case MimeType.AudioXAiff:
                    return "audio/x-aiff";
                case MimeType.AudioXMid:
                    return "audio/x-mid";
                case MimeType.AudioXMidi:
                    return "audio/x-midi";
                case MimeType.AudioXMp3:
                    return "audio/x-mp3";
                case MimeType.AudioXMpeg:
                    return "audio/x-mpeg";
                case MimeType.AudioXMpegUrl:
                    return "audio/x-mpegurl";
                case MimeType.AudioXMpg:
                    return "audio/x-mpg";
                case MimeType.AudioXMsWax:
                    return "audio/x-ms-wax";
                case MimeType.AudioXMsWma:
                    return "audio/x-ms-wma";
                case MimeType.AudioXWav:
                    return "audio/x-wav";
                case MimeType.Unknown:
                    return string.Empty;
                case MimeType.VideoAvi:
                    return "video/avi";
                case MimeType.VideoMpeg:
                    return "video/mpeg";
                case MimeType.VideoMpg:
                    return "video/mpg";
                case MimeType.VideoMsVideo:
                    return "video/msvideo";
                case MimeType.VideoQuicktime:
                    return "video/quicktime";
                case MimeType.VideoXMpeg:
                    return "video/x-mpeg";
                case MimeType.VideoXMpeg2A:
                    return "video/x-mpeg2a";
                case MimeType.VideoXMsAsf:
                    return "video/x-ms-asf";
                case MimeType.VideoXMsAsfPlugin:
                    return "video/x-ms-asf-plugin";
                case MimeType.VideoXMsWm:
                    return "video/x-ms-wm";
                case MimeType.VideoXMsWmv:
                    return "video/x-ms-wmv";
                case MimeType.VideoXMsWmx:
                    return "video/x-ms-wmx";
                case MimeType.VideoXMsWvx:
                    return "video/x-ms-wvx";
                case MimeType.VideoXMsVideo:
                    return "video/x-msvideo";
                default:
                    return string.Empty;
            }
        }

        public static MimeType ToMimeType(this string mimeTypeString)
        {
            switch (mimeTypeString)
            {
                case "audio/aiff":
                    return MimeType.AudioAiff;
                case "audio/basic":
                    return MimeType.AudioBasic;
                case "audio/mid":
                    return MimeType.AudioMid;
                case "audio/midi":
                    return MimeType.AudioMidi;
                case "audio/mp3":
                    return MimeType.AudioMp3;
                case "audio/mpeg":
                    return MimeType.AudioMpeg;
                case "audio/mpegurl":
                    return MimeType.AudioMpegUrl;
                case "audio/mpg":
                    return MimeType.AudioMpg;
                case "audio/wav":
                    return MimeType.AudioWav;
                case "audio/x-aiff":
                    return MimeType.AudioXAiff;
                case "audio/x-mid":
                    return MimeType.AudioXMid;
                case "audio/x-midi":
                    return MimeType.AudioXMidi;
                case "audio/x-mp3":
                    return MimeType.AudioXMp3;
                case "audio/x-mpeg":
                    return MimeType.AudioXMpeg;
                case "audio/x-mpegurl":
                    return MimeType.AudioXMpegUrl;
                case "audio/x-mpg":
                    return MimeType.AudioXMpg;
                case "audio/x-ms-wax":
                    return MimeType.AudioXMsWax;
                case "audio/x-ms-wma":
                    return MimeType.AudioXMsWma;
                case "audio/x-wav":
                    return MimeType.AudioXWav;
                case "video/avi":
                    return MimeType.VideoAvi;
                case "video/mpeg":
                    return MimeType.VideoMpeg;
                case "video/mpg":
                    return MimeType.VideoMpg;
                case "video/msvideo":
                    return MimeType.VideoMsVideo;
                case "video/quicktime":
                    return MimeType.VideoQuicktime;
                case "video/x-mpeg":
                    return MimeType.VideoXMpeg;
                case "video/x-mpeg2a":
                    return MimeType.VideoXMpeg2A;
                case "video/x-ms-asf":
                    return MimeType.VideoXMsAsf;
                case "video/x-ms-asf-plugin":
                    return MimeType.VideoXMsAsfPlugin;
                case "video/x-ms-wm":
                    return MimeType.VideoXMsWm;
                case "video/x-ms-wmv":
                    return MimeType.VideoXMsWmv;
                case "video/x-ms-wmx":
                    return MimeType.VideoXMsWmx;
                case "video/x-ms-wvx":
                    return MimeType.VideoXMsWvx;
                case "video/x-msvideo":
                    return MimeType.VideoXMsVideo;
                default:
                    return MimeType.Unknown;
            }
        }
    }
}
