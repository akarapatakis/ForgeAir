using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.AudioPlayout.Players.Interfaces
{
    public interface IPlayerFactory
    {
        IPlayer CreatePlayer();
    }
}
