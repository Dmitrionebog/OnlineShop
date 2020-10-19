using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.Models
{
    public class PropertiesOfProduct
    {
        public int PropertyId { get; set; }
        public string Value { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }

        public FileModel Image { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
        public List<ProductProperty> ProductProperties { get; set; }
        public Product()
        {
            ProductCategories = new List<ProductCategory>();
            ProductProperties = new List<ProductProperty>();

        }
           
    }
}



//Products (ProductId)
//ProductPropties(ProductId, PropertyId, PropertyValue)
//Properties (Id, Name, Type)