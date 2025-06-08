using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Shared
{
    public class DatabaseConnectionProperties
    {


        public string serverPort { get; set; }
        public string password { get; set; }
        public string dbName { get; set; }

        private static DatabaseConnectionProperties? instance;
        public static DatabaseConnectionProperties Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseConnectionProperties();
                }
                return instance;
            }
        }
    }
}
