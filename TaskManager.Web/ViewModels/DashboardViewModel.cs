using TaskManager.Core.Models;

namespace TaskManager.Web.ViewModels
{
    public class DashboardViewModel
    {
        public required string Title { get; set; }
        public required string Description { get; set; }

        public DateTime Deadline { get; set; }

        public  int? CreatedById { get; set; }
        public string? CreatedByName { get; set; }

        public  int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Populated in the controller
        public List<User> Users { get; set; } = new();
        public List<TaskItem> Tasks { get; set; } = new();
    }
}
