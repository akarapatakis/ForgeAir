using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Timers;

namespace ForgeAir.Core.RDS
{
    public class RDS
    {
        public RDS()
        {

        }

        public void UpdatePS(string text) { 
            Shared.RDSParams.Instance.currentPS = text;
            Shared.RDSParams.Instance.RaisePSChanged();
        }
        public void UpdateRT(string text) {
            Shared.RDSParams.Instance.currentRT = text;
            Shared.RDSParams.Instance.RaiseRTChanged();
        }
    }

}
