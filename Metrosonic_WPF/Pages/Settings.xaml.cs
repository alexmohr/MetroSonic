// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.xaml.cs" company="Alexander Mohr">
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
//   Interaction logic for Settings.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;

namespace MetroSonic.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml.
    /// </summary>
    public partial class Settings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The connect.
        /// </summary>
        /// <param name="connectionId">
        /// The connection id.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save button click event.
        /// </summary>
        /// <param name="sender">
        /// The sending control.
        /// </param>
        /// <param name="e">
        /// The EventArguments.
        /// </param>
        /// <remarks>
        /// Saves all Settings to the Application settings
        ///     After save it will navigate to the previous page.
        /// </remarks>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.color = AppearanceManager.Current.AccentColor.ToString();
            Properties.Settings.Default.darkTheme = AppearanceManager.Current.ThemeSource.OriginalString.Contains("Dark");
            Properties.Settings.Default.server = Properties.Settings.Default.server;
            
            NavigationCommands.BrowseBack.Execute(null, null);

            Properties.Settings.Default.Save();
            new ModernDialog
            {
                Title = "Settings saved", 
                Content = "Settings have been saved!."
            }.ShowDialog();
        }
    }
}