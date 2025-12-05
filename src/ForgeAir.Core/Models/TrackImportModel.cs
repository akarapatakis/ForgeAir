using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database.Models.Enums;


namespace ForgeAir.Core.Models
{
    public class TrackImportModel : INotifyPropertyChanged
    {
        private ImportTrackStatusEnum _importStatus;
        public ImportTrackStatusEnum ImportStatus
        {
            get => _importStatus;
            set
            {
                _importStatus = value;
                OnPropertyChanged(nameof(ImportStatus));
            }
        }

        private ImportTrackErrorsEnum _importErrors;
        public ImportTrackErrorsEnum ImportErrors
        {
            get => _importErrors;
            set
            {
                _importErrors = value;
                OnPropertyChanged(nameof(ImportErrors));
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string FilePath { get; }

        public TrackType TrackType { get; }
        public string? OverrideArtistString { get; } = null;

        private ObservableCollection<CategoryDTO>? _categories;
        public ObservableCollection<CategoryDTO>? Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public string? StreamDisplayTitle { get; set; } = null;
        public TimeSpan CrossfadeTime { get; }
        public TrackImportModel(string filepath, string? StreamdisplayTitle, TrackType type, TimeSpan crossfadeTime, ObservableCollection<CategoryDTO> categories, bool isVideo = false, string? artistString = null) {
            FilePath = filepath;
            TrackType = type;
            Categories = categories;
            if (type == TrackType.Rebroadcast) { StreamDisplayTitle = StreamdisplayTitle; }
            OverrideArtistString = artistString;
            CrossfadeTime = crossfadeTime;

        }
    }
}
