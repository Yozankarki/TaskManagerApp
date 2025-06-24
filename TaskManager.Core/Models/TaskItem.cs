namespace TaskManager.Core.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public  string? Title { get; set; }
        public  string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public TaskStatuss Status { get; set; } = TaskStatuss.Pending;
        
        public int? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public int? AssignedToId { get; set; }
        public string? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;

        // Navigation property for User
        public User? User { get; set; }
    }
    public enum TaskStatuss
    {
        Pending,
        InProgress,
        Completed,
        Overdue
    }
}
