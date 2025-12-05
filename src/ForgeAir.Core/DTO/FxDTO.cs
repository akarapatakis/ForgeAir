using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Core.DTO
{
    public class FxDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Color { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }
        public TimeSpan Duration { get; set; }
        public TrackStatus Status { get; set; }

        public static FxDTO FromEntity(FX fx)
        {
            return new FxDTO
            {
                Id = fx.Id,
                Name = fx.Name,
                FilePath = fx.FilePath,
                Color = fx.Color,
                DateAdded = fx.DateAdded,
                DateModified = fx.DateModified,
                DateDeleted = fx.DateDeleted,
                Duration = fx.Duration,
                Status = fx.Status,
            };
        }

        public static FX ToEntity(FxDTO fx)
        {
            return new FX
            {
                Id = fx.Id,
                Name = fx.Name,
                FilePath = fx.FilePath,
                Color = fx.Color,
                DateAdded = fx.DateAdded,
                DateModified = fx.DateModified,
                DateDeleted = fx.DateDeleted,
                Duration = fx.Duration,
                Status = fx.Status,
            };
        }
    }
}
