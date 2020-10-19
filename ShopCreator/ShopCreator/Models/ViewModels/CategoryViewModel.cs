using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public List<ProductCategory> ProductCategories { get; set; }

        public Category ParentCategory { get; set; }
        public CategoryViewModel()
        {
            ProductCategories = new List<ProductCategory>();
        }

    }
}
