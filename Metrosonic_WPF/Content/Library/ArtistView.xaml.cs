namespace MetroSonic.Content.Library
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using MediaControl;

    /// <summary>
    /// Interaction logic for All.xaml.
    /// </summary>
    public partial class All : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="All"/> class.
        /// </summary>
        public All()
        {
            InitializeComponent();
            var param = Constants.GetParameter();

            LibraryManagement.GetAllAlbums(); 

            //int folderId = int.Parse(param.Where(paramater => paramater.Key.ToLower() == "id").FirstOrDefault().Value);

            //foreach (Canvas cover in LibraryManagement.AllArtists[folderId].Select(item => GuiDrawing.DrawCover(item.Artist, WrapPanel, item.Artist, item, Constants.CoverType.Artist)))
            //{
            //    cover.MouseLeftButtonDown += CoverClickEvent;
            //}
        }
        
        /// <summary>
        /// Clickevent for the cover canvas.
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
            Constants.WindowMain.ContentSource = new Uri("/Content/Library/DetailView.xaml?id=" + mediaItem.ArtistId, UriKind.Relative);
        }
    }
}
