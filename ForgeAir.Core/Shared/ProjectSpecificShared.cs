using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class ProjectSpecificShared
    {
        public ForgeVariationsEnum productVariation { get; set; }

        private static ProjectSpecificShared? instance;
        public static ProjectSpecificShared Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProjectSpecificShared();
                }
                return instance;
            }
        }

    }
}
