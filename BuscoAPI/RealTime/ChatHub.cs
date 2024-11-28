using AutoMapper;
using BuscoAPI.DTOS.Chat;
using BuscoAPI.DTOS.Notification;
using BuscoAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace BuscoAPI.RealTime
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ChatHub(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task ViewChats()
        {
            try
            {
                var userId = int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                //Traigo los ids de los usuarios a los que he enviado o me han enviado un mensaje
                //Si el usuario especificado es el remitente (UserIdSender == userId), selecciona el ID del receptor (UserIdReceiver)
                //Elimino Ids duplicados
                var messageGroups = await context.Messages
                    .AsNoTracking()
                    .Where(m => m.UserIdSender == userId || m.UserIdReceiver == userId)
                    .ToListAsync();

                var messages = messageGroups
                    .GroupBy(m => m.UserIdSender == userId ? m.UserIdReceiver : m.UserIdSender)
                    .Select(g => g.OrderByDescending(m => m.Id).FirstOrDefault())
                    .OrderByDescending(m => m.DateAndTime);

                var usersId = messages
                    .Select(m => m.UserIdSender == userId ? m.UserIdReceiver : m.UserIdSender)
                    .ToList();

                var users = await context.Users
                    .Where(u => usersId.Contains(u.Id))
                    .AsNoTracking()
                    .ToListAsync();

                List<Chat> chats = new List<Chat>();


                foreach (var m in messages)
                {
                    chats.Add(new Chat
                    {
                        lastMessage = m,
                        user = users.FirstOrDefault(u => u.Id == m.UserIdSender || u.Id == m.UserIdReceiver)
                    });
                }

                var chatsMapped = mapper.Map<List<ChatDTO>>(chats);
                await Clients.User(userId.ToString()).SendAsync("Chats", chatsMapped);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        public async Task GetMessagesWithUser(int toUserId)
        {
            var userId = int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var messages = await context.Messages
                .Where(m => (m.UserIdSender == userId || m.UserIdReceiver == userId)
                    && (m.UserIdSender == toUserId || m.UserIdReceiver == toUserId)
                )
                .ToListAsync();

            await Clients.User(userId.ToString()).SendAsync("Messages", messages);
        }

        public async Task SendMessage(int userIdReceiver, string message, string date)
        {
            var userId = int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            var userIds = new List<string> { userIdReceiver.ToString(), userId.ToString() };

            var newMessage = new Message
            {
                UserIdSender = userId,
                UserIdReceiver = userIdReceiver,
                Text = message,
                DateAndTime = DateTime.Parse(date),
                UserSender = new User { Username = userName }
            };

            context.Messages.Add(newMessage);
            await context.SaveChangesAsync();

            await Clients.User(userIdReceiver.ToString()).SendAsync("ReceiveMessageNotification", newMessage);
            await Clients.Users(userIds).SendAsync("ReceiveMessage", newMessage);
        }

        public async Task SendProposal(NotificationCreationDTO notificationDTO)
        {
            var userId = int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

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
