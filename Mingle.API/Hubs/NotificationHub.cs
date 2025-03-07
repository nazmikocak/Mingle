using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class NotificationHub : Hub
    {
        private readonly IUserService _userService;


        private string UserId
        {
            get
            {
                var identity = Context.User!.Identity as ClaimsIdentity;
                return identity!
                    .Claims
                    .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)!
                    .Value;
            }
        }


        public NotificationHub(IUserService userService)
        {
            _userService = userService;
        }


        public override async Task OnConnectedAsync()
        {
            DateTime lastConnectionDate = DateTime.MinValue;
            await _userService.UpdateLastConnectionDateAsync(UserId, lastConnectionDate!);

            await Clients.Others.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, DateTime>> { { UserId, new Dictionary<string, DateTime> { { "lastConnectionDate", lastConnectionDate } } } });
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            DateTime lastConnectionDate = DateTime.UtcNow;
            await _userService.UpdateLastConnectionDateAsync(UserId, lastConnectionDate);

            await Clients.Others.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, DateTime>> { { UserId, new Dictionary<string, DateTime> { { "lastConnectionDate", lastConnectionDate } } } });
            await base.OnDisconnectedAsync(exception);
        }


        public async Task SearchUsers(string query)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(UserId, query);
                await Clients.Caller.SendAsync("ReceiveSearchUsers", new Dictionary<string, object>
                    {
                        {"query", query },
                        {"data", users }
                    }
                );
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException ||
                ex is FirebaseException)
            {
                await Clients.Caller.SendAsync("Error", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }
    }
}