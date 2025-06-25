using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
namespace ForgeAir.Playout.ViewModels.PlayoutWindows
{
    public class PlayoutPageViewModel : Screen
    {
        private readonly IAudioService _audioService;
        private readonly AppState _appState;

        public PlayoutPageViewModel(IAudioService audioService, AppState appState)
        {
            _audioService = audioService;
            _appState = appState;
        }

        public void Play()
        {
            _audioService.Play();
        }

        public void Pause()
        {
            _audioService.Pause();
        }

        public void Stop()
        {
            _audioService.Stop();
        }
    }
}
