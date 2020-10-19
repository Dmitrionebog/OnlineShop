using ShopCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.DBInitializer
{
    public class DBInitializer
    {
        public static void Initialize(DataContext context)
        {
            if (!context.Products.Any())
            {

                context.Products.AddRange(
                    new Product
                    {
                        Name = "iPhone 6S",
                        Price = 100
                        
                    

                    },
                    new Product
                    {
                        Name = "Samsung Galaxy Edge",
                        Price = 150


                    },
                    new Product
                    {
                        Name = "Lumia 950",
                        Price = 50


                    }
                );
            }

            if (!context.Categories.Any())
            {
                var c1 = new Category { Name = "Category 1" };
                var c2 = new Category { Name = "Category 2", };
                context.Categories.AddRange(
                   c1,
                   c2
                   );


                context.SaveChanges();
            }

            if (!context.Properties.Any())
            {
                var prop1 = new Property {Name ="Property 1",PropertyType = PropertyType.FloatNumberRange , ValidValues = "13.5"  };
                var prop2 = new Property { Name = "Property 2", PropertyType = PropertyType.Choice };
                context.Properties.AddRange(
                   prop1,
                   prop2
                   );


                context.SaveChanges();
            }




            if (!context.ProductCategories.Any())
            {
                var p1 = context.Products.FirstOrDefault(p => p.Name == "Lumia 950");
                var c1 = context.Categories.FirstOrDefault();
                if (p1 != null && c1 != null)
                    context.ProductCategories.Add(
                       new ProductCategory() { CategoryId = c1.Id, ProductId = p1.Id });



                context.SaveChanges();
            }

            if (!context.ProductProperties.Any())
            {
                var p1 = context.Products.FirstOrDefault(p => p.Name == "Lumia 950");
                var prop1 = context.Properties.FirstOrDefault();
                if (p1 != null && prop1 != null)
                    context.ProductProperties.Add(
                       new ProductProperty() { PropertyId = prop1.Id, ProductId = p1.Id });



                context.SaveChanges();
            }

        }
    }

}


