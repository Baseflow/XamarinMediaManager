using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MediaForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            this.volumeLabel.Text = "Volume (0-" + CrossMediaManager.Current.VolumeManager.MaxVolume + ")";
            //Initialize Volume settings to match interface
            int.TryParse(this.volumeEntry.Text, out var vol);
            CrossMediaManager.Current.VolumeManager.CurrentVolume = vol;
            CrossMediaManager.Current.VolumeManager.Muted = false;
        }

        private void MainBtn_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MediaFormsPage());
        }

        private async void PlayAudio_OnClicked(object sender, EventArgs e)
        {
            var mediaFile = new MediaFile
            {
                Type = MediaFileType.Audio,
                Availability = ResourceAvailability.Remote,
                Url = "https://audioboom.com/posts/5766044-follow-up-305.mp3",
                Metadata = new MediaFileMetadata() {Title = "My Title", Artist = "My Artist", Album = "My Album"},
                ExtractMetadata = false
            };
            await CrossMediaManager.Current.Play(mediaFile);
        }

        private async void PlaylistButton_OnClicked(object sender, EventArgs e)
        {
            var list = new List<MediaFile>
            {
                new MediaFile
                {
                    Url = "https://audioboom.com/posts/5766044-follow-up-305.mp3?source=rss&amp;stitched=1",
                    Type = MediaFileType.Audio,
                    Metadata = new MediaFileMetadata
                    {
                        Title = "Test1"
                    }
                },
                new MediaFile
                {
                    Url = "https://media.acast.com/mydadwroteaporno/s3e1-london-thursday15.55localtime/media.mp3",
                    Type = MediaFileType.Audio,
                    Metadata = new MediaFileMetadata
                    {
                        Title = "Test2"
                    }
                },
                new MediaFile
                {
                    Url =
                        "https://audioboom.com/posts/5770261-ep-306-a-theory-of-evolution.mp3?source=rss&amp;stitched=1",
                    Type = MediaFileType.Audio,
                    Metadata = new MediaFileMetadata
                    {
                        Title = "Test3"
                    }
                },
                new MediaFile
                {
                    Url = "https://audioboom.com/posts/5723344-ep-304-the-4th-dimension.mp3?source=rss&amp;stitched=1",
                    Type = MediaFileType.Audio,
                    Metadata = new MediaFileMetadata
                    {
                        Title = "Test4"
                    }
                }
            };
            // Follow-Up 305
            // Ep. 306: A Theory of Evolution
            // Ep. 304: The 4th Dimension
            await CrossMediaManager.Current.Play(list);
        }

        private void SetVolumeBtn_OnClicked(object sender, EventArgs e)
        {
            int.TryParse(this.volumeEntry.Text, out var vol);
            CrossMediaManager.Current.VolumeManager.CurrentVolume = vol;
        }

        private void MutedBtn_OnClicked(object sender, EventArgs e)
        {
            if (CrossMediaManager.Current.VolumeManager.Muted)
            {
                CrossMediaManager.Current.VolumeManager.Muted = false;
                mutedBtn.Text = "Mute";
            }
            else
            {
                CrossMediaManager.Current.VolumeManager.Muted = true;
                mutedBtn.Text = "Unmute";
            }
        }
    }
}