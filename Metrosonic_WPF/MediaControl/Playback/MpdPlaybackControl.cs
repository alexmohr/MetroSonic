using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libmpc;
using MetroSonic.MediaControl.Playback;

namespace MetroSonic.MediaControl
{
    public class MpdPlaybackControl : PlaybackControlBase
    {
        public MpdPlaybackControl()
        {
            //PlaybackState = MpdAccess.Mpc.playb
        }

        public override MediaItem[] GetCurrentPlaylist()
        {
           // var state = MpdAccess.Mpc.Status();
            List < MpdFile > playlist = MpdAccess.Mpc.PlaylistInfo();
            return MpdAccess.ConvertMpcFileToMediaItem( playlist ).ToArray();
        }

        public override void StopPlayback()
        {
            base.StopPlayback();
            MpdAccess.Mpc.Pause(pause:true);
        }

        public override void StartPlayback()
        {
            base.StartPlayback();
            MpdAccess.Mpc.Pause(pause: false);
            MpdAccess.Mpc.Play();
        }

        public override void Next()
        {
            MpdAccess.Mpc.Next();
        }

        public override void Dispose()
        {
          
        }

        public override void AddToPlaylist(MediaItem item)
        {
            MpdAccess.Mpc.Add(item.URL);
        }

      
    }
}
