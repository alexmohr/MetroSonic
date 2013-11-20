// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsAppearanceViewModel.cs" company="Alexander Mohr">
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
//   A simple view model for configuring theme, font and accent colors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;

namespace MetroSonic.Content
{
    /// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class SettingsAppearanceViewModel
        : NotifyPropertyChanged
    {
        /// <summary>
        /// The font small.
        /// </summary>
        private const string FontSmall = "small";

        /// <summary>
        /// The font large.
        /// </summary>
        private const string FontLarge = "large";

        // 9 accent colors from metro design principles
        /*private Color[] accentColors = new Color[]{
            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x33, 0x99, 0x33),   // green
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            Color.FromRgb(0xa2, 0x00, 0xff),   // purple            
        };*/

        // 20 accent colors from Windows Phone 8
        /// <summary>
        /// The accent colors.
        /// </summary>
        private readonly Color[] accentColors =
        {
            Color.FromRgb(0xa4, 0xc4, 0x00), // lime
            Color.FromRgb(0x60, 0xa9, 0x17), // green
            Color.FromRgb(0x00, 0x8a, 0x00), // emerald
            Color.FromRgb(0x00, 0xab, 0xa9), // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2), // cyan
            Color.FromRgb(0x00, 0x50, 0xef), // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff), // indigo
            Color.FromRgb(0xaa, 0x00, 0xff), // violet
            Color.FromRgb(0xf4, 0x72, 0xd0), // pink
            Color.FromRgb(0xd8, 0x00, 0x73), // magenta
            Color.FromRgb(0xa2, 0x00, 0x25), // crimson
            Color.FromRgb(0xe5, 0x14, 0x00), // red
            Color.FromRgb(0xfa, 0x68, 0x00), // orange
            Color.FromRgb(0xf0, 0xa3, 0x0a), // amber
            Color.FromRgb(0xe3, 0xc8, 0x00), // yellow
            Color.FromRgb(0x82, 0x5a, 0x2c), // brown
            Color.FromRgb(0x6d, 0x87, 0x64), // olive
            Color.FromRgb(0x64, 0x76, 0x87), // steel
            Color.FromRgb(0x76, 0x60, 0x8a), // mauve
            Color.FromRgb(0x87, 0x79, 0x4e) // taupe
        };

        /// <summary>
        /// The themes.
        /// </summary>
        private readonly LinkCollection themes = new LinkCollection();

        /// <summary>
        /// The selected accent color.
        /// </summary>
        private Color selectedAccentColor;

        /// <summary>
        /// The selected font size.
        /// </summary>
        private string selectedFontSize;

        /// <summary>
        /// The selected theme.
        /// </summary>
        private Link selectedTheme;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsAppearanceViewModel"/> class.
        /// </summary>
        public SettingsAppearanceViewModel()
        {
            // add the default themes
            themes.Add(new Link {DisplayName = "dark", Source = AppearanceManager.DarkThemeSource});
            themes.Add(new Link {DisplayName = "light", Source = AppearanceManager.LightThemeSource});

            SelectedFontSize = AppearanceManager.Current.FontSize == FontSize.Large ? FontLarge : FontSmall;
            SyncThemeAndColor();

            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

        /// <summary>
        /// Gets the themes.
        /// </summary>
        /// <value>
        /// The themes.
        /// </value>
        public LinkCollection Themes
        {
            get { return themes; }
        }

        /// <summary>
        /// Gets the font sizes.
        /// </summary>
        /// <value>
        /// The font sizes.
        /// </value>
        public string[] FontSizes
        {
            get { return new[] {FontSmall, FontLarge}; }
        }

        /// <summary>
        /// Gets the accent colors.
        /// </summary>
        /// <value>
        /// The accent colors.
        /// </value>
        public Color[] AccentColors
        {
            get { return accentColors; }
        }

        /// <summary>
        /// Gets or sets the selected theme.
        /// </summary>
        /// <value>
        /// The selected theme.
        /// </value>
        public Link SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    OnPropertyChanged("SelectedTheme");

                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected font size.
        /// </summary>
        /// <value>
        /// The selected font size.
        /// </value>
        public string SelectedFontSize
        {
            get { return selectedFontSize; }
            set
            {
                if (selectedFontSize != value)
                {
                    selectedFontSize = value;
                    OnPropertyChanged("SelectedFontSize");

                    AppearanceManager.Current.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected accent color.
        /// </summary>
        /// <value>
        /// The selected accent color.
        /// </value>
        public Color SelectedAccentColor
        {
            get { return selectedAccentColor; }
            set
            {
                if (selectedAccentColor != value)
                {
                    selectedAccentColor = value;
                    OnPropertyChanged("SelectedAccentColor");

                    AppearanceManager.Current.AccentColor = value;
                }
            }
        }

        /// <summary>
        /// The sync theme and color.
        /// </summary>
        private void SyncThemeAndColor()
        {
            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            SelectedTheme = themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource));

            // and make sure accent color is up-to-date
            SelectedAccentColor = AppearanceManager.Current.AccentColor;
        }

        /// <summary>
        /// The on appearance manager property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor")
            {
                SyncThemeAndColor();
            }
        }
    }
}