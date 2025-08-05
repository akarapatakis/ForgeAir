using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks;
using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class AddCategoryViewModel : Screen
    {
        private readonly Importer importer;

        private CategoryDTO categoryDTO;
        private readonly IWindowManager _windowManager;

        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryColor { get; set; }


        public AddCategoryViewModel(IServiceProvider provider, IWindowManager windowManager)
        {
            importer = new Importer(provider.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>());

            _windowManager = windowManager;

        }

        public void Add()
        {
            categoryDTO = new CategoryDTO
            {
                Color = CategoryColor,
                Description = CategoryDescription,
                Name = CategoryName
            };
            importer.AddCategory(categoryDTO);
            TryCloseAsync(true);
        }
        public void Cancel()
        {
            TryCloseAsync(true);
        }
    }
}
