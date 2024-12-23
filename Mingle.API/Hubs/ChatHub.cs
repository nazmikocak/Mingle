using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Services.Abstract;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
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



        public ChatHub(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }



        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            if (!userCS.ConnectionIds.Contains(connectionId))
            {
                userCS.ConnectionIds.Add(connectionId);
                userCS.LastConnectionDate = null;

                await _userService.SaveConnectionSettingsAsync(UserId, userCS);
            }

            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            if (!userCS.ConnectionIds.Count.Equals(0) && userCS.ConnectionIds.Contains(connectionId))
            {
                userCS.ConnectionIds.Remove(connectionId);
                userCS.LastConnectionDate = DateTime.UtcNow;

                await _userService.SaveConnectionSettingsAsync(UserId, userCS);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}