using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class AddCategoryViewModel : Screen
    {
        private readonly Importer importer = new();

        private CategoryDTO categoryDTO;
        private readonly IWindowManager _windowManager;

        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryColor { get; set; }


        public AddCategoryViewModel(IWindowManager windowManager)
        {
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
        }
        public void Cancel()
        {
            TryCloseAsync(true);
        }
    }
}
