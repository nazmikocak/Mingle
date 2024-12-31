using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class CallHub : Hub
    {

    }
}