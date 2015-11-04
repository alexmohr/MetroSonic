using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroSonic.MediaControl.Playback
{
    public abstract class PlaybackControlBase : IDisposable
    {
        /// <summary>
        /// The streaming playback state.
        /// </summary>
        public enum StreamingPlaybackState
        {
            /// <summary>
            /// The playback is stopped.
            /// </summary>
            Stopped,

            /// <summary>
            /// The playback is playing.
            /// </summary>
            Playing,

            /// <summary>
            /// The playback is buffering. 
            /// </summary>
            Buffering,

            /// <summary>
            /// The playback is paused.
            /// </summary>
            Paused
        }


        public event EventHandler<PlaybackStartEventArgs> PlaybackStarted;
        public event EventHandler<PlaybackUpdateEventArgs> PlaybackStatusUpdate;

        public double StreamLength
        {
            get;
            protected set;
        }

        public StreamingPlaybackState PlaybackState
        {
            get;
            protected set;
        } = StreamingPlaybackState.Stopped;

        public void TogglePlayBack()
        {
            if ( PlaybackState == StreamingPlaybackState.Paused || PlaybackState == StreamingPlaybackState.Playing )
            {
                StopPlayback(); 
            }
            else
            {
                StartPlayback();
            }
        }

        public virtual void StopPlayback()
        {
            PlaybackState = StreamingPlaybackState.Stopped;
        }


        public virtual void StartPlayback()
        {
            PlaybackState = StreamingPlaybackState.Playing;
        }

        public abstract void Dispose();

        protected void InvokePlaybackStatusUpdate(double passedSeconds)
        {
            PlaybackStatusUpdate?.Invoke(this, new PlaybackUpdateEventArgs(passedSeconds));
        }

        protected void InvokePlackbackStarted(MediaItem startedItem)
        {
            PlaybackStarted?.Invoke(this, new PlaybackStartEventArgs(startedItem));
        }

        public abstract void AddToPlaylist( MediaItem item );
        public abstract void Next();
        public abstract MediaItem[] GetCurrentPlaylist();
    }
}
