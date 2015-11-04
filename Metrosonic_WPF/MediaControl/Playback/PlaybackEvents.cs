using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroSonic.MediaControl.Playback
{
    public class PlaybackStartEventArgs
    {
        public PlaybackStartEventArgs(MediaItem item)
        {
            Item = item;
        }
        public MediaItem Item { get; private set; } // readonly
    }

    public class PlaybackUpdateEventArgs
    {
        public PlaybackUpdateEventArgs(double passedSecounds)
        {
            PassedSecounds = passedSecounds;
        }
        public double PassedSecounds { get; private set; }
    }
}
