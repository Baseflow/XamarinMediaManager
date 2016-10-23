using System;
namespace Plugin.MediaManager.Abstractions.EventArguments
{
    public class MediaFailedEventArgs : EventArgs
    {
        public MediaFailedEventArgs(string description, Exception exception)
        {
            Description = description;
            Exception = exception;
        }

        public Exception Exception { get; set; }
        public string Description { get; set; }
    }
}
