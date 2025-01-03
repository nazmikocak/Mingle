using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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
            var (calls, callRecipientIds) = await _callService.GetCallLogs(UserId);
            var recipientProfiles = await _userService.GetRecipientProfilesAsync(callRecipientIds);

            await Clients.Caller.SendAsync("ReceiveInitialCalls", calls);
            await Clients.Caller.SendAsync("ReceiveInitialCallRecipientProfiles", recipientProfiles);

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


        public async Task StartCall(string recipientId, CallType callType)
        {
            try
            {
                var callId = await _callService.StartCallAsync(UserId, recipientId, callType);

                var senderProfile = await _userService.GetUserProfileAsync(UserId);
                var recipientProfile = await _userService.GetUserProfileAsync(recipientId);

                await Clients.User(UserId).SendAsync("ReceiveOutgoingCall", new Dictionary<string, object> { { "callId", callId }, { "callType", callType }, { recipientId, recipientProfile } });
                await Clients.User(recipientId).SendAsync("ReceiveIncomingCall", new Dictionary<string, object> { { "callId", callId }, { "callType", callType }, { UserId, senderProfile } });
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
                var call = await _callService.EndCallAsync(UserId, callId, callStatus);

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


        public async Task SendSignal(string callId, object signalData)
        {
            try
            {
                var participants = await _callService.GetCallParticipantsAsync(UserId, callId);

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


        public async Task SendIceCandidate(string callId, object iceCandidate)
        {
            try
            {
                var participants = await _callService.GetCallParticipantsAsync(UserId, callId);

                foreach (var participant in participants)
                {
                    if (!participant.Equals(UserId))
                    {
                        await Clients.User(participant).SendAsync("ReceiveIceCandidate", iceCandidate);
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


        public async Task SendSdp(string callId, object sdp)
        {
            try
            {
                var participants = await _callService.GetCallParticipantsAsync(UserId, callId);

                foreach (var participant in participants)
                {
                    if (!participant.Equals(UserId))
                    {
                        await Clients.User(participant).SendAsync("ReceiveSdp", sdp);
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
    }
}