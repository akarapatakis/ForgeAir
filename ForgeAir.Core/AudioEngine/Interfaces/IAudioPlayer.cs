using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine.Interfaces
{
    public interface IAudioPlayer
    {
        Task Play();
        void PlayNext();
        void Stop();
        void FadeStop();
        void Pause();
        void FadeNext();
        void Next();
        void Restart();
    }
}
