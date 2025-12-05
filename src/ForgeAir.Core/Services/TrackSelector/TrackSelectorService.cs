using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class TrackSelectorService
    {
        private readonly TrackSelectorFactory _factory;

        public ITrackSelector CurrentSelector { get; private set; }

        public TrackSelectorService(TrackSelectorFactory factory)
        {
            _factory = factory;
            CurrentSelector = factory.Create(TrackSelectionMode.Random); //todo: make this configurable
        }

        public void Change(TrackSelectionMode mode)
        {
            CurrentSelector = _factory.Create(mode);
        }
    }

}
