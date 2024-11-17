using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ForgeAir.Core.RDS.VariableCollections
{
    static class TextVariables
    {
        public static readonly Dictionary<string, Func<string>> Variables = new Dictionary<string, Func<string>>
        {
            { "_time_", () => VariableCollections.DynamicVariables.Instance.currentTime },
            { "_date_", () => VariableCollections.DynamicVariables.Instance.currentDate },
            { "_track_", () => VariableCollections.DynamicVariables.Instance.currentSong}
        };
    }
}
