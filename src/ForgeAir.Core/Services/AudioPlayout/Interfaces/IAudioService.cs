using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.AudioPlayout.Interfaces
{
    public interface IAudioService
    {
        float[] GetWaveformPCM(int targetPoints = 800);
        void Initialize();
        void Stop();
        void Play(bool skipToNextTrack = false);
        void Pause();
        void Next();

        Task<TimeSpan> RemainingTime();
        Task<TimeSpan> ElapsedTime();

        float[] GetLevels();
    }
}
