using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MetroSonic.Content;

namespace MetroSonic.Utils
{
    internal static class GeneralExtensions
    {
        public static string CleanFileName( string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static string CleanPath(string path)
        {
            return Path.GetInvalidPathChars().Aggregate(path, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static void DispatcherHelper( Action dispatchThis )
        {
            // application has no dispatcher anymore > ui thread has ended.
            if ( Application.Current?.Dispatcher == null )
            {
                return; 
            }

            Application.Current.Dispatcher.Invoke( DispatcherPriority.Normal,
                (ThreadStart)delegate
                {
                    dispatchThis(); 
                });
        }
    }
}