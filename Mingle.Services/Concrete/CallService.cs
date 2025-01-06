using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;

namespace Mingle.Services.Concrete
{
    public sealed class CallService : ICallService
    {
        private readonly ICallRepository _callRepository;


        public CallService(ICallRepository callRepository)
        {
            _callRepository = callRepository;
        }


        public async Task<string> StartCallAsync(string userId, string recipientId, CallType callType)
        {
            FieldValidationHelper.ValidateRequiredFields((recipientId, "recipientId"), (callType.ToString(), "callType"));

            var callId = Guid.NewGuid().ToString();

            var call = new Call
            {
                Participants = [userId, recipientId],
                Type = callType,
                Status = CallStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            await _callRepository.CreateOrUpdateCallAsync(callId, call);

            return callId;
        }


        public async Task<Dictionary<string, Call>> EndCallAsync(string userId, string callId, CallStatus callStatus, DateTime? createdDate)
        {
            FieldValidationHelper.ValidateRequiredFields((callId, "callId"), (callStatus.ToString(), "callStatus"), (createdDate.ToString(), "createdDate"));

            var call = await _callRepository.GetCallByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

            if (!call.Participants.Contains(userId))
            {
                throw new ForbiddenException("Çağrı üzerinde yetkiniz yok.");
            }

            call.Status = callStatus;
            call.CallDuration = DateTime.UtcNow - createdDate;

            await _callRepository.CreateOrUpdateCallAsync(callId, call);

            return new Dictionary<string, Call> { { callId, call } };
        }


        public async Task<List<string>> GetCallParticipantsAsync(string userId, string callId)
        {
            var callParticipants = await _callRepository.GetCallParticipantsByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

            if (!callParticipants.Contains(userId))
            {
                throw new ForbiddenException("Çağrı üzerinde yetkiniz yok.");
            }

            return callParticipants;
        }


        public async Task<(Dictionary<string, Call>, List<string>)> GetCallLogs(string userId)
        {
            var calls = await _callRepository.GetCallsAsync();

            var userCalls = calls
                .Where(call => call.Object.Participants.Contains(userId))
                .ToDictionary(call =>
                    call.Key,
                    call => call.Object
                );

            var callRecipientIds = userCalls
                .SelectMany(call => call.Value.Participants)
                .Where(participantId => !participantId.Equals(userId))
                .ToList();

            return (userCalls, callRecipientIds);
        }
    }
}