using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Tracks.Enums
{
    public enum ImportTrackErrorsEnum
    {
        NoError = 0,
        TrackAlreadyExists = 1,
        TrackFileNotFound = 2,
        DbError = 3,
        UnknownError = 4
    }
}
