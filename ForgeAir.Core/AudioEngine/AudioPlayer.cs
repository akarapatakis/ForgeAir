using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFmpeg.AutoGen;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.Models;
using ForgeAir.Core.Shared;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Core.WebEncoder;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharpDX.Direct3D9;
using SharpDX.Win32;
using Track = ForgeAir.Database.Models.Track;

namespace ForgeAir.Core.AudioEngine
{

    // TODO: GET RID OF 333MS AND ADD CUSTOMIZABLE DURATION!

    public class AudioPlayer : IDisposable
    {
        private readonly ForgeAir.Core.AudioEngine.TagReader _tagReader;
        private readonly WebEncoder.NowPlaying _nowPlayingWeb;
        private readonly Pilots.RandomPilot _randomPilot;
        private readonly VSTEffectManager _vstEffect;
        private int _trackHandle;
        private CancellationTokenSource _crossfadeCts = new();
        private static readonly TaskCompletionSource<bool> _playbackCompletion = new();
        private bool willPlaySweeper = false;
        public bool isPaused = false;
        public bool isPlaying = false;
        public AudioPlayer()
        {
            _vstEffect = new VSTEffectManager();
            _randomPilot = new Pilots.RandomPilot();
            _tagReader = new TagReader();
            _nowPlayingWeb = new WebEncoder.NowPlaying();

            AudioPlayerShared.Instance.onTrackChanged += OnTrackChanged;
            AudioPlayerShared.Instance.onQueueChanged += OnQueueChanged;


        }


        /* todo: rewrite that piece of shit because it fucks cpu and the queue
         * 
         * thats why you shouldnt drink vodka while coding :(
         */

        public async Task FillQueue()
        {
            while (true)
            {
                var track = await Task.Run(() => _randomPilot.selectRandomTrack());
                if (track == null)
                {
                    break;
                }
                if (!(track == AudioPlayerShared.Instance.currentTrack)) {
                    if (File.Exists(track.FilePath))
                    {
                        AudioPlayerShared.Instance.trackQueue.EnqueueAtBottom(track);
                        if (track.Intro != null && !track.containsVideoTrack)
                        {
                            Database.Models.Track sweeper;
                            while (true && (AudioPlayerShared.Instance.currentTrack != track)) // if the track has started playing, no need to run anymore
                            {
                                var newSweeper = await Task.Run(() => _randomPilot.selectRandomSweeper());
                                if (newSweeper == null || newSweeper.TrackStatus == TrackStatus.Disabled ||
                                    newSweeper.TrackType != Database.Models.Enums.TrackType.Sweeper ||
                                    newSweeper.Duration > track.Intro)
                                {

                                    SweeperShared.Instance.sweeper = null;
                                    SweeperShared.Instance.targetTrack = null;
                                    willPlaySweeper = false;
                                    await Task.Delay(500);
                                    break;
                                }
                                else
                                {
                                    sweeper = newSweeper;
                                    SweeperShared.Instance.sweeper = newSweeper;
                                    SweeperShared.Instance.targetTrack = track;
                                    willPlaySweeper = true;
                                    SweeperShared.Instance.RaiseOnSweeperChanged();
                                    break;
                                }
                            } // no clue why the cpu usage dropped from 20% to 0.7% lol
                        }

                        if (AudioPlayerShared.Instance.trackQueue.Any())
                        {
                            AudioPlayerShared.Instance.RaiseOnQueueChanged();
                        }
                        return;
                    }
                    else { await Task.Delay(500); continue; }
                }
                else { await Task.Delay(500); continue; }
            }
        }

        private async void OnQueueChanged(object sender, EventArgs e)
        {
            if (!AudioPlayerShared.Instance.trackQueue.Any())
            {
                await FillQueue();
            }
        }



        // get a better look at this because it is acting up when it is called by a trackbar
        public void SeekTo(TimeSpan time)
        {
        }




        public async Task PlayFX(Database.Models.FX fx)
        {
        }

        public async Task Play()
        {

        }

        public void PauseResumeHandler()
        {
        }
        public void Pause()
        {
        }

        public void Resume()
        {
        }
        private async Task MonitorPlayback()
        {

        }

        private async Task PlayNextTrack()
        {
        }

        public void Stop()
        {
        }

        private async Task UpdateMetadata() // add handling
        {
            if (AudioPlayerShared.Instance.currentTrack == null)
            {
                AudioPlayerShared.Instance.currentAlbumArt = null;

                WebEncoderNowPlaying.Instance.nowPlayingText = "";
                new ButtEncoder().UpdateNowPlayingText();
                return;
            }
            if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
            {
                AudioPlayerShared.Instance.currentAlbumArt = null;

                WebEncoderNowPlaying.Instance.nowPlayingText = "";
                new ButtEncoder().UpdateNowPlayingText();
                return;
            }
            else
            {
                SendNewCover();

                WebEncoderNowPlaying.Instance.nowPlayingText = await Task.Run(() => _nowPlayingWeb.CreateString());
                // new ButtEncoder().UpdateNowPlayingText();
                new TextOutputEncoder().UpdateNowPlayingText();
                return;
            }
        }

        public async Task HookSweeperNowAsync(Database.Models.Track sweeper, Database.Models.Track targetTrack)
        {
            if (targetTrack != AudioPlayerShared.Instance.currentTrack)
            {
                return;
            }
        }
        private void SendNewCover()
        {
            var picture = _tagReader.GetPicture(AudioPlayerShared.Instance.currentTrack)?.Data.Data;
            if (picture != null)
            {
                AudioPlayerShared.Instance.currentAlbumArt = new MemoryStream(picture);
            }
            else
            {
                AudioPlayerShared.Instance.currentAlbumArt = null;
            }
        }
        private void OnTrackChanged(object sender, EventArgs e)
        {
            _ = UpdateMetadata();
        }

        public void Dispose()
        {
            AudioPlayerShared.Instance.onTrackChanged -= OnTrackChanged;
            AudioPlayerShared.Instance.onQueueChanged -= OnQueueChanged;

            _crossfadeCts.Dispose();
        }

        private void FadeOut()
        {
        }

        private void FadeIn()
        {
        }
    }
}
