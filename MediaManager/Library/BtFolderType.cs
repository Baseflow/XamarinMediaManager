namespace MediaManager.Library
{
    public enum BtFolderType
    {
        /// <summary>
        /// The type of folder that contains folders categorized by Album as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        Albums,

        /// <summary>
        /// The type of folder that contains folders categorized by artist as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        Artists,

        /// <summary>
        /// The type of folder that contains folders categorized by genre as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        Genres,

        /// <summary>
        /// The type of folder that is unknown or contains media elements of mixed types as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        Mixed,

        /// <summary>
        /// The type of folder that contains folders categorized by playlist as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        Playlists,

        /// <summary>
        /// The type of folder that contains media elements only as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        Titles,

        /// <summary>
        /// The type of folder that contains folders categorized by year as specified in the section 6.10.2.2 of the Bluetooth AVRCP 1.5.
        /// </summary>
        Years
    }
}
