using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class NotificationHub : Hub
    {
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


        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserId);

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}