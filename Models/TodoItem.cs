using System.ComponentModel.DataAnnotations;
namespace TodoList.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        public bool IsCompleted { get; set;}

        [Required]
        public required string Category { get; set; }

        [Range(1, 5)]
        public int Priority { get; set; } // 1 = highest, 5 = lowest 

    }
}
