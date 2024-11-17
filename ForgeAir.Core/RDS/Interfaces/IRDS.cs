using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.RDS.Interfaces
{
    public interface IRDS
    {
        public string currentPS => Shared.RDSParams.Instance.currentPS;
        public string currentRT => Shared.RDSParams.Instance.currentRT;

        public int psHoldTimeout => Shared.RDSParams.Instance.psHoldInterval;
        public int rtHoldTimeout => Shared.RDSParams.Instance.rtHoldInterval;

        Task SetPS(string text);
        Task SetRT(string text);
    }
}
