using TaskManager.Core.Models;

namespace TaskManager.Web.ViewModels
{
    public class TaskListViewModel
    {
        public List<User> Users { get; set; } = new();
        public List<TaskItem> Tasks { get; set; } = new();
        // For editing a single task
        public TaskItem? Task { get; set; }
    }
}
