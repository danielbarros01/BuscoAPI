using AutoMapper;
using BuscoAPI.DTOS.Notification;
using BuscoAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BuscoAPI.RealTime
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public NotificationHub(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task SendNotification(NotificationCreationDTO notificationDTO)
        {
            var userId = int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var notification = mapper.Map<Notification>(notificationDTO);
            notification.UserSenderId = userId;

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            await Clients.User(notificationDTO.UserReceiveId.ToString()).SendAsync("ReceiveNotification", notification);
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"User connected: {userId}");

            return base.OnConnectedAsync();
        }
    }
}
