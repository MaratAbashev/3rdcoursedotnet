using Grpc.Core;

namespace NotificationService.Services;

public class UserNotificationService(ILogger<UserNotificationService> logger): NotificationService.UserNotificationService.UserNotificationServiceBase
{
    public override Task<UserEventResponse> NotifyUserEvent(UserEventRequest request, ServerCallContext context)
    {
        var dateTime = DateTime.Now;
        var message = $"Event Type: {request.EventType}, UserId: {request.UserId} at {dateTime}";
        logger.LogInformation(message);
        return Task.FromResult(new UserEventResponse
        {
            EventType = request.EventType,
            UserId = request.UserId,
            Message = message,
            Timestamp = dateTime.ToShortTimeString()
        });
    }
}