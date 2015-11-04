// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaylistItem.cs" company="Alexander Mohr">
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
//   The playlist item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace MetroSonic.MediaControl
{
    /// <summary>
    /// The playlist item.
    /// </summary>
    public class MediaItem
    {
        /// <summary>
        /// Gets the Name of the Artist.
        /// </summary>
        /// <value>
        /// The artist.
        /// </value>
        public string ArtistName { get;  set; }

        // private string TrackID { get; set; }

        /// <summary>
        /// Gets the album id.
        /// </summary>
        /// <value>
        /// The album id.
        /// </value>
        public string AlbumID { get; set; }

        /// <summary>
        /// Gets the track name.
        /// </summary>
        /// <value>
        /// The track name.
        /// </value>
        public string TrackName { get; set; }

        /// <summary>
        /// Gets the album name.
        /// </summary>
        /// <value>
        /// The album name.
        /// </value>
        public string AlbumName { get; set; }

        /// <summary>
        /// Gets the cover id.
        /// </summary>
        /// <value>
        /// The cover id.
        /// </value>
        public string CoverID { get; set; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        /// <value>
        /// The url.
        /// </value>
        public string URL { get; set; }

        /// <summary>
        /// Gets the track duration.
        /// </summary>
        /// <value>
        /// The track duration.
        /// </value>
        public TimeSpan TrackDuration { get; set; }

        /// <summary>
        /// Gets a value indicating whether is dir.
        /// </summary>
        /// <value>
        /// The is dir.
        /// </value>
        public bool IsDir { get; set; }

        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentID { get; set; }

        public string ArtistId { get; set; }

        public int Id
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }
    }
}