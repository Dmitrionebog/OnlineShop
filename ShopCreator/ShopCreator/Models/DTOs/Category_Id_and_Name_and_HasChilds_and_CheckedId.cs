using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.Models.DTOs
{
    public class Category_Id_and_Name_and_HasChilds_and_CheckedId
    {
        
            public int Id { get; set; }
            public string Name { get; set; }
            public bool HasChilds { get; set; }

            public int CheckedId { get; set; }
        
    }
}
