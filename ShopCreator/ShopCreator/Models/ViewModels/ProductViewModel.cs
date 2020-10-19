using System.ComponentModel.DataAnnotations;

namespace ShopCreator.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указано цена")]
        public double Price { get; set; }

        public ViewProductProperty[] PropertiesOfProduct { get; set; }

        public int[] IdsOfCategories { get; set; }


    }


    public class ViewProductProperty
    {
        [Required]
        public int PropertyId { get; set; }
        [Required]
        public string Value { get; set; }
    }
}