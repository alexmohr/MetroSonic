using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    /// Interaction logic for All.xaml
    /// </summary>
    public partial class All : UserControl
    {
        private int _folderId;
        private string _folderName;

        /// <summary>
        /// Initializes a new instance of the <see cref="All"/> class.
        /// </summary>
        public All()
        {
            InitializeComponent();
            var param = Constants.GetParameter();

            _folderId = int.Parse(param.Where(paramater => paramater.Key.ToLower() == "id").FirstOrDefault().Value);
            _folderName = param.Where(paramater => paramater.Key.ToLower() == "name").FirstOrDefault().Value;
            
            // string type = param.Where(paramater => paramater.Key.ToLower() == "type").FirstOrDefault().Value;

            foreach (Canvas cover in LibraryManagement.AllArtists[_folderId].Select(item => GuiDrawing.DrawCover(item.Artist, WrapPanel, item.Artist, item, Constants.CoverType.Artist)))
            {
                cover.MouseLeftButtonDown += GuiDrawing.CoverClickEvent;
            }
        }
    }
}

