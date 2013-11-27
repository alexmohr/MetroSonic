// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Alexander Mohr">
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
//   Interaction logic for MainWindow.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using MetroSonic.MediaControl;

namespace MetroSonic
{
    using System.ComponentModel;
    using System.Windows.Media;
    using FirstFloor.ModernUI.Presentation;
    using Properties;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            LoadAppearance();

            // ContentSource = new Uri("/View/ContextPage.xaml", UriKind.Relative);
        }

        /// <summary>
        /// The load appearance.
        /// </summary>
        private void LoadAppearance()
        {
            if (Settings.Default.color != string.Empty)
            {
                object convertFromString = ColorConverter.ConvertFromString(Settings.Default.color);
                if (convertFromString != null)
                    AppearanceManager.Current.AccentColor = (Color)convertFromString;
            }

            AppearanceManager.Current.FontSize = Settings.Default.fontSize;

            if (Settings.Default.darkTheme)
                AppearanceManager.Current.DarkThemeCommand.Execute(null);
            else
                AppearanceManager.Current.LightThemeCommand.Execute(null);

            Width = Settings.Default.mainWidth;
            Height = Settings.Default.mainHeight;
            WindowState = Settings.Default.mainState;
            Left = Settings.Default.mainLeft;
            Top = Settings.Default.mainTop; 
        }

        /// <summary>
        /// The window_ closing.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The eventarguments.
        /// </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            LibraryManagement.Playback.StopPlayback();
            LibraryManagement.Playback.Dispose();

            Settings.Default.mainWidth = Width;
            Settings.Default.mainHeight = Height;
            Settings.Default.mainState = WindowState;
            Settings.Default.mainLeft = Left;
            Settings.Default.mainTop = Top;
            Settings.Default.Save();
        }
    }
}