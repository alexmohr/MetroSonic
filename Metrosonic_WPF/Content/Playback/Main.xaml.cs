// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.xaml.cs" company="Alexander Mohr">
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, 
//   or (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
// </copyright>
// <summary>
//   Interaction logic for PlaybackMain.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Windows.Controls.Primitives;

namespace MetroSonic.Pages.Playback
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;
    using FirstFloor.ModernUI.Presentation;
    using FirstFloor.ModernUI.Windows.Controls;
    using Content;
    using MediaControl;

    /// <summary>
    /// Interaction logic for PlaybackMain.xaml.
    /// </summary>
    public partial class Main : UserControl
    {
        private bool _SliderDisableUpdate;
        private double _sliderLastValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            if (LibraryManagement.CurrentPlaylist.Count == 0)
            {
                Constants.WindowMain.ContentSource = new Uri("/Content/Home/AlbumPage.xaml?type=all", UriKind.Relative);
                new ModernDialog()
                {
                    Content = LanguageOutput.Warnings.PlaylistEmpty,
                    Title = LanguageOutput.Warnings.WarningTitle
                }.ShowDialog();
            }

            InitializeComponent();
            LibraryManagement.Playback.PlaybackStarted += PlaybackOnPlaybackStarted;
            LibraryManagement.Playback.PlaybackStatusUpdate += PlaybackUpdateEvent;
        }

        private void PlaybackOnPlaybackStarted(object sender, PlaybackStartEventArgs playbackStartEventArgs)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart) SetSongData);
        }

        private void PlaybackUpdateEvent(object sender, PlaybackUpdateEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                (ThreadStart)delegate
                {
                    if (!_SliderDisableUpdate)
                    {
                        Slider.Maximum = LibraryManagement.Playback.StreamLength;
                        Slider.Value = e.PassedSecounds;
                        _sliderLastValue = Slider.Value;
                    }

                    TimePlaying.Content = LibraryManagement.CurrentPlaylist[LibraryManagement.CurrentIndex]
                        .TrackDuration.Subtract(
                            TimeSpan.FromSeconds(e.PassedSecounds)).ToString("mm':'ss");
                });
        }   

        /// <summary>
        /// Updates the datagrid and button colors.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Repeat.Foreground = LibraryManagement.Shuffle ? new SolidColorBrush(AppearanceManager.Current.AccentColor) : Play.Foreground;
            Shuffle.Foreground = LibraryManagement.Shuffle ? new SolidColorBrush(AppearanceManager.Current.AccentColor) : Play.Foreground;

           SetSongData();

            foreach (var item in LibraryManagement.CurrentPlaylist)
            {
                DataGrid.Items.Add(new DataItem
                {
                    Track = item.TrackName,
                    Length = item.TrackDuration.ToString("mm':'ss"),
                    //Rating = null,
                    Album = item.AlbumName,
                    Artist = item.Artist
                });
            }
        }

        /// <summary>
        /// Clickevent for the playbutton.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            LibraryManagement.Playback.TogglePlayBack();

            if (LibraryManagement.Playback.PlaybackState == PlaybackControl.StreamingPlaybackState.Buffering ||
                LibraryManagement.Playback.PlaybackState == PlaybackControl.StreamingPlaybackState.Playing)
            {
                Play.IconData =
                    Geometry.Parse(
                        "M 26.9167,23.75L 33.25,23.75L 33.25,52.25L 26.9167,52.25L 26.9167,23.75 Z M 42.75,23.75L 49.0833,23.75L 49.0833,52.25L 42.75,52.25L 42.75,23.75 Z");
                Play.IconHeight = 25;
            }
            else
            {
                Play.IconData =
                    Geometry.Parse(
                        "F1 M 30.0833,22.1667L 50.6665,37.6043L 50.6665,38.7918L 30.0833,53.8333L 30.0833,22.1667 Z");
                Play.IconHeight = 25;
            }
        }

        /// <summary>
        /// Clickevent for the backwardbutton.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            LibraryManagement.Playback.StopPlayback();
            LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Backward);
            SetSongData();
            LibraryManagement.Playback.StartPlayback();
        }

        /// <summary>
        /// Clickevent for the forwardbutton.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            LibraryManagement.Playback.StopPlayback();
            LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Forward);
            SetSongData();
            LibraryManagement.Playback.StartPlayback();
        }

        /// <summary>
        /// Clickevent for the repeatbutton.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            LibraryManagement.ToggleRepeat();
            Repeat.Foreground = LibraryManagement.Shuffle ? new SolidColorBrush(AppearanceManager.Current.AccentColor) : Play.Foreground;
        }

        /// <summary>
        /// Clickevent for the shufflebutton.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            LibraryManagement.ToggleShuffle();
            Shuffle.Foreground = LibraryManagement.Shuffle ? new SolidColorBrush(AppearanceManager.Current.AccentColor) : Play.Foreground;
        }

        /// <summary>
        /// Sets the data for the current song.
        /// </summary>
        public void SetSongData()
        {
            var currentItem = LibraryManagement.CurrentPlaylist[LibraryManagement.CurrentIndex];
            LibraryManagement.CoverDownload(Cover, currentItem.CoverID, Constants.CoverType.Album);
            NameOfSong.Text = currentItem.TrackName;
            NameOfAlbum.BBCode = string.Format("[url=/Content/Library/AlbumView.xaml?id={0}]{1}[/url]", currentItem.CoverID, currentItem.AlbumName);
            NameOfArtist.BBCode = string.Format("[url=/Content/Library/DetailView.xaml?id={0}]{1}[/url]", currentItem.ParentID, currentItem.Artist);

            TimeLeft.Content = currentItem.TrackDuration.ToString("mm':'ss");
            TimePlaying.Content = TimeSpan.FromSeconds(LibraryManagement.Playback.StreamLength).ToString("mm':'ss");
            Slider.Maximum = LibraryManagement.Playback.StreamLength;
            Slider.Value = 0; 
        }

        private void SliderInputStart(object sender, DragStartedEventArgs e)
        {
            e.Handled = true;
            _SliderDisableUpdate = true;
        }

        private void SliderInputEnd(object sender, DragCompletedEventArgs e)
        {
            _SliderDisableUpdate = false; 
        }
    }

    class DataItem
    {
        public string Track { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Length { get; set; }
        public string Rating { get; set; }
    }
}