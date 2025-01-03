using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;

namespace Mingle.Services.Concrete
{
    public class CallService : ICallService
    {
        private readonly ICallRepository _callRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;


        public CallService(ICallRepository callRepository, IChatRepository chatRepository, IUserRepository userRepository)
        {
            _callRepository = callRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }


        public async Task<string> StartCallAsync(string userId, string recipientId, CallType callType)
        {
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


        public async Task<Dictionary<string, Call>> EndCallAsync(string userId, string callId, CallStatus callStatus)
        {
            var call = await _callRepository.GetCallByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

            if (!call.Participants.Contains(userId))
            {
                throw new ForbiddenException("Çağrı üzerinde yetkiniz yok.");
            }

            call.Status = callStatus;
            call.CallDuration = DateTime.UtcNow - call.CreatedDate;

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
    }
}