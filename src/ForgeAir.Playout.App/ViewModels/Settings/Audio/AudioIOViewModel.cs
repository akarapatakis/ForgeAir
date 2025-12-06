using Caliburn.Micro;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.DeviceManager;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeAir.Playout.App.ViewModels.Settings.Audio
{
    public class AudioIOViewModel : Screen
    {

        public ObservableCollection<string> AudioEngines { get; } = new() { "NAudio", "BASS" };
        public string SelectedAudioEngine { get; set; }

        public ObservableCollection<InputDevice> InputDevices { get; set; }
        public string SelectedInputDevice { get; set; }

        public ObservableCollection<OutputDevice> OutputDevices { get; set; }
        public string SelectedOutputDevice { get; set; }


        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AudioIOViewModel(IServiceProvider serviceProvider, IWindowManager windowManager)
        {
            // test 

            SelectedAudioEngine = "NAudio";
            
            #if WINDOWS
            OutputDevices = new ObservableCollection<OutputDevice>(NAudioManager.ListDevicesByAPI_Ex(Core.AudioEngine.Enums.DeviceOutputMethodEnum.MME));
            #endif

        }
    }

}
