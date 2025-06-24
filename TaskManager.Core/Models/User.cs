
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
