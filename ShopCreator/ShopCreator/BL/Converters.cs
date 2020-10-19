using ShopCreator.Controllers;
using ShopCreator.Models;
using ShopCreator.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.BL
{
    public class Converters
    {
        public static void ConvertToDBProperty(PropertyViewModel propertyViewModel, Property dbProperty)
        {
            dbProperty.Id = propertyViewModel.Id;
            dbProperty.Name = propertyViewModel.Name;
            dbProperty.PropertyType = (ShopCreator.Models.PropertyType)propertyViewModel.PropertyType;
            dbProperty.ValidValues = propertyViewModel.ValidValues;
        }
        
        public static void ConvertToDBProduct(ProductViewModel productViewModel, Product dbProduct)
        {
            dbProduct.Id = productViewModel.Id;
            dbProduct.Name = productViewModel.Name;
            dbProduct.Price = productViewModel.Price;
            
            dbProduct.ProductProperties = new List<ProductProperty>();

            if (productViewModel.PropertiesOfProduct != null)
            {
                foreach (var pp in productViewModel.PropertiesOfProduct)

                {

                    var prpr = new ProductProperty();
                    prpr.Value = pp.Value;
                    prpr.PropertyId = pp.PropertyId;
                    prpr.ProductId = productViewModel.Id;
                    dbProduct.ProductProperties.Add(prpr);
                }
            }

            dbProduct.ProductCategories = new List<ProductCategory>();
            if (productViewModel.IdsOfCategories != null)
            {
                foreach (var idOfCategory in productViewModel.IdsOfCategories)

                {

                    var prcat = new ProductCategory();
                    prcat.ProductId = dbProduct.Id;
                    prcat.CategoryId = idOfCategory;
                    dbProduct.ProductCategories.Add(prcat);
                }
            }


        }
    }
}
