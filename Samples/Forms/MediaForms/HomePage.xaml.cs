using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CrossMediaManager.Current.StatusChanged += CurrentOnStatusChanged;
        }

        protected override void OnDisappearing()
        {
            CrossMediaManager.Current.StatusChanged -= CurrentOnStatusChanged;
            base.OnDisappearing();
        }

        private void CurrentOnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Debug.WriteLine($"MediaManager Status: {e.Status}");

            Device.BeginInvokeOnMainThread(() =>
            {
                PlayerStatus.Text = e.Status.ToString();
                IsBufferingIndicator.IsVisible = e.Status == MediaPlayerStatus.Buffering || e.Status == MediaPlayerStatus.Loading;
            });
        }

        private async void StopButton_OnClicked(object sender, EventArgs e)
        {
            await CrossMediaManager.Current.Stop();
        }

        private async void PlayAudio_OnClicked(object sender, EventArgs e)
        {
            var mediaFile = new MediaFile
            {
                Type = MediaFileType.Audio,
                Availability = ResourceAvailability.Remote,
                Url = "https://audioboom.com/posts/5766044-follow-up-305.mp3",
                ExtractMetadata = true
            };
            await CrossMediaManager.Current.Play(mediaFile);
        }

        private async void PlayAudioMyTrack_OnClicked(object sender, EventArgs e)
        {
            var mediaFile = new MediaFile
            {
                Type = MediaFileType.Audio,
                Availability = ResourceAvailability.Remote,
                Url = "https://audioboom.com/posts/5766044-follow-up-305.mp3",
                Metadata = new MediaFileMetadata() { Title = "My Title", Artist = "My Artist", Album = "My Album" },
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
                        Title = "Test2",
                        DisplayIconUri = "https://d15mj6e6qmt1na.cloudfront.net/i/8457198.jpg"
                    }
                },
                new MediaFile
                {
                    Url =
                        "https://audioboom.com/posts/5770261-ep-306-a-theory-of-evolution.mp3?source=rss&amp;stitched=1",
                    Type = MediaFileType.Audio,
                    Metadata = new MediaFileMetadata
                    {
                        Title = "Test3",
                        DisplayIconUri = "https://d15mj6e6qmt1na.cloudfront.net/i/30739475.jpg"
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

        private void AddToPlaylistClicked(object sender, EventArgs e)
        {
            CrossMediaManager.Current.MediaQueue.Add(new MediaFile
            {
                Url = "https://audioboom.com/posts/5723344-ep-304-the-4th-dimension.mp3?source=rss&amp;stitched=1",
                Type = MediaFileType.Audio,
                Metadata = new MediaFileMetadata
                {
                    Title = "Test4",
                    DisplayIconUri = "https://d15mj6e6qmt1na.cloudfront.net/i/30739475.jpg"
                }
            });
        }

        private void RemoveLastFromPlaylistClicked(object sender, EventArgs e)
        {
            if (!(CrossMediaManager.Current.MediaQueue?.Any() ?? false))
            {
                return;
            }

            CrossMediaManager.Current.MediaQueue.RemoveAt(CrossMediaManager.Current.MediaQueue.Count - 1);
        }
    }
}