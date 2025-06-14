using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.RDS.Enums;

namespace ForgeAir.Core.Models
{
    class RDSBlock
    {
        /// <summary>
        /// Determine the type of the block (PS, RT, TA, TP etc)
        /// </summary>
        public required RDSGroupsEnum Group { get; set; }

        /// <summary>
        /// The data that will be sent to the encoder
        /// </summary>
        public required string Data { get; set; }

        /// <summary>
        /// Time to wait until the encoder requests a new block
        /// </summary>
        public TimeSpan? Interval { get; set; }
    }
}
