// --------------------------------------------------------------------------------------------------------------------
// <copyright file="All.xaml.cs" company="Alexander Mohr">
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
//   Interaction logic for All.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MetroSonic.Content;
using MetroSonic.MediaControl;

namespace MetroSonic.Pages.Welcome
{
    /// <summary>
    /// Interaction logic for All.xaml.
    /// </summary>
    public partial class All : UserControl
    {
        private int _index = 0;
        private Button _addButton;
        private LibraryManagement.ViewType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="All"/> class.
        /// </summary>
        public All()
        {
            InitializeComponent();
            var param = Constants.GetParameter();
            string paramType = param.Where(paramater => paramater.Key.ToLower() == "type").FirstOrDefault().Value;

            switch (paramType)
            {
                case "all":
                    _type = LibraryManagement.ViewType.All;
                    Title.Text = "All Albums";
                    break; 
                case "new":
                    _type = LibraryManagement.ViewType.New;
                    Title.Text = "New Albums";
                    break;
                case "random": 
                    _type = LibraryManagement.ViewType.Random;
                    Title.Text = "Random Albums";
                    break; 
                case "mostplayed":
                    _type = LibraryManagement.ViewType.Most;
                    Title.Text = "Most plaed Albums";
                    break; 
                case "nowplaying":
                    Title.Text = "Now playing Album";
                    _type = LibraryManagement.ViewType.Now;
                    break;
            }
            GetMediaItems();
        }

        private void GetMediaItems()
        {
            foreach (Canvas cover in LibraryManagement.GetView(_index.ToString(CultureInfo.InvariantCulture), _type).Select(item => GuiDrawing.DrawCover(item.CoverID, WrapPanel, item.TrackName, item, Constants.CoverType.Album)))
            {
                cover.MouseLeftButtonDown += GuiDrawing.CoverClickEvent;
            }

            _addButton = new Button
            {
                Content = "More...",
                Width = 200,
                Name = "btMore",
                Margin = new Thickness(50, 10, 0, 0),
                Height = 25,
            };
            _addButton.Click += ClickMoreEvent;

            WrapPanel.Children.Add(_addButton);
        }

        private void ClickMoreEvent(object sender, EventArgs e)
        {
            WrapPanel.Children.Remove(_addButton);
            _index += 11;
            GetMediaItems();
        }

       

    }
}