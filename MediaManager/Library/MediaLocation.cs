namespace MediaManager.Library
{
    public enum MediaLocation
    {
        /// <summary>
        /// Used when MediaLoction type is not known
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Used for Media that is available online, like http, https, udp, etc
        /// </summary>
        Remote,
        /// <summary>
        /// Used for local File media. Typically used together with File and Directory api's
        /// </summary>
        FileSystem,
        /// <summary>
        /// Used for Media that is embedded into an Assembly. Build action of file needs to be set Embedded Resource
        /// </summary>
        Embedded,
        /// <summary>
        /// Used for media that is added to the native system. For example Assets and raw folder on Android, and Resources folder on iOS
        /// </summary>
        Resource,
        /// <summary>
        /// Used for media that is loaded in-memory.
        /// </summary>
        InMemory
    }
}
