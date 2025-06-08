using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.RDS.Helpers
{
    public class BlockManager
    {
        BlockManager()
        {
            Shared.RDSParams.Instance.psHoldTimer = new System.Timers.Timer();
            Shared.RDSParams.Instance.psHoldTimer.Interval = Shared.RDSParams.Instance.psHoldInterval;

            Shared.RDSParams.Instance.rtHoldTimer = new System.Timers.Timer();
            Shared.RDSParams.Instance.rtHoldTimer.Interval = Shared.RDSParams.Instance.rtHoldInterval;

            Shared.RDSParams.Instance.psBlocks = null;
            Shared.RDSParams.Instance.rtBlocks = null;
        }

        public void LoadPSBlocks() 
        {

            Shared.RDSParams.Instance.psBlocks = new string[File.ReadAllLines(@"config_rds/psblocks").Length];
            foreach (string line in File.ReadLines(@"config_rds/psblocks"))
            {
                Shared.RDSParams.Instance.psBlocks.Append(line);
            }
        }

        public void LoadRTBlocks()
        {

            Shared.RDSParams.Instance.rtBlocks = new string[File.ReadAllLines(@"config_rds/rtblocks").Length];
            foreach (string line in File.ReadLines(@"config_rds/rtblocks"))
            {
                Shared.RDSParams.Instance.rtBlocks.Append(line);
            }
        }
    }
}