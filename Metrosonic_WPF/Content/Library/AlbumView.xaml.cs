using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MetroSonic.MediaControl;

namespace MetroSonic.Content.Library
{
    /// <summary>
    /// Interaction logic for AlbumView.xaml
    /// </summary>
    public partial class AlbumView : UserControl
    {
        /// <summary>
        /// All MediaItems from the album.
        /// </summary>
        private readonly MediaItem[] _albumData;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumView"/> class.
        /// </summary>
        public AlbumView()
        {
            InitializeComponent();
            var param = Constants.GetParameter();
            string id = param.Where(paramater => paramater.Key.ToLower() == "id").FirstOrDefault().Value;

            _albumData = LibraryManagement.GetView(id, LibraryManagement.ViewType.ID);
            
            var columnName = new DataGridTextColumn();
            var columnLength = new DataGridTextColumn();

            if (_albumData.Length == 0)
                return;

            LibraryManagement.CoverDownload(Cover, _albumData[0].CoverID, Constants.CoverType.Album);
            Title.Text = _albumData[0].AlbumName + " by " + _albumData[0].Artist;

            columnName.Header = "Name";
            columnName.Binding = new Binding("Name");

            columnLength.Header = "Length";
            columnLength.Binding = new Binding("Length");

            DataGrid.Columns.Add(columnName);
            DataGrid.Columns.Add(columnLength);

            foreach (MediaItem item in _albumData)
            {
                DataGrid.Items.Add(new DataItem
                {
                    Name = item.TrackName,
                    Length = item.TrackDuration.ToString("mm':'ss"),
                    Rating = null
                });
            }
        }

        /// <summary>
        /// Button click event to add items to the playlist.
        /// </summary>
        /// <param name="sender">The sending control.</param>
        /// <param name="e">The eventparameter.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (MediaItem item in _albumData)
                LibraryManagement.PlaylistAddItems(item);
        }
    }

    class DataItem
    {
        public string Name { get; set; }
        public string Length { get; set; }
        public string Rating { get; set; }
    }
}
