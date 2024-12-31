using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
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
            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            await Clients.Others.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, ConnectionSettings> { { UserId, userCS } });
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            await Clients.Others.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, ConnectionSettings> { { UserId, userCS } });
            await base.OnDisconnectedAsync(exception);
        }
    }
}