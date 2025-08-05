using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.Models
{
    public abstract class TabItemViewModelBase : Screen
    {
        public virtual string Title { get; }
        public virtual bool Closeable { get; } = true;
    }

}
