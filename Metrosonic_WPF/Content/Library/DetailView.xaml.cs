﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetailView.xaml.cs" company="Alexander Mohr">
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
//   Interaction logic for DetailView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MetroSonic.Content.Library
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using MediaControl;

    /// <summary>
    /// Interaction logic for DetailView.xaml.
    /// </summary>
    public partial class DetailView : UserControl
    {
        /// <summary>
        /// All MediaItems.
        /// </summary>
        private readonly MediaItem[] _allItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailView"/> class.
        /// </summary>
        public DetailView()
        {
            InitializeComponent();
            var param = Constants.GetParameter();
            string id = param.Where(paramater => paramater.Key.ToLower() == "id").FirstOrDefault().Value;

            // _allItems = LibraryManagement.GetView(id, LibraryManagement.ViewType.ID);
            if (!Constants.MainDisplayedMediaItem.IsDir)
            {
                Constants.WindowMain.ContentSource =
                    new Uri("/Content/Library/AlbumView.xaml?id=" + id, UriKind.Relative);
                return;
            }

            foreach (MediaItem item in LibraryManagement.AllAlbums)
            {
                Title.Text = "Albums by " + item.ArtistName;
                Canvas cover = GuiDrawing.DrawCover(item, WrapPanel, item.TrackName, item, Constants.CoverType.Album);
                cover.MouseLeftButtonDown += CoverClickEvent;
            }
        }

        /// <summary>
        /// The click event for the covers.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private static void CoverClickEvent(object sender, EventArgs e)
        {
            var sendingControl = (Canvas)sender;
            var mediaItem = (MediaItem)sendingControl.Tag;

            Constants.MainDisplayedMediaItem = mediaItem; 

            Constants.WindowMain.ContentSource = mediaItem.IsDir 
                ? new Uri("/Content/Library/DetailView1.xaml?id=" + mediaItem.AlbumID, UriKind.Relative) 
                : new Uri("/Content/Library/AlbumView.xaml?id=" + mediaItem.AlbumID, UriKind.Relative);
        }

        /// <summary>
        /// The click event for the covers.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (MediaItem item in _allItems)
            {
                LibraryManagement.PlaylistAddItems(item);
            }
        }
    }
}
