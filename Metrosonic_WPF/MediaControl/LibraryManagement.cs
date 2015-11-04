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

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MetroSonic.MediaControl.Playback;

namespace MetroSonic.MediaControl
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Content;
    using FirstFloor.ModernUI.Windows.Controls;
    using Properties;

    /// <summary>
    /// The library management.
    /// </summary>
    public static class LibraryManagement
    {
        /// <summary>
        /// The current playlist.
        /// </summary>
        public static List < MediaItem > CurrentPlaylist
        {
            get
            {
                _currentPlaylist.Clear();
                foreach (var song in LibraryManagement.Playback.GetCurrentPlaylist())
                {
                    _currentPlaylist.Add(song);
                }
                return _currentPlaylist;
            }
        }

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
        /// The playback control.
        /// </summary>
        public static readonly PlaybackControlBase Playback;

        /// <summary>
        /// The access to the Subsonic Server.
        /// </summary>
        private static readonly MediaAccessBase Access;

        private static readonly List<int> PlayedSongs = new List<int>();

        


        /// <summary>
        /// Initializes static members of the <see cref="LibraryManagement"/> class.
        /// </summary>
        static LibraryManagement()
        {
            
            Access = new MpdAccess();
            Playback = new MpdPlaybackControl();
            
            

        }

        public static void Init()
        {
            foreach (var item in Playback.GetCurrentPlaylist())
                PlaylistAddItems(item);
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

        private static MediaItem[] _allArtists;
        public static MediaItem[] AllArtists
        {
            get
            {
               if (_allArtists == null)
                    GetAllArtists();
                return _allArtists;
            }
        }

        private static MediaItem[] _allAlbums;
        public static MediaItem[] AllAlbums
        {
            get
            {
                if (_allAlbums == null)
                    _allAlbums =  GetAllAlbums();
                return _allAlbums;
            }
        }

        public static MediaItem[] GetAlbumTracks( MediaItem album )
        {
            return Access.GetTracks( album ); 
        }

        /// <summary>
        /// Queries the login status.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// Return the login status from the server.
        /// </returns>
        public static bool CanLoginSuccessfully(string server, string username, string password)
        {
            bool canLogin; 
            try
            {
                canLogin= Access.Login(username, password, server, 6600);
            }
            catch (Exception e )
            {
                return false; 
            }

            return canLogin; 
        }

        

        private static void GetAllArtists()
        {
            _allArtists = Access.GetArtists();
            
            //_allArtists = new MediaItem[1][] { };

        }

        //{
        //    if (Folder == null)
        //        GetFolder();

        //    if (Folder == null) return;

        //    int folderCount = Folder.GetLength(0); 

        //    _allArtists = new MediaItem[folderCount][];

        //    for (int i = 0; i < folderCount; i++)
        //    {
        //        _allArtists[i] = GetArtists(Folder[i, 1]);
        //    }
        //}


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

        private static bool _initialized = false;
        private static readonly List < MediaItem > _currentPlaylist = new List < MediaItem >();

        /// <summary>
        /// Gets the Subsonic URL of the Current Track.
        /// </summary>
        /// <returns>
        /// The Subsonic URL.
        /// </returns>
        public static string PlaylistGetCurrentTrackURL()
        {
            if ( CurrentPlaylist.Count == 0 )
            {
                new ModernDialog
                {
                    Title = "Warning",
                    Content = LanguageOutput.Warnings.PlaylistEmpty
                }.ShowDialog();
                return null;
            }

            return CurrentIndex < CurrentPlaylist.Count ? CurrentPlaylist[CurrentIndex].URL : null;
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
            PlayedSongs.Add(CurrentIndex);

            if (Shuffle)
            {
                var rnd = new Random();
                switch (direction)
                {
                    case SkipDirection.Forward:
                        do
                        {
                            CurrentIndex = rnd.Next(CurrentPlaylist.Count - 1);
                        } 
                        while (PlayedSongs.Contains(CurrentIndex));
                        break;

                    case SkipDirection.Backward:
                        if (PlayedSongs.Count > 1)
                        {
                            CurrentIndex = PlayedSongs[PlayedSongs.Count - 1];
                        }

                        break;
                }
            }
            else if (Repeat)
            {

            }
            else
            {
                switch (direction)
                {
                    case SkipDirection.Backward:
                        if (CurrentIndex - 1 > -1)
                            CurrentIndex--;
                        break;
                    case SkipDirection.Forward:
                        if (CurrentIndex + 1 < CurrentPlaylist.Count)
                            CurrentIndex++;
                        break;
                }
            }

            Playback.Next();
        }

        public static void GetFolder()
        {
          //  Access.GetLib
            //Folder = Access.Folder;
        }
        
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
        internal async static Task<string> CoverDownload(MediaItem item, Constants.CoverType coverType)
        {
            var x = Access.GetCoverArtPath( item, coverType );
            return x;
        }

        /// <summary>
        /// Adds Items to the Playlist.
        /// </summary>
        /// <param name="item">
        /// A Playlistem Object.
        /// </param>
        public static void PlaylistAddItems(MediaItem item)
        {
            //if (item.IsDir)
            //{
            //    AddDir(item.AlbumID);
            //}
            //else
            //{
            //    CurrentPlaylist.Add(item);
            //}
            CurrentPlaylist.Add(item);
            Playback.AddToPlaylist( item );

        }

        ///// <summary>
        ///// Adds a whole Directory to the Playlist.
        ///// </summary>
        ///// <param name="id">
        ///// The id of the directory to add.
        ///// </param>
        //private static void AddDir(string id)
        //{
        //    MediaItem[] items = GetView(id, ViewType.ID);
        //    foreach (MediaItem item in items)
        //    {
        //        if (item.IsDir)
        //            AddDir(item.AlbumID);
        //        else
        //            PlaylistAddItems(item);
        //    }
        //}
        public static MediaItem[] GetAllAlbums()
        {
            return Access.GetAllAbums();
        }

        public static void PlayListClear()
        {
            CurrentPlaylist.Clear();
            CurrentIndex = 0; 
        }
    }
}