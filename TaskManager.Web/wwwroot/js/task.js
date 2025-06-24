// SignalR Connection
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notification")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start()
    .then(() => console.log("Connected to SignalR"))
    .catch(err => console.error("SignalR Connection Error: ", err));

connection.on("ReceiveNotification", (message) => {
    console.log("Notification received:", message);
    alert(message);
});