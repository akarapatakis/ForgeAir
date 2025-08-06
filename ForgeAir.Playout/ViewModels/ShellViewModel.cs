using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout;
using ForgeAir.Core.Services.AudioPlayout.Players;
using ForgeAir.Core.Services.Weather;
using ForgeAir.Playout.Models;
using ForgeAir.Playout.ViewModels.Helpers;
using ForgeAir.Playout.ViewModels.PlayoutWindows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors.Core;

namespace ForgeAir.Playout.ViewModels
{
    public class ShellViewModel : Conductor<TabItemViewModelBase>.Collection.OneActive, INotifyPropertyChanged
    {
        public ICommand CloseTabCommand { get; }
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _tempertatureUpdateTimer;
        public TabItemViewModelBase SelectedTab { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private string currentTime;
        public string CurrentTime
        {
            get => currentTime;
            set
            {
                if (currentTime != value)
                {
                    currentTime = value;
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }
        }

        private string currentDate;
        public string CurrentDate
        {
            get => currentDate;
            set
            {
                if ( currentDate != value)
                {
                    currentDate = value;
                    OnPropertyChanged(nameof(CurrentDate));
                }

            }
        }

        private string currentTemp;
        public string CurrentTemp
        {
            get => currentTemp;
            set
            {
                if (currentTemp != value) {
                    currentTemp = value;
                    OnPropertyChanged(nameof(CurrentTemp));
                }
            }
        }
        private Visibility state;
        public Visibility State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        private readonly IServiceProvider _provider;
        public ShellViewModel(IServiceProvider provider)
        {
            _provider = provider;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            _tempertatureUpdateTimer = new DispatcherTimer();
            _tempertatureUpdateTimer.Interval = TimeSpan.FromHours(new Random().Next(2, 4)); // random update hour because we dont want to get a rate limit by the api
            _tempertatureUpdateTimer.Tick += updateTemp_Tick;
            _tempertatureUpdateTimer.Start();
            Task.Run(() => updateTemp_Tick(null, null)); // since the window is launched we need to update the temp right away
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            CloseTabCommand = CloseTabCommand = new RelayCommand<TabItemViewModelBase>(CloseTab);


            var activityCenterVM = _provider.GetRequiredService<ActivityCenterViewModel>();
            activityCenterVM.SetShellViewModel(this);  // pass ShellViewModel reference
            Items.Add(activityCenterVM);
            ActivateItemAsync(Items.First());

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            CurrentDate = DateTime.Now.ToString("yyyy/MM/dd");
        }

        private async void updateTemp_Tick(object sender, EventArgs e)
        {
            var tempService = _provider.GetRequiredService<IWeatherService>();
            if (tempService.CurrentWeather != null) { 
                CurrentTemp = $"{tempService.CurrentWeather.First().ToString()} °C / {tempService.CurrentWeather.Last().ToString()} °F";
            }
        }
        public void OpenTab(TabItemViewModelBase tab)
        {
            // for some reason .Contains() doesn't work so i did it this bad way
            foreach (var item in Items)
            {
                if (item.Title == tab.Title)
                {
                    return;
                }
            }
            Items.Add(tab);
            ActivateItemAsync(tab);
        }

        public void CloseTab(TabItemViewModelBase tab)
        {
            // Don't allow removing the Activity Center tab
            if (tab.Title == "Κέντρο Έναρξης")
                return;

            Items.Remove(tab);

            // Ensure there's always at least one tab open
            if (Items.Count == 0)
            {
                var activityCenterVM = _provider.GetRequiredService<ActivityCenterViewModel>();
                activityCenterVM.SetShellViewModel(this);
                Items.Add(activityCenterVM);

                var firstItem = Items.FirstOrDefault();
                if (firstItem != null)
                    ActivateItemAsync(firstItem);
            }
        }
    }
}
