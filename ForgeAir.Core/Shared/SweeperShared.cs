using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.Shared
{
    public class SweeperShared
    {
        public Track? sweeper { get; set; }

        public Track? targetTrack { get; set; }

        public event EventHandler? updateListUI;

        public void RaiseOnSweeperChanged()
        {

            if (updateListUI != null)
            {
                updateListUI.Invoke(this, EventArgs.Empty);

            }
        }

        private static SweeperShared? instance;
        public static SweeperShared Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SweeperShared();
                }
                return instance;
            }
        }
    }
}
