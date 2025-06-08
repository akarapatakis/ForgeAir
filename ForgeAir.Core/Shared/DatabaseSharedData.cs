using ForgeAir.Database;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class DatabaseSharedData
    {
        public List<ForgeAir.Database.Models.Track> tracks;


        public string server { get; set; } = "localhost";
        public int port { get; set; } = 3306;
        public string dbName { get; set; } = "ForgeVision";


        public EventHandler dbModified;
        public void RaiseDBModified() { dbModified?.Invoke(this, EventArgs.Empty); }
        private static DatabaseSharedData? instance;
        public static DatabaseSharedData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseSharedData();
                }
                return instance;
            }
        }
    }
}
