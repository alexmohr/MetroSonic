// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubsonicAccess.cs" company="Alexander Mohr">
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
//   Access class to the subsonic Server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Windows;

namespace MetroSonic.MediaControl
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml;
    using FirstFloor.ModernUI.Windows.Controls;
    using Content;

    /// <summary>
    /// Access class to the subsonic Server.
    /// </summary>
    internal class SubsonicAccess
    {
        /// <summary>
        /// The authentication string used in the subsonic url.
        /// </summary>
        private readonly string _authentication;

        /// <summary>
        /// The _server.
        /// </summary>
        private readonly string _server;

        /// Todo: Make the Authentication string more readable and easy to use.
        /// <summary>
        /// Initializes a new instance of the <see cref="SubsonicAccess"/> class. 
        /// Initializes a new instance of the <see cref="SubsonicAccess"/> class.
        /// </summary>
        /// <param name="auth">
        /// The authentication string.
        /// </param>
        /// <param name="srv">
        /// The Subsonic Server.
        /// </param>
        internal SubsonicAccess(string auth, string srv)
        {
            _server = srv;
            _authentication = auth;
        }

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <value>
        /// The folder.
        /// </value>
        public string[,] Folder { get; private set; }

        /// <summary>
        /// Downloads Artists from Subssonic Folder.
        /// </summary>
        /// <param name="folderID">
        /// The folder ID.
        /// </param>
        /// <returns>
        /// Artistname, ArtistID.
        /// </returns>
        internal MediaItem[] GetArtists(string folderID)
        {
            string musicFolderId = "&musicFolderId=" + folderID;
            string url = _server + "getIndexes.view?" + musicFolderId + _authentication;

            var doc = new XmlDocument();
            doc.Load(url);
            XmlNodeList elemList = doc.GetElementsByTagName("artist");
            var artistDetails = new List<MediaItem>();

            for (int i = 0; i < elemList.Count; i++)
            {
                XmlAttributeCollection xmlAttributeCollection = elemList[i].Attributes;

                if (xmlAttributeCollection == null) continue;
                artistDetails.Add(new MediaItem(xmlAttributeCollection[0].Value, xmlAttributeCollection[1].Value));
            }

            return artistDetails.ToArray();
        }

        /// <summary>
        /// Returns the queried view.
        /// </summary>
        /// <param name="id">
        /// The id to query.
        /// </param>
        /// <param name="type">
        /// The query type.
        /// </param>
        /// <returns>
        /// The <see cref="MediaItem"/>.
        /// </returns>
        internal MediaItem[] GetView(string id, LibraryManagement.ViewType type)
        {
            string url = string.Empty;
            switch (type)
            {
                case LibraryManagement.ViewType.ID:
                    url = _server + "getMusicDirectory.view?id=" + id + _authentication;
                    break;
                case LibraryManagement.ViewType.Random:
                    url = _server + "getAlbumList.view?type=random" + _authentication;
                    break;
                case LibraryManagement.ViewType.All:
                    url = _server + "getAlbumList.view?type=alphabeticalByName&offset=" + id + _authentication;
                    break;
                case LibraryManagement.ViewType.Most:
                    url = _server + "getAlbumList.view?type=frequent&offset=" + id + _authentication;
                    break;
                case LibraryManagement.ViewType.New:
                    url = _server + "getAlbumList.view?type=newest&offset=" + id + _authentication;
                    break;
            }

            return GenerateData(url);
        }

        /// <summary>
        /// Gets the Login Status from Subsonic.
        /// </summary>
        /// <returns>
        /// NULL if everything is ok, else the error message.
        /// </returns>
        internal bool LoginSuccesfull()
        {
            if (string.IsNullOrEmpty(_server))
                return false; 

            string url = _server + "ping.view?" + _authentication;
            var doc = new XmlDocument();
            doc.Load(url);
            XmlNodeList elemList = doc.GetElementsByTagName("error");

            if (elemList[0] != null) return false;

            elemList = doc.GetElementsByTagName("subsonic-response");
            return elemList[0].Attributes.GetNamedItem("status").Value == "ok";
        }

        /// <summary>
        /// Downloads a cover and sets it to a picturebox.
        /// </summary>
        /// <param name="coverArtBox">
        /// The picturebox to display the cover.
        /// </param>
        /// <param name="coverID">
        /// The cover id.
        /// </param>
        /// <param name="drawReflection">
        /// The draw reflection.
        /// </param>
        public void SetCoverArt(Image coverArtBox, string coverID, Constants.CoverType coverType)
        {
            string url = null;
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Constants.AlbumCoverPath;

            switch (coverType)
            {
                case Constants.CoverType.Album:
                    url = _server + "getCoverArt.view?id=" + coverID + _authentication + "size=" + Constants.MaxCoverSize;
                    break;
                case Constants.CoverType.Artist:
                    path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Constants.ArtistCoverPath;
                    break;
            }
            string imagePath = path + coverID + ".png";
            
            if (File.Exists(imagePath))
            {
                coverArtBox.Source = new BitmapImage(new Uri(imagePath));
                coverArtBox.Stretch = Stretch.UniformToFill;
                return;
            }

            if (coverType == Constants.CoverType.Artist)
                url = Constants.GetLastFmArtistUrl(coverID);

            if (string.IsNullOrEmpty(url))
               url = _server + "getCoverArt.view?id=" + "1" + _authentication + "size=" + Constants.MaxCoverSize;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var webClient = new WebClient();

            //webClient.DownloadDataCompleted += delegate(object sender, DownloadDataCompletedEventArgs e)
            //{

                //byte[] graphicData = e.Result;
                byte[] graphicData = webClient.DownloadData(url);
                System.Drawing.Image img  = null;
                using (var ms = new MemoryStream(graphicData, 0, graphicData.Length))
                {
                    ms.Write(graphicData, 0, graphicData.Length);
                    try
                    {
                        img = System.Drawing.Image.FromStream(ms, true);
                        img.Save(imagePath, ImageFormat.Png);
                    }
                    catch (ArgumentException)
                    {
                        img = null;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }

                if (img == null) return;

                coverArtBox.Source = new BitmapImage(new Uri(imagePath));
                coverArtBox.Stretch = Stretch.UniformToFill;
            //};

            //webClient.DownloadDataAsync(new Uri(url));
        }



        /// <summary>
        /// The get all attributs.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] GetAllAttributs(XmlNode node)
        {
            var atr = new List<string>();
            if (node == null || node.Attributes == null) return atr.ToArray();

            for (int i = 0; i < node.Attributes.Count; i++)
            {
                atr.Add(node.Attributes[i].Name);
            }

            return atr.ToArray();
        }

        /// <summary>
        /// The initilazie folder.
        /// </summary>
        internal void InitilazieFolder()
        {
            var items = new List<string> { "All" };
            string url = _server + "getMusicFolders.view?" + _authentication;
            new WebClient().DownloadString(url);
            var doc = new XmlDocument();
            doc.Load(url);


            XmlNodeList elemList = doc.GetElementsByTagName("musicFolder");
            Folder = new string[elemList.Count + 1, 2];
            Folder[0, 1] = 0.ToString(CultureInfo.InvariantCulture);
            Folder[0, 0] = "All";
            for (int i = 0; i < elemList.Count; i++)
            {
                XmlAttributeCollection xmlAttributeCollection = elemList[i].Attributes;
                if (xmlAttributeCollection == null) continue;
                items.Add(xmlAttributeCollection[1].Value);
                Folder[i + 1, 0] = xmlAttributeCollection[(int) FolderIndices.FolderName].Value;
                Folder[i + 1, 1] = xmlAttributeCollection[(int) FolderIndices.FolderID].Value;
            }
        }

        /// <summary>
        /// Generates an array with playlist items from the given URL.
        /// </summary>
        /// <param name="url">
        /// The url where the XML comes from.
        /// </param>
        /// <returns>
        /// The <see cref="MediaItem"/>.
        /// </returns>
        private MediaItem[] GenerateData(string url)
        {
            var items = new List<MediaItem>();
            var doc = new XmlDocument();     
            doc.Load(url);
            XmlNodeList elemList = doc.GetElementsByTagName("child");
            if (elemList.Count == 0)
                elemList = doc.GetElementsByTagName("album");

            for (int i = 0; i < elemList.Count; i++)
            {
                string[] attributes = GetAllAttributs(elemList[i]);
                string coverArt = "1";
                bool isDir = false;

                if (attributes.Contains("coverArt"))
                {
                    XmlAttributeCollection xmlAttributeCollection = elemList[i].Attributes;
                    if (xmlAttributeCollection != null)
                        coverArt = xmlAttributeCollection.GetNamedItem("coverArt").Value;
                }

                if (attributes.Contains("isDir"))
                {
                    XmlAttributeCollection xmlAttributeCollection = elemList[i].Attributes;
                    if (xmlAttributeCollection != null)
                        isDir = bool.Parse(xmlAttributeCollection.GetNamedItem("isDir").Value);

                }

                XmlNodeList directoryElement = doc.GetElementsByTagName("directory");
                string parent = null;
                if (directoryElement.Count > 0)
                {
                    if (GetAllAttributs(directoryElement[0]).Contains("parent"))
                    {
                        XmlAttributeCollection xmlAttributeCollection = directoryElement[0].Attributes;
                        if (xmlAttributeCollection != null)
                            parent = xmlAttributeCollection.GetNamedItem("parent").Value;
                    }
                }

                if (attributes.Contains("contentType"))
                {
                    XmlAttributeCollection xmlAttributeCollection = elemList[i].Attributes;
                    if (xmlAttributeCollection != null &&
                        xmlAttributeCollection.GetNamedItem("contentType").Value.StartsWith("audio"))
                    {
                        XmlAttributeCollection attributeCollection = elemList[i].Attributes;
                        if (attributeCollection != null)
                            items.Add(new MediaItem(
                                attributeCollection.GetNamedItem("id").Value, // < :)
                                attributeCollection.GetNamedItem("title").Value, 
                                attributeCollection.GetNamedItem("album").Value,
                                coverArt, 
                                attributeCollection.GetNamedItem("duration").Value,
                                attributeCollection.GetNamedItem("artist").Value,
                                attributeCollection.GetNamedItem("artistId").Value, 
                                isDir,
                                parent,
                                _server + "stream.view?id=" + attributeCollection.GetNamedItem("id").Value + _authentication));
                    }
                    else
                    {
                        new ModernDialog
                        {
                            Title = "Movie Support", 
                            Content = LanguageOutput.Warnings.MovieNotSupported
                        }.ShowDialog();
                        break;
                    }
                }
                else
                {
                    if (!attributes.Contains("parent"))
                        continue;

                    XmlAttributeCollection xmlAttributeCollection = elemList[i].Attributes;
                    if (xmlAttributeCollection != null)
                        items.Add(new MediaItem(
                            xmlAttributeCollection.GetNamedItem("id").Value,
                            xmlAttributeCollection.GetNamedItem("title").Value,
                            null,
                            coverArt,
                            null,
                            xmlAttributeCollection.GetNamedItem("artist").Value,
                            null,
                            isDir,
                            parent,
                            url));
                }
            }

            return items.ToArray();
        }



        /// <summary>
        /// Enumartor for the FolderIndicies.
        /// </summary>
        internal enum FolderIndices
        {
            /// <summary>
            /// The folder id.
            /// </summary>
            FolderID, 

            /// <summary>
            /// The folder name.
            /// </summary>
            FolderName
        }
    }
}