using ManagedBass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Helpers
{
    public static class BassCheatsheet
    {
        public static string LastErrorToDetailedError(Errors error)
        {
            // waiting for managedbass to release an updated version that contains my changes for description attributes
            // https://github.com/ManagedBass/ManagedBass/pull/137
            return GeneralHelpers.GetEnumDescription((Errors)error);
        }
    }
}
