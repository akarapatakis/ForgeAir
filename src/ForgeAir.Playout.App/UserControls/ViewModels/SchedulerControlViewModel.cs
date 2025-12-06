using ForgeAir.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class SchedulerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Core.Models.DailySchedule> Appointments { get; } = new();

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged(nameof(DayAppointments));
            }
        }

        public IEnumerable<DailySchedule> DayAppointments =>
            Appointments.Where(a => a.Date.Date == SelectedDate.Date);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
