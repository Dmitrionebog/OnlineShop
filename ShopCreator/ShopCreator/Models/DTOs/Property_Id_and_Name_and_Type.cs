using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.Models.DTOs
{
    public class Property_Id_and_Name_and_Type_and_Values
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string ValidValues { get; set; }
    }
}
