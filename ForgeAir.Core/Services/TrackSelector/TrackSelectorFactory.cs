using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.TrackSelector
{
    public class TrackSelectorFactory
    {
        private readonly IServiceProvider _provider;

        public TrackSelectorFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ITrackSelector Create(TrackSelectionMode mode)
        {
            return mode switch
            {
                TrackSelectionMode.Clock => _provider.GetRequiredService<ClockTrackSelector>(),
                TrackSelectionMode.Random => _provider.GetRequiredService<RandomTrackSelector>(),
                TrackSelectionMode.Manual => _provider.GetRequiredService<ManualTrackSelector>(),
                TrackSelectionMode.ML => _provider.GetRequiredService<MLTrackSelector>(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
