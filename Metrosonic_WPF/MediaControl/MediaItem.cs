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
namespace MetroSonic.MediaControl
{
    /// <summary>
    /// The playlist item.
    /// </summary>
    public class MediaItem
    {
        /// <summary>
        /// Indicates if the Item is a directory.
        /// </summary>
        private readonly bool _isDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaItem"/> class.
        /// </summary>
        /// <param name="albumID">
        /// The album id.
        /// </param>
        /// <param name="trackName">
        /// The track name.
        /// </param>
        /// <param name="albumName">
        /// The album name.
        /// </param>
        /// <param name="coverID">
        /// The cover id.
        /// </param>
        /// <param name="trackDuration">
        /// The track duration.
        /// </param>
        /// <param name="artist">
        /// The artist.
        /// </param>
        /// <param name="isDir">
        /// The is dir.
        /// </param>
        /// <param name="parentID">
        /// The parent id.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        public MediaItem(string albumID, string trackName, string albumName, string coverID, 
            string trackDuration, string artist, string artistId, bool isDir, string parentID, string url = null)
        {
            AlbumID = albumID;
            TrackName = trackName;
            AlbumName = albumName;
            CoverID = coverID;
            URL = url;
            TrackDuration = trackDuration;
            Artist = artist;
            _isDir = isDir;
            ParentID = parentID;
            ArtistId = artistId;
        }

        public MediaItem(string artist, string artistId)
        {
            Artist = artist;
            ArtistId = artistId; 
        }

        /// <summary>
        /// Gets the Name of the Artist.
        /// </summary>
        /// <value>
        /// The artist.
        /// </value>
        public string Artist { get; private set; }

        // private string TrackID { get; set; }

        /// <summary>
        /// Gets the album id.
        /// </summary>
        /// <value>
        /// The album id.
        /// </value>
        public string AlbumID { get; private set; }

        /// <summary>
        /// Gets the track name.
        /// </summary>
        /// <value>
        /// The track name.
        /// </value>
        public string TrackName { get; private set; }

        /// <summary>
        /// Gets the album name.
        /// </summary>
        /// <value>
        /// The album name.
        /// </value>
        public string AlbumName { get; private set; }

        /// <summary>
        /// Gets the cover id.
        /// </summary>
        /// <value>
        /// The cover id.
        /// </value>
        public string CoverID { get; private set; }

        /// <summary>
        /// Gets the url.
        /// </summary>
        /// <value>
        /// The url.
        /// </value>
        public string URL { get; private set; }

        /// <summary>
        /// Gets the track duration.
        /// </summary>
        /// <value>
        /// The track duration.
        /// </value>
        public string TrackDuration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is dir.
        /// </summary>
        /// <value>
        /// The is dir.
        /// </value>
        public bool IsDir
        {
            get { return _isDir; }
        }

        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentID { get; private set; }

        public string ArtistId { get; private set; }

        
    }
}