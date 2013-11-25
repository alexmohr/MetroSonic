// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaybackControl.cs" company="Alexander Mohr">
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//       MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//       GNU General Public License for more details.
//   
//       You should have received a copy of the GNU General Public License
//       along with this program.  If not, see <http://www.gnu.org/licenses/>.
//       MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//       GNU General Public License for more details.
//   
//       You should have received a copy of the GNU General Public License
//       along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The playback control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FirstFloor.ModernUI.Windows.Controls;
using MetroSonic.Content;
using MetroSonic.Pages.Playback;

namespace MetroSonic.MediaControl
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using NAudio.Wave;

    /// <summary>
    /// The playback control.
    /// </summary>
    public class PlaybackControl
    {
        /// <summary>
        /// The playback state.
        /// </summary>
        private volatile StreamingPlaybackState _playbackState;

        /// <summary>
        /// The _buffered wave provider.
        /// </summary>
        private BufferedWaveProvider _bufferedWaveProvider;

        /// <summary>
        /// Shows if the track is completly downloaded.
        /// </summary>
        private volatile bool _fullyDownloaded;

        /// <summary>
        /// Shows if the playback should be canceld.
        /// </summary>
        private bool _keepPlaying;

        /// <summary>
        /// The waveplayer.
        /// </summary>
        private IWavePlayer _waveOut;

        /// <summary>
        /// The thread for decoding and playback.
        /// </summary>
        private Thread _waveThread;

        /// <summary>
        /// Http request to help download the mp3.
        /// </summary>
        private HttpWebRequest _webRequest;

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

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>
        /// The volume.
        /// </value>
        public float Volume
        {
            get { return VolumeProvider.Volume; }
            set { VolumeProvider.Volume = value; }
        }

        /// <summary>
        /// Gets the playback state.
        /// </summary>
        /// <value>
        /// The playback state.
        /// </value>
        public StreamingPlaybackState PlaybackState
        {
            get { return _playbackState; }
        }

        /// <summary>
        /// Gets the volume provider.
        /// </summary>
        /// <value>
        /// The volume provider.
        /// </value>
        public VolumeWaveProvider16 VolumeProvider { get; private set; }

        /// <summary>
        /// The wave out playback stopped.
        /// </summary>
        /// <param name="sender">
        /// The sending object.
        /// </param>
        /// <param name="e">
        /// The stopped event arguemnts.
        /// </param>
        private static void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                new ModernDialog()
                {
                    Content = string.Format("Playback Error {0}", e.Exception.Message),
                    Title = LanguageOutput.Errors.PlaybackErrorTitle
                }.ShowDialog(); 
            }
        }

        /// <summary>
        /// Create the wave player.
        /// </summary>
        /// <returns>
        /// The wave player.
        /// </returns>
        private static IWavePlayer CreateWaveOut()
        {
            return new WaveOut();
        }

        /// <summary>
        /// Disposes resources and stops playback.
        /// </summary>
        internal void Dispose()
        {
            if (_waveThread != null)
                _waveThread.Abort();
        }

        /// <summary>
        /// Toggles the playback.
        /// </summary>
        internal void TogglePlayBack()
        {
            if (_playbackState == StreamingPlaybackState.Stopped
                || _playbackState == StreamingPlaybackState.Paused)
            {
                StartPlayback();
            }
            else
            {
                _playbackState = StreamingPlaybackState.Paused;
                _waveOut.Pause();
            }
        }

        /// <summary>
        /// Starts the playback.
        /// </summary>
        internal void StartPlayback()
        {
            switch (_playbackState)
            {
                case StreamingPlaybackState.Stopped:
                    string trackURL = LibraryManagement.PlaylistGetCurrentTrackURL();
                    _keepPlaying = true;
                    if (trackURL == null) return;

                    _playbackState = StreamingPlaybackState.Buffering;
                    _bufferedWaveProvider = null;
                    
                    ThreadPool.QueueUserWorkItem(StreamMP3, trackURL);
                    _waveThread = new Thread(PlayUpdate);
                    _waveThread.Start();
                    
                    break;
                case StreamingPlaybackState.Paused:
                    _playbackState = StreamingPlaybackState.Buffering;
                    break;
            }
        }

        /// <summary>
        /// Stops the playback.
        /// </summary>
        internal void StopPlayback()
        {
            if (_playbackState == StreamingPlaybackState.Stopped) return;

            if (!_fullyDownloaded)
            {
                _webRequest.Abort();
            }

            _playbackState = StreamingPlaybackState.Stopped;

            if (_waveOut != null)
            {
                _waveOut.Stop();
                _waveOut.Dispose();
                _waveOut = null;
            }
            
            // SetCurrentSongData.ResetAudioBar();

            _keepPlaying = false;
            
            // n.b. streaming thread may not yet have exited
            Thread.Sleep(500);
            ShowBufferState();
        }

        /// <summary>
        /// The stream m p 3.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void StreamMP3(object state)
        {
            _fullyDownloaded = false;
            var url = (string)state;
            _webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)_webRequest.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.RequestCanceled)
                {
                    new ModernDialog()
                    {
                        Content = LanguageOutput.Errors.DownloadFailed,
                        Title = LanguageOutput.Errors.PlaybackErrorTitle
                    }.ShowDialog();
                }

                return;
            }

            var buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame

            IMp3FrameDecompressor decompressor = null;
            try
            {
                using (Stream responseStream = resp.GetResponseStream())
                {
                    var readFullyStream = new ReadFullyStream(responseStream);
                    do
                    {
                        if (_bufferedWaveProvider != null &&
                            _bufferedWaveProvider.BufferLength - _bufferedWaveProvider.BufferedBytes <
                            _bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4)
                        {
                            Thread.Sleep(500);
                        }
                        else
                        {
                            Mp3Frame frame;
                            try
                            {
                                frame = Mp3Frame.LoadFromStream(readFullyStream);
                            }
                            catch (EndOfStreamException)
                            {
                                _fullyDownloaded = true;

                                // reached the end of the MP3 file / stream
                                break;
                            }
                            catch (WebException)
                            {
                                // probably we have aborted download from the GUI thread
                                break;
                            }

                            if (decompressor == null)
                            {
                                // don't think these details matter too much - just help ACM select the right codec
                                // however, the buffered provider doesn't know what sample rate it is working at
                                // until we have a frame
                                WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate,
                                    frame.ChannelMode == ChannelMode.Mono ? 1 : 2, frame.FrameLength, frame.BitRate);
                                decompressor = new AcmMp3FrameDecompressor(waveFormat);
                                _bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat)
                                {
                                    BufferDuration = TimeSpan.FromSeconds(20)
                                };

                                // allow us to get well ahead of ourselves
                                // this.bufferedWaveProvider.BufferedDuration = 250;
                            }

                            if (frame == null) continue;

                            try
                            {
                                int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                                if (_bufferedWaveProvider != null)
                                    _bufferedWaveProvider.AddSamples(buffer, 0, decompressed);
                            }
                            catch (NullReferenceException)
                            {
                                // Stopped Playback
                            }
                        }
                    }
                    while (_playbackState != StreamingPlaybackState.Stopped);

                    if (decompressor != null) decompressor.Dispose();
                }
            }
            finally
            {
                if (decompressor != null)
                {
                    decompressor.Dispose();
                }
            }

            //LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Forward);
            //_playbackState = StreamingPlaybackState.Stopped;
            //StartPlayback();
        }

        /// <summary>
        /// The play update.
        /// </summary>
        private void PlayUpdate()
        {
            while (_keepPlaying)
            {
                if (_playbackState != StreamingPlaybackState.Stopped)
                {
                    if (_waveOut == null && _bufferedWaveProvider != null)
                    {
                        _waveOut = CreateWaveOut();
                        _waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
                        VolumeProvider = new VolumeWaveProvider16(_bufferedWaveProvider) { Volume = Properties.Settings.Default.Volume };
                        _waveOut.Init(VolumeProvider);

                        // progressBarBuffer.Maximum = (int)bufferedWaveProvider.BufferDuration.TotalMilliseconds;
                    }
                    else if (_bufferedWaveProvider != null)
                    {
                        double bufferedSeconds = _bufferedWaveProvider.BufferedDuration.TotalSeconds;
                        ShowBufferState();

                        // make it stutter less if we buffer up a decent amount before playin
                        if (bufferedSeconds < 1 && _playbackState == StreamingPlaybackState.Playing &&
                            !_fullyDownloaded)
                        {
                            _playbackState = StreamingPlaybackState.Buffering;
                            if (_waveOut != null) _waveOut.Pause();
                        }
                        else if (bufferedSeconds > 4 && _playbackState == StreamingPlaybackState.Buffering &&
                                 _waveOut != null)
                        {
                            if (_waveOut == null)
                                return;

                            _waveOut.Play();
                            // _playbackState = StreamingPlaybackState.Playing;
                        }
                        else if (_fullyDownloaded && bufferedSeconds > 1)
                        {
                            NextTrack();
                        }

                        //if (_playbackState == StreamingPlaybackState.Playing)
                        //    SetCurrentSongData.UpdateAudioProgress();
                    }
                }

                Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Skips to the next track.
        /// </summary>
        private void NextTrack()
        {
            StopPlayback();
            LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Forward);
            StartPlayback();
        }

        /// <summary>
        /// The buffer state.
        /// </summary>
        /// Todo: implement status display on form.
        private void ShowBufferState()
        {
            // labelBuffered.Text = String.Format("{0:0.0}s", totalSeconds);
            // progressBarBuffer.Value = (int)(totalSeconds * 1000);
        }

        /*
        //private void KeyDownEvent(object sender, KeyEventArgs e)
        //{
        //    switch (e.KeyCode)
        //    {
        //        case Keys.MediaNextTrack:
        //            LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Forward);
        //            AfterSkip();
        //            break;
        //        case Keys.MediaPreviousTrack:
        //            LibraryManagement.SkipTrack(LibraryManagement.SkipDirection.Backward);
        //            AfterSkip();
        //            break;
        //        case Keys.MediaPlayPause:
        //            TogglePlayBack();
        //            break;
        //        case Keys.MediaStop:
        //            StopPlayback();
        //            break;
        //    }
        //}
        
        //private void AfterSkip()
        //{
        //    StopPlayback();
        //    StartPlayback();
        //}
         */
    }
}