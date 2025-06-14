using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class DatabaseCredentials
    {
        public required string DatabaseName { get; set; }
        public string? DatabasePassword { get; set; } = string.Empty;
        public required string server { get; set; }
        public required int port { get; set; }
    }
}
