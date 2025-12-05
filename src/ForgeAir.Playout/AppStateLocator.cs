using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAir.Playout
{
    public static class AppStateLocator
    {
        public static ServiceCollection Current { get; set; } = null!;
    }

}
