using AutoMapper;
using BuscoAPI.DTOS;
using BuscoAPI.DTOS.Notification;
using BuscoAPI.Entities;
using BuscoAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public NotificationsController(ApplicationDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet(Name = "GetNotifications")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<Notification>>> Get([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var user = await GetEntity.GetUser(HttpContext, context);
                if (user == null) { return Unauthorized(); }

                var queryable = context.Notifications
                    .Where(n => n.UserReceiveId == user.Id)
                    .Include(n => n.UserSender)
                    .Include(n => n.Proposal)
                    .OrderByDescending(n => n.DateAndTime)
                    .AsQueryable();

                await HttpContext.InsertPageParameters(queryable, pagination.NumberRecordsPerPage);

                var notifications = await queryable.Paginate(pagination).ToListAsync();

                var mappedNotifications = mapper.Map<List<NotificationDTO>>(notifications);

                return Ok(mappedNotifications);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error 500: An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
