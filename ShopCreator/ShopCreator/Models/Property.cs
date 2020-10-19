using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.Models { 

 
public class Property
    {
    
        public int Id { get; set; }
        public string Name { get; set; }

        public PropertyType PropertyType { get; set;}

        public string ValidValues { get; set; }
               

        public List<ProductProperty> ProductProperties { get; set; }
        public Property()
        {
            ProductProperties = new List<ProductProperty>();

        }

        public List<SelectListItem> GetPropertyTypes { get; } = Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>().
                  Select(p => new SelectListItem() { Text = p.ToString(), Value = ((int)p).ToString() }).ToList();


    }


   public enum PropertyType
   {
        NotDefined = 0,
        StringMinMaxLength = 1,
        IntegerNumberRange = 2,
        FloatNumberRange = 3,
        Choice = 4,
        Boolean = 5
   }
}
