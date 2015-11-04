using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MetroSonic.MediaControl;
using MetroSonic.Utils;

namespace MetroSonic.Content
{
    class GuiDrawing
    {
        public static Canvas DrawCover(MediaItem item, WrapPanel targetPanel, string Text, MediaItem tag, Constants.CoverType coverType)
        {
            var displayedCanvas = new Canvas()
            {
                Width = 250,
                Height = 250,
                Margin = new Thickness(10, 10, 0, 0)
            };

            var imagePath =
                LibraryManagement.CoverDownload(item, coverType).Result;

            var cover = new Image()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Stretch = Stretch.UniformToFill,
                Width = 250,
                Height = 250,
                Source = new BitmapImage(new Uri(imagePath))
            };



            var label = new TextBlock
            {
                MaxWidth = 200,
                Text = Text,
                FontSize = 15,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            var labelOutline1 = new TextBlock { MaxWidth = 200, Text = Text, FontSize = 15, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap };
            var labelOutline2 = new TextBlock { MaxWidth = 200, Text = Text, FontSize = 15, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap };
            var labelOutline3 = new TextBlock { MaxWidth = 200, Text = Text, FontSize = 15, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap };
            var labelOutline4 = new TextBlock { MaxWidth = 200, Text = Text, FontSize = 15, Foreground = Brushes.Black, TextWrapping = TextWrapping.Wrap };

            const int labelFromTop = 200;
            const int labelFromLeft = 20;

            // White label
            Canvas.SetLeft(label, labelFromLeft);
            Canvas.SetTop(label, labelFromTop);

            // making the outline
            const int borderWith = 1;


            Canvas.SetTop(labelOutline1, labelFromTop - borderWith);
            Canvas.SetLeft(labelOutline1, labelFromLeft - borderWith);

            Canvas.SetTop(labelOutline2, labelFromTop - borderWith);
            Canvas.SetLeft(labelOutline2, labelFromLeft + borderWith);

            Canvas.SetTop(labelOutline3, labelFromTop + borderWith);
            Canvas.SetLeft(labelOutline3, labelFromLeft - borderWith);

            Canvas.SetTop(labelOutline4, labelFromTop + borderWith);
            Canvas.SetLeft(labelOutline4, labelFromLeft + borderWith);

            displayedCanvas.Children.Add(cover);

            displayedCanvas.Children.Add(labelOutline1);
            displayedCanvas.Children.Add(labelOutline2);
            displayedCanvas.Children.Add(labelOutline3);
            displayedCanvas.Children.Add(labelOutline4);

            displayedCanvas.Children.Add(label);
            displayedCanvas.Tag = tag;

            targetPanel.Children.Add(displayedCanvas);

            return displayedCanvas;
        }
    }
}
