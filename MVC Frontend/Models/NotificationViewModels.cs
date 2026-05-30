namespace MVC_Frontend.Models;

public class NotificationIndexViewModel
{
    public List<NotificationItemViewModel> Notifications { get; set; } = new();
    public int UnreadCount { get; set; }
}

public class NotificationItemViewModel
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string Type { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = "";
}

public class UnreadNotificationsDto
{
    public int UnreadCount { get; set; }
    public List<NotificationItemViewModel> Recent { get; set; } = new();
}
