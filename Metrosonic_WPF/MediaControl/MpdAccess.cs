using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Libmpc;

namespace MetroSonic.MediaControl
{
    internal class MpdAccess : MediaAccessBase
    {
        private static Mpc _mpc;
        //private Libmpc.Mpc = new 
        public MpdAccess()
        {
            _mpc = new Libmpc.Mpc();


        }


        public static Mpc Mpc => _mpc;

        private IPEndPoint ParseIPEndPoint(string text)
        {
            Uri uri;
            if (Uri.TryCreate(text, UriKind.Absolute, out uri))
                return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
            if (Uri.TryCreate(String.Concat("tcp://", text), UriKind.Absolute, out uri))
                return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
            if (Uri.TryCreate(String.Concat("tcp://", String.Concat("[", text, "]")), UriKind.Absolute, out uri))
                return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
            throw new Exception("Failed to parse text to IPEndPoint");
        }

        public override bool Login(string username, string password, string server, int port)
        {
            var address =
                Dns.GetHostAddresses(server).First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            _mpc.Connection = new MpcConnection(new IPEndPoint(address, port));

            //GetLib();
            return _mpc.Connection.Connected;
        }

        public override MediaItem[] GetLibrary()
        {
            var x = _mpc.ListAll("/");
            var element = _mpc.LsInfo(x[0]);

            List<MediaItem> item = new List<MediaItem>();
            var ni = new MediaItem();
            ni.IsDir = true;


            item.Add(ni);

            return item.ToArray();
        }

        public override MediaItem[] GetArtists()
        {
            var allItems = _mpc.ListAllInfo("/");

            List<MediaItem> media = new List<MediaItem>();
            Dictionary<string, string> albums = new Dictionary<string, string>();
            foreach (MpdFile item in allItems)
            {
                if (item.HasArtist && !albums.ContainsKey(item.Artist))
                {
                    albums.Add(item.Artist, null);
                    var mi = new MediaItem();
                    mi.ArtistName = item.Artist;
                    mi.Id = item.Id;
                    media.Add(mi);
                }
            }



            return media.ToArray();
        }

        public override MediaItem[] GetAllAbums()
        {
            var allItems = _mpc.ListAllInfo("/");

            List<MediaItem> media = new List<MediaItem>();
            Dictionary<string, string> albums = new Dictionary<string, string>();
            foreach (MpdFile item in allItems)
            {
                if (item.HasAlbum && !albums.ContainsKey(item.Album))
                {
                    albums.Add(item.Album, null);
                    var mi = new MediaItem();
                    mi.AlbumName = item.Album;
                    mi.ArtistName = item.Artist;
                    mi.Id = item.Id;
                    media.Add(mi);
                }
            }



            return media.ToArray();
        }

        private static int _replaceId = -100; 

        public static MediaItem[] ConvertMpcFileToMediaItem(IEnumerable<MpdFile> files)
        {
            return files.Select( delegate( MpdFile item )
            {
                var mi = new MediaItem
                {
                    AlbumName = item.Album,
                    Id = item.Id,
                    TrackName = item.Title,
                    ArtistName = item.Artist,
                    URL = item.File,
                    FileName = item.Name,
                    TrackDuration = TimeSpan.FromSeconds( item.Time ),
                };

                if ( mi.Id < 1 )
                {
                    _replaceId--;
                    mi.Id = _replaceId; 
                }

                return mi; 
            } ).ToArray(); 

        }

        public override MediaItem[] GetTracks(MediaItem album = null)
        {
            // Monster linq :D 
            return ConvertMpcFileToMediaItem(_mpc.ListAllInfo("/")
                .Where(x => x.HasAlbum
                            && (null == album || x.Album.ToLower() == album.AlbumName.ToLower())
                            && x.HasTrack
                            && x.HasTime
                            && x.HasTitle));
        }
    }
}
