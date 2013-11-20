using System;
using System.Collections.Generic;
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
    /// Interaction logic for DetailView.xaml
    /// </summary>
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
            var param = Constants.GetParameter();
            string id = param.Where(paramater => paramater.Key.ToLower() == "id").FirstOrDefault().Value;
            

            foreach (MediaItem item in LibraryManagement.GetView(id, LibraryManagement.ViewType.ID))
            {
                Title.Text = "Albums by " + item.Artist;
                if (!item.IsDir)
                {
                    Constants.WindowMain.ContentSource =
                        new Uri("/Content/Library/AlbumView.xaml?id=" + id, UriKind.Relative);
                    return;
                }
                Canvas cover = GuiDrawing.DrawCover(item.CoverID, WrapPanel, item.TrackName, item, Constants.CoverType.Album);
                cover.MouseLeftButtonDown += ClickEvent;
            }
        }

        private void ClickEvent(object sender, EventArgs e)
        {
            var sendingControl = (Canvas)sender;
            var mediaItem = (MediaItem)sendingControl.Tag;
            Constants.WindowMain.ContentSource = mediaItem.IsDir ? new Uri("/Content/Library/DetailView1.xaml?id=" + mediaItem.AlbumID, UriKind.Relative) : new Uri("/Content/Library/AlbumView.xaml?id=" + mediaItem.AlbumID, UriKind.Relative);
        }
    }
}
