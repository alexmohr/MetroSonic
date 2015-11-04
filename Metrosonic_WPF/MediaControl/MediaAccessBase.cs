using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MetroSonic.Properties;
using MetroSonic.Utils;
using Image = System.Drawing.Image;

namespace MetroSonic.MediaControl
{
    abstract class MediaAccessBase
    {
        private string _server;
        private object _authentication;
        public abstract bool Login( string username, string password, string server, int port );

        public abstract MediaItem[] GetLibrary();

        public abstract MediaItem[] GetArtists();

        public abstract MediaItem[] GetAllAbums();
        ImageConverter converter = new ImageConverter();

        public string GetCoverArtPath(MediaItem mediaItem, Constants.CoverType coverType)
        {
            string url = null;
            var assemblyPath = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ); 

            string path =  Path.Combine(assemblyPath, "cache");

            switch (coverType)
            {
                case Constants.CoverType.Album:
                    path = Path.Combine( path, "albums" ); 
                    break;
                case Constants.CoverType.Artist:
                    path = Path.Combine(path, "artists");
                    break;
            }
            if ( !path.EndsWith( "\\" ) )
                path = path + "\\"; 

            string fileName = GeneralExtensions.CleanFileName(mediaItem.AlbumName + ".png" );  
            string imagePath = GeneralExtensions.CleanPath(path) + fileName;


            if ( !File.Exists( imagePath ) )
            {
                if ( coverType == Constants.CoverType.Artist )
                    url = LastFm.GetLastFmArtistUrl(mediaItem.ArtistName);
                else url = LastFm.GetLastFmAlbumUrl(mediaItem);

                // last fm does not know this artist 
                if ( string.IsNullOrEmpty( url ) )
                {
                    string backupFile = 
                        GeneralExtensions.CleanPath(path) + nameof(Resources.unknownartist) + ".png";
                    if ( !File.Exists( backupFile ) )
                    {
                        if ( Resources.unknownartist != null )
                        {
                            File.WriteAllBytes(backupFile, 
                                (byte[])converter.ConvertTo(Resources.unknownartist, typeof(byte[])));
                        }
                    }

                    return backupFile; 

                }
                   

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var webClient = new WebClient();

                //webClient.DownloadDataCompleted += delegate(object sender, DownloadDataCompletedEventArgs e)
                //{

                //byte[] graphicData = e.Result;
                byte[] graphicData = webClient.DownloadData(url);
                System.Drawing.Image img = null;
                using (var ms = new MemoryStream(graphicData, 0, graphicData.Length))
                {
                    ms.Write(graphicData, 0, graphicData.Length);
                    try
                    {
                        img = System.Drawing.Image.FromStream(ms, true);
                        img.Save(imagePath, ImageFormat.Png);
                    }
                   
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }

                
            }
            

            return imagePath;
        }

        public abstract MediaItem[] GetTracks( MediaItem album );
        
    }
}
