# TaskManagerApp

This is a basic ASP.NET Core application demonstrating real-time communication using **SignalR**. It includes a working example of a Task Assignment system (or notification system), powered by SignalR.

## 🚀 How to Run the Application

1. **Clone or Download** this repository or extract the provided ZIP file.
   ```bash
   git clone https://github.com/Yozankarki/TaskManagerApp
2. Open the solution file in Visual Studio 2022 or higher.
3. Restore NuGet packages and build the project.
4. Run the project using:
   - Visual Studio’s IIS Express
   - Or dotnet run via terminal in the project folder
   
   <hr/>
**Here is the project demo link**
```bash
https://taskmanagerapp.tryasp.net/
```
## Assumptions Made

- The application runs locally and does not require any external SignalR services.
- SignalR hub is configured with EF Core in-memory no external database needed.
- Task Assigned is seen by all users when it is assigned.

## 📡 SignalR Implementation
The real-time Task Assignment system was implemented in the following steps:

1. **Add SignalR Client Script**
```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/6.0.1/signalr.min.js"></script>
```
2. Configure SignalR in Program.cs
   ```csharp
   builder.Services.AddSignalR();
   app.MapHub<NotificationHub>("/notification");
   ```
3. Create a SignalR Hub
   - NotificationHub.cs
   ```csharp
   using Microsoft.AspNetCore.SignalR;
   namespace TaskManager.Web.Hubs
   {
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
   }
   ```
5. Inject and Use HubContext in Controller
   Inject IHubContext<NotificationHub> in the HomeController constructor:
   ```csharp
   private readonly IHubContext<NotificationHub> _hub;
   public HomeController(ILogger<HomeController> logger, IHubContext<NotificationHub> hub, ApplicationDbContext context)
   {
    _logger = logger;
    _hub = hub;
    _context = context;
   }
   ```
   Call SignalR from your action after a task is created:

   ```csharp
   await _hub.Clients.All.SendAsync("ReceiveNotification", TempData["SuccessMessage"]);
   ```
6. Create SignalR JavaScript File
   Add a JavaScript file (e.g., site.js) and initialize the connection:
   ```csharp
   var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notification")
    .configureLogging(signalR.LogLevel.Information)
    .build();
   connection.on("ReceiveNotification", function (message) {
    toastr.success(message); //this is the message shown as notification
   });
   // Handle connection start
   connection.start()
    .then(function () {
        console.log("SignalR connected.");
    })
    .catch(function (err) {
        console.error("SignalR connection error: ", err.toString());
    });
   ```
## 📸 Screenshot or Demo
When a task is successfully created, a toast notification is displayed in real-time to all connected users without refreshing the page.

<table>
  <tr>
    <td align="center">
      <a href="Images/Initial%20Project%20View.png" target="_blank">
        <img src="Images/Initial%20Project%20View.png" alt="Initial Project View" width="250"/>
      </a>
      <br/>Basic UI of The Project
    </td>
    <td align="center">
      <a href="Images/When%20task%20is%20assigned.png" target="_blank">
        <img src="Images/When%20task%20is%20assigned.png" alt="When task is assigned" width="250"/>
      </a>
      <br/>When Task Is Assigned
    </td>
    <td align="center">
      <a href="Images/Task%20Filter.png" target="_blank">
        <img src="Images/Task%20Filter.png" alt="Task Filter" width="250"/>
      </a>
      <br/>Task Filtering
    </td>
  </tr>
  <tr>
    <td align="center">
      <a href="Images/Deadline%20showcase.png" target="_blank">
        <img src="Images/Deadline%20showcase.png" alt="Deadline Showcase" width="250"/>
      </a>
      <br/>Deadline Showcase
    </td>
    <td align="center">
      <a href="Images/Edit%20View.png" target="_blank">
        <img src="Images/Edit%20View.png" alt="Edit View" width="250"/>
      </a>
      <br/>Edit Assigned Tasks
    </td>
    <td align="center">
      <!-- Empty or add another screenshot here -->
      <em>More Coming Soon...</em>
    </td>
  </tr>
</table>
