using Game.Models;
using System.ComponentModel.DataAnnotations;

namespace Game.Models
{
    public class Character
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя персонажа обязательно")]
        [Display(Name = "Имя персонажа")]
        public string Name { get; set; }

        [Display(Name = "Класс")]
        public string Class { get; set; }

        [Display(Name = "Уровень")]
        public int LevelId { get; set; }

        [Display(Name = "Игрок")]
        public int PlayerId { get; set; }

        public Level Level { get; set; }
        public Player Player { get; set; }
    }
}