// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LibraryManagement.cs" company="Alexander Mohr">
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
//   The library management.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetroSonic.MediaControl
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using FirstFloor.ModernUI.Windows.Controls;
    using Content;
    using Properties;

    /// <summary>
    /// The library management.
    /// </summary>
    public static class LibraryManagement
    {
        /// <summary>
        /// The skip directions.
        /// </summary>
        public enum SkipDirection
        {
            /// <summary>
            /// The next Track will be played.
            /// </summary>
            Forward, 

            /// <summary>
            /// The last Track will be played.
            /// </summary>
            Backward
        }

        /// <summary>
        /// The view type.
        /// </summary>
        public enum ViewType
        {
            /// <summary>
            /// Random Albums from the Server.
            /// </summary>
            Random, 

            /// <summary>
            /// The newest Albums from the Server.
            /// </summary>
            New, 

            /// <summary>
            /// The now Playing Album.
            /// </summary>
            Now, 

            /// <summary>
            /// The most played Albums.
            /// </summary>
            Most, 

            /// <summary>
            /// All Albums by Name.
            /// </summary>
            All, 

            /// <summary>
            /// Get Details of ID.
            /// </summary>
            ID
        }

        /// <summary>
        /// The current playlist.
        /// </summary>
        public static readonly List<MediaItem> CurrentPlaylist = new List<MediaItem>();

        /// <summary>
        /// The access to the Subsonic Server.
        /// </summary>
        private static readonly SubsonicAccess Access;

        /// <summary>
        /// Initializes static members of the <see cref="LibraryManagement"/> class.
        /// </summary>
        static LibraryManagement()
        {
            string authentication = "&u=" + Settings.Default.username +
                                    "&p=" + Settings.Default.password +
                                    "&v=1.9.0" +
                                    "&c=MetroSonic";
            Access = new SubsonicAccess(authentication, Settings.Default.server);

            Folder = Access.Folder;
        }

        /// <summary>
        /// Inidicates if the playlist is shuffeld.
        /// </summary>
        /// <value>
        /// The shuffle.
        /// </value>
        public static bool Shuffle { get; private set; }

        /// <summary>
        /// Indicates if songs are repeated.
        /// </summary>
        /// <value>
        /// The repeat.
        /// </value>
        public static bool Repeat { get; private set; }

        /// <summary>
        /// The Current index in the playlist.
        /// </summary>
        /// <value>
        /// The current index.
        /// </value>
        public static int CurrentIndex { get; private set; }

        /// <summary>
        /// The folder data.
        /// </summary>
        public static string[,] Folder { get; private set; }

        private static MediaItem[][] _allArtists;
        public static MediaItem[][] AllArtists
        {
            get
            {
               if (_allArtists == null)
                    SetAllArtists();
                return _allArtists;
            }
        }
       

        /// <summary>
        /// Queries the login status.
        /// </summary>
        /// <returns>
        /// Return the login status from the server.
        /// </returns>
        public static bool LoginSuccessfull()
        {
            return Access.LoginSuccesfull();
        }

        /// <summary>
        /// Downloads the Artists from given ID.
        /// </summary>
        /// <param name="folderID">
        /// The folderID of the Artist.
        /// </param>
        /// <returns>
        /// String array with all artists in the folder.
        /// </returns>
        public static MediaItem[] GetArtists(string folderID)
        {
            return Access.GetArtists(folderID);
        }

        private static void SetAllArtists()
        {
            if (Folder == null)
                GetFolder();

            if (Folder == null) return;

            int folderCount = Folder.GetLength(0); 

            _allArtists = new MediaItem[folderCount][];

            for (int i = 0; i < folderCount; i++)
            {
                _allArtists[i] = GetArtists(Folder[i, 1]);
            }
        }

        /// <summary>
        /// Downloads a view from the server.
        /// </summary>
        /// <param name="id">
        /// The ID to download.
        /// </param>
        /// <param name="type">
        /// The viewtype.
        /// </param>
        /// <returns>
        /// Playlistem array with all data.
        /// </returns>
        public static MediaItem[] GetView(string id, ViewType type)
        {
            return Access.GetView(id, type);
        }

        /// <summary>
        /// Toggles the Shuffle Playback.
        /// </summary>
        public static void ToggleShuffle()
        {
            Shuffle = !Shuffle;
        }

        /// <summary>
        /// The toggle random.
        /// </summary>
        public static void ToggleRepeat()
        {
            Repeat = !Repeat;
        }

        /// <summary>
        /// Gets the Subsonic URL of the Current Track.
        /// </summary>
        /// <returns>
        /// The Subsonic URL.
        /// </returns>
        public static string PlaylistGetCurrentTrackURL()
        {
            if (CurrentPlaylist.Count != 0)
                return CurrentIndex < CurrentPlaylist.Count ? CurrentPlaylist[CurrentIndex].URL : null;

            new ModernDialog
            {
                Title = "Warning", 
                Content = LanguageOutput.Warnings.PlaylistEmpty
            }.ShowDialog();
            return null;
        }

        /// <summary>
        /// Resorts the Playlist [Not Implemted].
        /// </summary>
        public static void ResortPlayList()
        {
            // NOT IMPLEMENTED!
        }

        /// <summary>
        /// Skips a Track in the given direction.
        /// </summary>
        /// <param name="direction">
        /// The direction to skip.
        /// </param>
        public static void SkipTrack(SkipDirection direction)
        {
            switch (direction)
            {
                case SkipDirection.Backward:
                    if (CurrentIndex - 1 > 0)
                        CurrentIndex--;
                    break;
                case SkipDirection.Forward:
                    if (CurrentIndex + 1 < CurrentPlaylist.Count)
                        CurrentIndex++;
                    break;
            }
        }

        public static void GetFolder()
        {
            Access.InitilazieFolder();
            Folder = Access.Folder;
        }
        ////public static PlaylistItem[] GetRandomAlbums(int Count, string Genre = "*", string YearFrom = "*", string YearTo = "*")
        ////{
        ////    //return Access.GetRandomAlbums(Count, Genre, YearFrom, YearTo); 
        ////}
        ////public static string[,] GetNewestAlbums(int Offset)
        ////{
        ////    return Access.GetNewest(Offset); 
        ////}
        ////public static string[,] GetAllAlbums(int Offset)
        ////{
        ////}
        ////public static string[,] GetNowPlaying()
        ////{
        ////    return Access.GetView(CurrentPlaylist[CurrentIndex].AlbumID); 
        ////}
        ////public static string[,] GetMostAlbums(int Offset)
        ////{
        ////    return Access.GetMostPlayed(Offset);
        ////}

        /// <summary>
        /// The cover download.
        /// </summary>
        /// <param name="coverBox">
        /// The cover box in which the Image is displayed.
        /// </param>
        /// <param name="id">
        /// The Cover ID for the Download.
        /// </param>
        /// <param name="drawReflection">
        /// Draw a Coverreflection.
        /// </param>
        internal static void CoverDownload(Image coverBox, string id, Constants.CoverType coverType)
        {
            Access.SetCoverArt(coverBox, id, coverType);
        }

        /// <summary>
        /// Adds Items to the Playlist.
        /// </summary>
        /// <param name="item">
        /// A Playlistem Object.
        /// </param>
        public static void PlaylistAddItems(MediaItem item)
        {
            if (item.IsDir)
            {
                AddDir(item.AlbumID);
            }
            else
            {
                CurrentPlaylist.Add(item);
            }
        }

        /// <summary>
        /// Adds a whole Directory to the Playlist.
        /// </summary>
        /// <param name="id">
        /// The id of the directory to add.
        /// </param>
        private static void AddDir(string id)
        {
            MediaItem[] items = GetView(id, ViewType.ID);
            foreach (MediaItem item in items)
            {
                if (item.IsDir)
                    AddDir(item.AlbumID);
                else
                    PlaylistAddItems(item);
            }
        }
    }
}