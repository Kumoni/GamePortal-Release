using System.ComponentModel.DataAnnotations;

namespace Game.Models
{
    public class Level
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Название уровня обязательно")]
        [Display(Name = "Название уровня")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Сложность")]
        [Range(1, 10, ErrorMessage = "Сложность должна быть от 1 до 10")]
        public int Difficulty { get; set; }
    }
}