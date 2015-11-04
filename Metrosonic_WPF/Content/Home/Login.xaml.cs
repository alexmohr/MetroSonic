// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Login.xaml.cs" company="Alexander Mohr">
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
//   Interaction logic for Login.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using MetroSonic.MediaControl;
using MetroSonic.Utils;

namespace MetroSonic.Content.Welcome
{
    /// <summary>
    /// Interaction logic for Login.xaml.
    /// </summary>
    public partial class Login : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            InitializeComponent();

            // Constants.WindowMain.MenuLinkGroups.Clear();
            // Constants.WindowMain.TitleLinks.Clear();
            DoLogin();
        }

        /// <summary>
        /// The test login.
        /// </summary>
        private void DoLogin()
        {
            var login = new Thread(() =>
            {
                bool success = LibraryManagement.CanLoginSuccessfully(Properties.Settings.Default.server,
                    Properties.Settings.Default.username, Properties.Settings.Default.password);
                if (success)
                {
                    GetArtists();
                    GetArtistCovers();
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate
                        {
                            Constants.LoggedIn = true;
                            LibraryManagement.GetFolder();
                            BuildNavigation();
                            Constants.WindowMain.ContentSource = new Uri("/Content/Home/AlbumPage.xaml?type=all", UriKind.Relative);
                        });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate
                       {
                           new ModernDialog
                           {
                               Content = LanguageOutput.Errors.LoginFailedLong,
                               Title = LanguageOutput.Errors.LoginFailedShort,
                           }.ShowDialog();
                           BuildNavigation();
                           Constants.WindowMain.ContentSource = new Uri("/Pages/Settings.xaml", UriKind.Relative);
                       });
                }
            });
            login.SetApartmentState(ApartmentState.STA);
            login.Start();
        }

        private void GetArtists()
        {
            if (Application.Current == null)
                return;

            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                (ThreadStart)delegate
                {
                    LabelStatus.Text = LanguageOutput.Notice.GettingArtists;
                    var init = LibraryManagement.AllArtists;
                });
        }

        private void GetArtistCovers()
        {
            if ( !Constants.CheckCovers )
            {
                return; 
            }


            const string artistCoverDownload = "Downloading artist covers {0} of {1}";
            const string albumCoverDownload = "Downloading album covers {0} of {1}";

            //! \todo clean this crap.
            int index = 0;
            //.AsParallel().For
            Parallel.ForEach( LibraryManagement.AllArtists,
                new ParallelOptions {MaxDegreeOfParallelism = 4}, // limit threads
                async (artist) =>
                {
                    GeneralExtensions.DispatcherHelper(() =>
                       LabelStatus.Text = String.Format(artistCoverDownload, index, LibraryManagement.AllArtists.Length));

                    MediaItem item = LibraryManagement.AllArtists[index];
                    await LibraryManagement.CoverDownload(null, Constants.CoverType.Artist);
                    index++;
                });

            index = 0; 
            Parallel.ForEach(LibraryManagement.AllAlbums,
                new ParallelOptions { MaxDegreeOfParallelism = 4 }, // limit threads
                (artist) =>
                {
                    GeneralExtensions.DispatcherHelper(() =>
                     LabelStatus.Text = String.Format(albumCoverDownload, index, LibraryManagement.AllAlbums.Length));

                    MediaItem item = LibraryManagement.AllAlbums[index];
                    LibraryManagement.CoverDownload(null, Constants.CoverType.Album);
                    index++;
                });

            //for ( int index = 0; index < LibraryManagement.AllArtists.Length; index++ )
            //{
            //    GeneralExtensions.DispatcherHelper(() => 
            //      LabelStatus.Text = String.Format(coverDownload, index, LibraryManagement.AllArtists.Length)) ;

            //    MediaItem item = LibraryManagement.AllArtists[index];
            //    LibraryManagement.CoverDownload( new Image(), item.Artist, Constants.CoverType.Artist );

            //    // dunno what this was suposed to do but it crashes. 
            //    //Application.Current.Dispatcher.Invoke(
            //    //DispatcherPriority.Normal,
            //    //(ThreadStart)delegate
            //    //{
            //    //    LibraryManagement.CoverDownload(new Image(), item.Artist, Constants.CoverType.Artist);
            //    //});
            //    // }    
            //}
        }

        private void BuildNavigation()
        {
            const string contentRoot = "/Content";

            Constants.WindowMain.MenuLinkGroups.Clear();
            Constants.WindowMain.TitleLinks.Clear();

            var welcome = new LinkGroup { DisplayName = "welcome" };
            var library = new LinkGroup { DisplayName = "library" };
            var playback = new LinkGroup { DisplayName = "playback" };
            var settings = new LinkGroup { DisplayName = "settings", GroupName = "settings" };

            welcome.Links.Add(new Link
            {
                DisplayName = "All",
                Source = new Uri(contentRoot + "/Home/AlbumPage.xaml?type=all", UriKind.Relative)
            });
            welcome.Links.Add(new Link
            {
                DisplayName = "new",
                Source = new Uri(contentRoot + "/Home/AlbumPage.xaml?type=new", UriKind.Relative)
            });
            welcome.Links.Add(new Link
            {
                DisplayName = "random",
                Source = new Uri(contentRoot + "/Home/AlbumPage.xaml?type=random", UriKind.Relative)
            });
            welcome.Links.Add(new Link
            {
                DisplayName = "most played",
                Source = new Uri(contentRoot + "/Home/AlbumPage.xaml?type=mostplayed", UriKind.Relative)
            });
            welcome.Links.Add(new Link
            {
                DisplayName = "now playing",
                Source = new Uri(contentRoot + "/Home/AlbumPage.xaml?type=nowplaying", UriKind.Relative)
            });

            if (Constants.LoggedIn)
            {
                //for (int i = 0; i < LibraryManagement.Folder.GetLength(0); i++)
                //{
                //    string uri = contentRoot + "/Library/ArtistView.xaml?id=" + i +
                //                 "&name=" + LibraryManagement.Folder[i, 0] + "type=folder";
                //    library.Links.Add(new Link
                //    {
                //        DisplayName = LibraryManagement.Folder[i,0],
                //        Source = new Uri(uri, UriKind.Relative)
                //    });
                //}
            }

            playback.Links.Add(new Link
            {
                DisplayName = "Home",
                Source = new Uri(contentRoot + "/Playback/Main.xaml", UriKind.Relative)
            });

            settings.Links.Add(new Link
            {
                DisplayName = "MetroSonic",
                Source = new Uri("/Pages/Settings.xaml", UriKind.Relative)
            });

            Constants.WindowMain.MenuLinkGroups.Add(welcome);
            Constants.WindowMain.MenuLinkGroups.Add(library);
            Constants.WindowMain.MenuLinkGroups.Add(playback);
            Constants.WindowMain.MenuLinkGroups.Add(settings);

            Constants.WindowMain.TitleLinks.Add(new Link
            {
                DisplayName = "settings",
                Source = new Uri("/Pages/Settings.xaml", UriKind.Relative)
            });
        }
    }
}