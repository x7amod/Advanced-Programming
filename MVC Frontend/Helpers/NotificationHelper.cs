using Web_API.Models;

namespace MVC_Frontend.Helpers;

public static class NotificationHelper
{
    public static async Task CreateAsync(
        TrainingInstituteDBContext context,
        string userId,
        string title,
        string message,
        string type,
        string? relatedEntityType = null)
    {
        context.Notifications.Add(new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedEntityType = relatedEntityType,
            IsRead = false,
            CreatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();
    }
}
