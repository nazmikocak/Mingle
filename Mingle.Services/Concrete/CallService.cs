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


        public CallService(ICallRepository callRepository, IChatRepository chatRepository)
        {
            _callRepository = callRepository;
            _chatRepository = chatRepository;
        }


        public async Task<List<string>> StartCallAsync(string userId, string chatId, CallType callType)
        {
            var chatParticipants = await _chatRepository.GetChatParticipantsByIdAsync("Individual", chatId) ?? throw new NotFoundException("Sohbet bulunamadı");

            if (!chatParticipants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            var callId = Guid.NewGuid().ToString();

            var call = new Call
            {
                Participants = [userId, chatParticipants.SingleOrDefault(x => !x.Equals(userId))],
                Type = callType,
                Status = CallStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            await _callRepository.CreateOrUpdateCallAsync(callId, call);

            return chatParticipants;
        }


        public async Task<Dictionary<string, Call>> EndCallAsync(string callId, CallStatus callStatus)
        {
            var call = await _callRepository.GetCallByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

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