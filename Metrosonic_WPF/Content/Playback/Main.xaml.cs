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


using System.Windows.Input;

namespace MetroSonic.Pages.Playback
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using FirstFloor.ModernUI.Presentation;
    using FirstFloor.ModernUI.Windows.Controls;
    using Content;
    using MediaControl;

    /// <summary>
    /// Interaction logic for PlaybackMain.xaml.
    /// </summary>
    public partial class Main : UserControl
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (LibraryManagement.CurrentPlaylist.Count == 0)
            {
                Constants.WindowMain.ContentSource = new Uri("/Content/Home/AlbumPage.xaml?type=all", UriKind.Relative);
                new ModernDialog()
                {
                    Content = LanguageOutput.Warnings.PlaylistEmpty,
                    Title = LanguageOutput.Warnings.WarningTitle
                }.ShowDialog();
                return;
            }

           SetSongData();

            foreach (var item in LibraryManagement.CurrentPlaylist)
            {
                TimeSpan itemLength = TimeSpan.FromSeconds(double.Parse(item.TrackDuration));
                DataGrid.Items.Add(new DataItem
                {
                    Track = item.TrackName,
                    Length = itemLength.ToString("mm':'ss"),
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
            Constants.WindowMain.Playback.StartPlayback();
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
            Constants.WindowMain.Playback.StopPlayback();
            LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Backward);
            SetSongData();
            Constants.WindowMain.Playback.StartPlayback();
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
            Constants.WindowMain.Playback.StopPlayback();
            LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Forward);
            SetSongData();
            Constants.WindowMain.Playback.StartPlayback();
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
            Repeat.Foreground = LibraryManagement.Repeat ? new SolidColorBrush(AppearanceManager.Current.AccentColor) : Play.Foreground;
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

        public void SetSongData()
        {
            var currentItem = LibraryManagement.CurrentPlaylist[LibraryManagement.CurrentIndex];
            LibraryManagement.CoverDownload(Cover, currentItem.CoverID, Constants.CoverType.Album);
            NameOfSong.Text = currentItem.TrackName;
            NameOfAlbum.BBCode = string.Format("[url=/Content/Library/AlbumView.xaml?id={0}]{1}[/url]", currentItem.CoverID, currentItem.AlbumName);
            NameOfArtist.BBCode = string.Format("[url=/Content/Library/DetailView.xaml?id={0}]{1}[/url]", currentItem.ParentID, currentItem.Artist);
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