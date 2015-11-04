using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using MetroSonic.MediaControl;

namespace MetroSonic
{
    internal static class LastFm
    {
        const string _urlBase = "http://ws.audioscrobbler.com/2.0/?method={0}.{1}&{2}";

        private enum Commands
        {
            GetArtistCover, 
            GetAlbumCover, 
        }

        private enum ImageSizes
        {
            Small,
            Medium, 
            Large, 
            Extralarge, 
        }

        private static XmlDocument ExecuteCommand( string command, MediaItem item )
        {
            var doc = new XmlDocument();
            try
            {
                // encode and send the command
                var encodedCmd = Uri.EscapeUriString( command );
                doc.Load(encodedCmd ); 
            }
            catch (WebException e)
            {
                return null;
            }

            return doc; 
        }

        private static string BuildCommand( string methodGroup, string method, Dictionary <string, string> parameter )
        {
            string concatedParam = string.Empty;
            foreach ( var param in parameter )
            {
                concatedParam += param.Key + "=" + param.Value+ "&";
            }
            concatedParam += "api_key=" + Constants.LastFmApiKey; 

            var cmd = string.Format( _urlBase, methodGroup, method, concatedParam ); 
            return cmd;
        }

        private static string GetImageUrl( ImageSizes size, XmlDocument doc )
        {
            if ( doc == null ) return null; 
            XmlNodeList elemList = doc.GetElementsByTagName("image");
            for (int i = 0; i < elemList.Count; i++)
            {
                if (elemList[i].Attributes == null) continue;
                if (elemList[i].Attributes.Cast<XmlAttribute>().Any(attr => attr.Value == size.ToString().ToLower()))
                {
                    return elemList[i].InnerText;
                }
            }

            return null; 
        }

        internal static string GetLastFmAlbumUrl(MediaItem album)
        {
            /*
            artist (Required (unless mbid)] : The artist name
            album (Required (unless mbid)] : The album name
            mbid (Optional) : The musicbrainz id for the album
            autocorrect[0|1] (Optional) : Transform misspelled artist names into correct artist names, returning the correct version instead. The corrected artist name will be returned in the response.
            username (Optional) : The username for the context of the request. If supplied, the user's playcount for this album is included in the response.
            lang (Optional) : The language to return the biography in, expressed as an ISO 639 alpha-2 code.
            api_key (Required) : A Last.fm API key.
            */
            var param = new Dictionary<string, string>
                    {
                        {"artist", album.ArtistName.Replace("&", "&amp;")},
                        {"album", album.AlbumName},
                        {"autocorrect", "1" }
                    };
            string command = BuildCommand("album", "getInfo", param);
            
            XmlDocument doc = ExecuteCommand( command, album );
            
            var imgUrl = GetImageUrl( ImageSizes.Large, doc );

            return imgUrl; 
        }

        /// <summary>
        /// Gets the url to the artist picture.
        /// </summary>
        /// <param name="coverID"></param>
        /// <returns></returns>
        internal static string GetLastFmArtistUrl(string coverID)
        {
            string url =
                string.Format(
                    "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist={0}&api_key=0b5c899b0a3b089b49b4c7ee5ffb2abc",
                    coverID);

            var doc = new XmlDocument();
            try
            {
                doc.Load(url);
            }
            catch (WebException)
            {
                return null;
            }


            XmlNodeList elemList = doc.GetElementsByTagName("image");
            for (int i = 0; i < elemList.Count; i++)
            {
                if (elemList[i].Attributes == null) continue;
                if (elemList[i].Attributes.Cast<XmlAttribute>().Any(attr => attr.Value == "extralarge"))
                {
                    return elemList[i].InnerText;
                }
            }

            return null;
            //return (from XmlNode node in elemList from XmlAttribute attribute in node.Attributes where attribute.Name == "extralarge" select node).Select(node => node.Value).FirstOrDefault();
        }
    }
}