using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.DTO
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string FilePath { get; set; }
        public string Album { get; set; }

        [Column(TypeName = "time(3)")]
        public TimeSpan Duration { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? StartPoint { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? MixPoint { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? EndPoint { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? HookInPoint { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? HookOutPoint { get; set; }

    }


}
