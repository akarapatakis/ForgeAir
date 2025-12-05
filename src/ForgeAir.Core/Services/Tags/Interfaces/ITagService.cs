using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Tags.Interfaces
{
    public interface ITagService
    {
        public string Title { get; }
        public DateTime? ReleaseDate { get; }
        public string Comment { get; }
        public string Album { get; }
        public string[]? Gernes { get; }
        public string? ISRC { get; }

        public TimeSpan AudioDuration { get; }
        public int BPM { get; }

        // will add more once i finish tagservice rewrite
    }
}
