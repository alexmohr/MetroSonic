// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsNetwork.xaml.cs" company="Alexander Mohr">
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
//   Interaction logic for SettingsNetwork.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using FirstFloor.ModernUI.Windows.Controls;
using MetroSonic.MediaControl;

namespace MetroSonic.Content.Settings
{
    /// <summary>
    /// Interaction logic for SettingsNetwork.xaml.
    /// </summary>
    public partial class SettingsNetwork : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNetwork"/> class.
        /// </summary>
        public SettingsNetwork()
        {
            InitializeComponent();
            TxtPassword.Text = Properties.Settings.Default.password;
            TxtServer.Text = Properties.Settings.Default.server;
            TxtUsername.Text = Properties.Settings.Default.username; 
        }

        /// <summary>
        /// The txt server_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void txtServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.server = TxtServer.Text;
        }

        /// <summary>
        /// The txt username_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.username = TxtUsername.Text;
        }

        /// <summary>
        /// The txt password_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.password = TxtPassword.Text;
        }

        /// <summary>
        /// The txt password_ password changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The bt test connection_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btTestConnection_Click(object sender, RoutedEventArgs e)
        {
            bool successfull = LibraryManagement.LoginSuccessfull();
            if (successfull)
                new ModernDialog()
                {
                    Content = LanguageOutput.Notice.ConnectionSuccessfull,
                }.ShowDialog(); 
            else
                new ModernDialog()
                {
                    Title = LanguageOutput.Errors.LoginFailedShort,
                    Content = LanguageOutput.Errors.LoginFailedShort
                }.ShowDialog();
        }
    }
}