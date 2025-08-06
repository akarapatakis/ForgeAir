using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.Database;
using ForgeAir.Database.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class CategoryManipulatorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CategoryDTO> FetchedDTOCategories { get; } = new();
        private readonly IServiceProvider _provider;
        private readonly RepositoryService<Category> repositoryService;
        public readonly ICollection<Category> FetchedCategories;
        public event PropertyChangedEventHandler PropertyChanged;


        private ObservableCollection<CategoryDTO> selectedCategories = new();
        public ObservableCollection<CategoryDTO> SelectedCategories
        {
            get => selectedCategories;
            set
            {
                if (selectedCategories != value)
                {
                    selectedCategories = value;
                    OnPropertyChanged(nameof(selectedCategories));
                }
            }
        }

        private CategoryDTO selectedCategory = new();
        public CategoryDTO SelectedCategory
        {
            get => selectedCategory;
            set
            {
                if (selectedCategory != value)
                {
                    selectedCategory = value;
                    OnPropertyChanged(nameof(selectedCategory));
                }
            }
        }

        private CategoryDTO selectedAddedCategory = new();
        public CategoryDTO SelectedAddedCategory
        {
            get => selectedAddedCategory;
            set
            {
                if (selectedAddedCategory != value)
                {
                    selectedAddedCategory = value;
                    OnPropertyChanged(nameof(selectedAddedCategory));
                }
            }
        }

        public void Add()
        {
            if (SelectedCategory != null && SelectedCategory.Id != null && SelectedCategory.Name != null && !SelectedCategories.Contains(SelectedCategory))
                SelectedCategories.Add(SelectedCategory);
        }
        public void Remove()
        {
            if (SelectedAddedCategory != null && SelectedCategories.Contains(SelectedAddedCategory))
                SelectedCategories.Remove(SelectedAddedCategory);
        }

        public CategoryManipulatorViewModel(IServiceProvider provider)
        {
            _provider = provider;
            repositoryService = _provider.GetRequiredService<RepositoryService<Category>>();
            FetchedDTOCategories = _provider.GetRequiredService<ObservableCollection<CategoryDTO>>();
            FetchedCategories = _provider.GetRequiredService<ObservableCollection<Category>>();

            FetchCategories();
        }
        protected void OnPropertyChanged(string propName) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


        private async Task FetchCategories()
        {
            FetchedCategories.Clear();
            List<Category> categories = await Task.Run(() => repositoryService.GetAll(Core.Tracks.Enums.ModelTypesEnum.Category));

            foreach (var category in categories)
            {
                FetchedCategories.Add(category);
                FetchedDTOCategories.Add(CategoryDTO.FromEntity(category));
            }
        }
    }

}


