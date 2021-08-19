using System;
using System.IO;
using System.Text;
using AVFoundation;
using Foundation;
using UniformTypeIdentifiers;

namespace MediaManager.Platforms.Apple.Player
{
    public class StreamResourceLoaderDelegate : AVAssetResourceLoaderDelegate {
        private readonly Stream _stream;
        private readonly UTType _type;

        public StreamResourceLoaderDelegate(Stream stream, UTType type)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public override bool ShouldWaitForLoadingOfRequestedResource(AVAssetResourceLoader resourceLoader,
            AVAssetResourceLoadingRequest loadingRequest)
        {
            if (loadingRequest.ContentInformationRequest != null)
            {
                loadingRequest.ContentInformationRequest.ContentLength = _stream.Length;
                loadingRequest.ContentInformationRequest.ContentType = _type.Identifier;
                loadingRequest.ContentInformationRequest.ByteRangeAccessSupported = true;
            }

            var offset = Convert.ToInt32(loadingRequest.DataRequest.CurrentOffset);
            if (_stream.CanSeek)
            {
                _stream.Seek(offset, SeekOrigin.Begin);
            }

            var remainingBytes = (int) _stream.Length - offset;
            var bytesToRead = Math.Min(remainingBytes, Convert.ToInt32(loadingRequest.DataRequest.RequestedLength));

            using (var reader = new BinaryReader(_stream, Encoding.UTF8, true))
            {
                var bytes = reader.ReadBytes(bytesToRead);
                var data = NSData.FromArray(bytes);
                loadingRequest.DataRequest.Respond(data);
            }

            loadingRequest.FinishLoading();
            return true;
        }
    }
}
