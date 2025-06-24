using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using TaskManager.Core.Models;
using TaskManager.Web.Hubs;
using TaskManager.Web.ViewModels;

namespace TaskManager.Web.Controllers
{

    public class HomeController : Controller
    {
        private static List<User> Users = new() {
            new User {Id = 1 , Name = "Admin"},
            new User {Id = 2 , Name = "User1"},
            new User {Id = 3 , Name = "User2"}
        };

        private static List<TaskItem> TaskList = new();

        private readonly IHubContext<NotificationHub> _hub;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHubContext<NotificationHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                Title = string.Empty,
                Description = string.Empty,
                Deadline = DateTime.Today,
                CreatedAt = DateTime.UtcNow,

                // Populate the users and tasks
                Users = Users,
                Tasks = TaskList.OrderByDescending(t => t.Id).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DashboardViewModel model)
        {
            if (model.Deadline.Date < DateTime.Today)
            {
                ModelState.AddModelError("Deadline", "Deadline cannot be in the past.");
            }

            if (!ModelState.IsValid)
            {
                model.Users = Users;
                model.Tasks = TaskList;
                return BadRequest(model);
            }

            var createdBy = Users.FirstOrDefault(u => u.Id == model.CreatedById);
            model.CreatedByName = createdBy?.Name;

            var assignedTo = Users.FirstOrDefault(u => u.Id == model.AssignedToId);
            model.AssignedToName = assignedTo?.Name;

            var task = new TaskItem
            {
                Id = TaskList.Count + 1,
                Title = model.Title,
                Description = model.Description,
                Deadline = model.Deadline,
                CreatedBy = model.CreatedByName,
                CreatedById = model.CreatedById,
                AssignedToId = model.AssignedToId,
                AssignedTo = model.AssignedToName,
                CreatedAt = DateTime.UtcNow
            };

            TaskList.Add(task);
           // await _hub.Clients.All.SendAsync("ReceiveNotification", $"New task created: {model.Title}");
            await _hub.Clients.All.SendAsync("ReceiveNotification", $"You have been assigned a new task by: {model.CreatedByName }");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            var task = TaskList.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.Status = TaskStatuss.Completed;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var task = TaskList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            var model = new TaskListViewModel
            {
                Users = Users,
                Task = task
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(TaskListViewModel model)
        {
            var updated = model.Task;
            if (updated == null)
                return BadRequest();

            var task = TaskList.FirstOrDefault(t => t.Id == updated.Id);
            if (task == null)
                return NotFound();

            task.Title = updated.Title;
            task.Description = updated.Description;
            task.Deadline = updated.Deadline;
            task.CreatedById = updated.CreatedById;
            task.AssignedToId = updated.AssignedToId;
            task.CreatedBy = Users.FirstOrDefault(u => u.Id == updated.CreatedById)?.Name;
            task.AssignedTo = Users.FirstOrDefault(u => u.Id == updated.AssignedToId)?.Name;
            task.Status = updated.Status;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var task = TaskList.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                TaskList.Remove(task);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
