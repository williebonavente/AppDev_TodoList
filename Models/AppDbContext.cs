using Microsoft.EntityFrameworkCore;

namespace TodoList.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<TodoItem> ToDoItems { get; set; }
    }
}
