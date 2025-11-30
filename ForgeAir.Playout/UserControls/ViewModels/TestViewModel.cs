using Caliburn.Micro;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class TestViewModel : Screen
    {
        private readonly IAudioService _audioService;

        private float[] _waveform;
        public float[] Waveform
        {
            get => _waveform;
            set
            {
                _waveform = value;
                NotifyOfPropertyChange(() => Waveform);
            }
        }

        public TestViewModel(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            Waveform = _audioService.GetWaveformPCM(800);
        }
    }
}
