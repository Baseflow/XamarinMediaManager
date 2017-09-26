using System.Threading.Tasks;

namespace MediaManager.Abstractions
{
    public interface IMediaExtractor
    {
        /// <summary>
        /// Returns the same file with the extracted media information
        /// </summary>
        /// <param name="mediaFile">The media file</param>
        Task<IMediaItem> ExtractMediaInfo(IMediaItem mediaFile);
    }
}
