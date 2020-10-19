using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }


        public List<ProductCategory> ProductCategories { get; set; }

        //public Category ParentCategory { get; set; }

        public List<Category> ChildCategories { get; set; }
        public Category()
        {
            ProductCategories = new List<ProductCategory>();
        }

    }
}
