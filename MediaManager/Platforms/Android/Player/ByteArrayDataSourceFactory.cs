using Com.Google.Android.Exoplayer2.Upstream;

namespace MediaManager.Platforms.Android.Player
{
    public class ByteArrayDataSourceFactory : Java.Lang.Object, IDataSourceFactory
    {
        private readonly byte[] _data;

        public ByteArrayDataSourceFactory(byte[] data)
        {
            _data = data;
        }

        public IDataSource CreateDataSource()
        {
            return new ByteArrayDataSource(_data);
        }
    }
}
