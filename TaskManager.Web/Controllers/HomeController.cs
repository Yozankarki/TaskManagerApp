using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using TaskManager.Core.Data;
using TaskManager.Core.Models;
using TaskManager.Web.Hubs;
using TaskManager.Web.ViewModels;

namespace TaskManager.Web.Controllers
{

    public class HomeController : Controller
    {
        #region declarations

        private readonly IHubContext<NotificationHub> _hub;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IHubContext<NotificationHub> hub, ApplicationDbContext context)
        {
            _logger = logger;
            _hub = hub;
            _context = context;
        }

        #endregion

        #region code base

        public async Task<IActionResult> Index(int? currentUserId)
        {
            var users = await _context.Users.ToListAsync();
            var tasksQuery = _context.Tasks.AsQueryable();

            if (currentUserId.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.AssignedToId == currentUserId);
            }

            var model = new DashboardViewModel
            {
                Title = string.Empty,
                Description = string.Empty,
                Deadline = DateTime.Today,
                CreatedAt = DateTime.UtcNow,
                Users = users,
                Tasks = await tasksQuery.OrderByDescending(t => t.Id).ToListAsync(),
                CurrentUserId = currentUserId
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
                model.Users = _context.Users.ToList();
                model.Tasks = _context.Tasks.ToList();
                return View("Index", model);
            }

            var createdBy = await _context.Users.FindAsync(model.CreatedById);
            var assignedTo = await _context.Users.FindAsync(model.AssignedToId);

            var task = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                Deadline = model.Deadline,
                CreatedById = model.CreatedById,
                CreatedBy = createdBy?.Name,
                AssignedToId = model.AssignedToId,
                AssignedTo = assignedTo?.Name,
                CreatedAt = DateTime.UtcNow,
                Status = TaskStatuss.Pending
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Toastr success message
            TempData["SuccessMessage"] = $"You have been assigned a new task by: {createdBy?.Name}";

            await _hub.Clients.All.SendAsync("ReceiveNotification", TempData["SuccessMessage"]);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                task.Status = TaskStatuss.Completed;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var model = new TaskListViewModel
            {
                Users = await _context.Users.ToListAsync(),
                Task = task
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskListViewModel model)
        {
            var updated = model.Task;
            if (updated == null)
                return BadRequest();

            var task = await _context.Tasks.FindAsync(updated.Id);

            if (task == null)
                return NotFound();

            var createdBy = await _context.Users.FindAsync(updated.CreatedById);
            var assignedTo = await _context.Users.FindAsync(updated.AssignedToId);

            task.Title = updated.Title;
            task.Description = updated.Description;
            task.Deadline = updated.Deadline;
            task.CreatedById = updated.CreatedById;
            task.AssignedToId = updated.AssignedToId;
            task.CreatedBy = createdBy?.Name;
            task.AssignedTo = assignedTo?.Name;
            task.Status = updated.Status;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
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

        #endregion
    }
}
