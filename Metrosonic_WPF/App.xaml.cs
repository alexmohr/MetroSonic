﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Alexander Mohr">
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
//   Interaction logic for App.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;

namespace MetroSonic
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += OnStartup;
        }

        private void OnStartup( object sender, StartupEventArgs args )
        {
            if (args.Args.Any(x => x.ToLower() == "/disablecoverhheck"))
            {
                Constants.CheckCovers = false;
            }
        }

        /// <summary>
        /// The on exit.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnExit(object sender, ExitEventArgs e)
        {
            // throw new NotImplementedException();
        }
    }
}