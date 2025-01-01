using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Response;
using Mingle.Services.Exceptions;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class CallHub : Hub
    {
        private readonly IUserService _userService;
        private readonly ICallService _callService;


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


        public CallHub(IUserService userService, ICallService callService)
        {
            _userService = userService;
            _callService = callService;
        }



        public override async Task OnConnectedAsync()
        {


            await base.OnConnectedAsync();
        }



        public async Task SendSignal(string chatId, object signalData)
        {
            try
            {
                var participants = await _callService.GetCallParticipantsAsync(UserId, chatId);

                for (int i = 0; i < participants.Count; i++)
                {
                    if (!participants[i].Equals(UserId))
                    {
                        await Clients.User(participants[i]).SendAsync("ReceiveSignal", signalData);
                    }
                }
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task StartCall(string chatId, CallType callType)
        {
            try
            {
                var participants = await _callService.StartCallAsync(UserId, chatId, callType);

                var senderProfile = await _userService.GetUserProfileAsync(participants[0]);
                var recipientProfile = await _userService.GetUserProfileAsync(participants[1]);

                for (int i = 0; i < participants.Count; i++)
                {
                    var profileToSend = participants[i].Equals(UserId) ? recipientProfile : senderProfile;

                    await Clients.User(participants[i]).SendAsync("ReceiveCall", new Dictionary<string, CallerUser>
                    {
                        { profileToSend.Equals(recipientProfile) ? participants[1] : UserId, profileToSend }
                    }
                    );
                }
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }


        public async Task EndCall(string callId, CallStatus callStatus)
        {
            try
            {
                var call = await _callService.EndCallAsync(callId, callStatus);

                foreach (var participant in call.Values.First().Participants)
                {
                    await Clients.User(participant).SendAsync("ReceiveEndCall", call);
                }
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu: {ex.Message}" });
            }
        }
    }
}