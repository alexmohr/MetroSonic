// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Alexander Mohr">
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
//   The constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using MetroSonic.MediaControl;
using Size = System.Drawing.Size;

namespace MetroSonic
{
    /// <summary>
    /// The constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The max cover size.
        /// </summary>
        public const int MaxCoverSize = 300;

        /// <summary>
        /// The last fm api key.
        /// </summary>
        public const string LastFmApiKey = "0b5c899b0a3b089b49b4c7ee5ffb2abc";

        /// <summary>
        /// The cover path.
        /// </summary>
        public static string AlbumCoverPath = @"\cache\covers\";

        /// <summary>
        /// The artist cover path.
        /// </summary>
        public static string ArtistCoverPath = @"\cache\artists\";

        /// <summary>
        /// The MainWindow of the Application.
        /// </summary>
        public static readonly MainWindow WindowMain = (MainWindow) Application.Current.MainWindow;
    

        /// <summary>
        /// True if user is logged in.
        /// </summary>
        public static bool LoggedIn { get; set; }

        /// <summary>
        /// Makes a TimeString from a Double Value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string TimeStringFromDouble(double value)
        {
            string str = TimeSpan.FromSeconds(value).ToString();
            if (str[1] == '0')
                str = str.Substring(3);
            return str;
        }

        public static KeyValuePair<string, string>[] GetParameter()
        {
            var hasParametersCheck = WindowMain.ContentSource.OriginalString.Split('?');
            if (hasParametersCheck.Length != 2)
                return null;
            string allData = hasParametersCheck[1];

            return (from param in allData.Split('&') select param.Split('=') into hasValuesCheck where hasValuesCheck.Length == 2 select new KeyValuePair<string, string>(hasValuesCheck[0], hasValuesCheck[1])).ToArray();
        }

        /// <summary>
        /// Type of The cover
        /// </summary>
        public enum CoverType
        {
            /// <summary>
            /// Download album cover from subsonic
            /// </summary>
            Album,

            /// <summary>
            /// Download artist cover from last.fm
            /// </summary>
            Artist
        };

        /// <summary>
        /// Gets the url to the artist picture.
        /// </summary>
        /// <param name="coverID"></param>
        /// <returns></returns>
        internal static string GetLastFmArtistUrl(string coverID)
        {
            string url =
                string.Format(
                    "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist={0}&api_key=0b5c899b0a3b089b49b4c7ee5ffb2abc",
                    coverID);

            var doc = new XmlDocument();
            try
            {
                doc.Load(url);
            }
            catch (WebException)
            {
                return null;
            }


            XmlNodeList elemList = doc.GetElementsByTagName("image");
            for (int i = 0; i < elemList.Count; i++)
            {
                if (elemList[i].Attributes == null) continue;
                if (elemList[i].Attributes.Cast<XmlAttribute>().Any(attr => attr.Value == "extralarge"))
                {
                    return elemList[i].InnerText;
                }
            }

            return null;
            //return (from XmlNode node in elemList from XmlAttribute attribute in node.Attributes where attribute.Name == "extralarge" select node).Select(node => node.Value).FirstOrDefault();
        }

        /// <summary>
        /// Gets the server url.
        /// </summary>
        /// <param name="hostname">
        /// The hostname.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string GetServerUrl(string hostname)
        {
            if (!hostname.EndsWith("/rest/"))
            {
                string apiIdent = "rest/";
                if (!hostname.EndsWith("/"))
                    apiIdent = "/" + apiIdent;
                hostname += apiIdent;
            }

            const string http = "http://";

            if (!hostname.StartsWith(http))
                hostname = http + hostname;

            return hostname; 
        }
    }
}