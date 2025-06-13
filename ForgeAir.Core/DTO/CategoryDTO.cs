﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.DTO
{
    public class CategoryDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public string? Color { get; set; }

        public static CategoryDTO FromEntity(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentId = category.ParentId,
                Color = category.Color
            };
        }

        public static Category ToEntity(CategoryDTO dto)
        {
            return new Category
            {
                Id = dto.Id ?? 0,
                Name = dto.Name,
                Description = dto.Description,
                ParentId = dto.ParentId,
                Color = dto.Color
            };
        }
    }
}
