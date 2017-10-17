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
            var mediaItem = new MediaItem
            {
                Type = MediaItemType.Audio,
                //Availability = ResourceAvailability.Remote,
                Url = "https://audioboom.com/posts/5766044-follow-up-305.mp3",
                Id = new Guid(), Metadata = new MediaItemMetadata() { Title = "Test"}, MetadataExtracted = false
            };
            //await CrossMediaManager.Current.Play(MediaItem);
            await CrossMediaManager.Current.CurrentPlaybackManager.Play(mediaItem);
        }

        private async void PlaylistButton_OnClicked(object sender, EventArgs e)
        {
            var list = new List<MediaItem>
            {
                new MediaItem
                {
                    Url = "https://audioboom.com/posts/5766044-follow-up-305.mp3?source=rss&amp;stitched=1",
                    Type = MediaItemType.Audio,
                    Metadata = new MediaItemMetadata
                    {
                        Title = "Test1"
                    }
                },
                new MediaItem
                {
                    Url = "https://media.acast.com/mydadwroteaporno/s3e1-london-thursday15.55localtime/media.mp3",
                    Type = MediaItemType.Audio,
                    Metadata = new MediaItemMetadata
                    {
                        Title = "Test2"
                    }
                },
                new MediaItem
                {
                    Url =
                        "https://audioboom.com/posts/5770261-ep-306-a-theory-of-evolution.mp3?source=rss&amp;stitched=1",
                    Type = MediaItemType.Audio,
                    Metadata = new MediaItemMetadata
                    {
                        Title = "Test3"
                    }
                },
                new MediaItem
                {
                    Url = "https://audioboom.com/posts/5723344-ep-304-the-4th-dimension.mp3?source=rss&amp;stitched=1",
                    Type = MediaItemType.Audio,
                    Metadata = new MediaItemMetadata
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