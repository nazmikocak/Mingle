using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;

namespace Mingle.Services.Concrete
{
    /// <summary>
    /// Kullanıcı çağrılarını yöneten servis sınıfıdır.
    /// Çağrı başlatma, kabul etme, sonlandırma ve çağrı geçmişini getirme gibi işlemleri içerir.
    /// </summary>
    public sealed class CallService : ICallService
    {
        private readonly ICallRepository _callRepository;



        /// <summary>
        /// CallService sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="callRepository">Çağrı işlemleri için kullanılan repository.</param>
        public CallService(ICallRepository callRepository)
        {
            _callRepository = callRepository;
        }


        /// <summary>
        /// Yeni bir çağrı başlatır.
        /// </summary>
        /// <param name="userId">Çağrıyı başlatan kullanıcının kimliği.</param>
        /// <param name="recipientId">Çağrının alıcısının kimliği.</param>
        /// <param name="callType">Çağrı türü (sesli veya görüntülü).</param>
        /// <returns>Çağrı kimliği döndürülür.</returns>
        /// <exception cref="BadRequestException">Kullanıcı başka bir çağrıda ise fırlatılır.</exception>
        public async Task<string> StartCallAsync(string userId, string recipientId, CallType callType)
        {
            FieldValidationHelper.ValidateRequiredFields((recipientId, "recipientId"), (callType.ToString(), "callType"));

            var callSnapshot = await _callRepository.GetCallsAsync();

            var userActiveCalls = callSnapshot
                .Where(call =>
                    (call.Object.Participants.Contains(recipientId) || call.Object.Participants.Contains(userId))
                    &&
                    (call.Object.Status.Equals(CallStatus.Ongoing) || call.Object.Status.Equals(CallStatus.Pending))
                );

            var callId = Guid.NewGuid().ToString();

            var call = new Call
            {
                Participants = [userId, recipientId],
                Type = callType,
                Status = userActiveCalls.Count() != 0 ? CallStatus.Declined : CallStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            await _callRepository.CreateOrUpdateCallAsync(callId, call);

            if (userActiveCalls.Count() != 0)
            {
                throw new BadRequestException("Kullanıcı meşgul!");
            }

            return callId;
        }



        /// <summary>
        /// Kullanıcının gelen bir çağrıyı kabul etmesini sağlar.
        /// </summary>
        /// <param name="userId">Çağrıyı kabul eden kullanıcının kimliği.</param>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir <see cref="Task"/> nesnesi.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının çağrı üzerinde yetkisi yoksa fırlatılır.</exception>
        public async Task AcceptCallAsync(string userId, string callId)
        {
            FieldValidationHelper.ValidateRequiredFields((callId, "callId"));

            var call = await _callRepository.GetCallByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

            if (!call.Participants.Contains(userId))
            {
                throw new ForbiddenException("Çağrı üzerinde yetkiniz yok.");
            }

            call.Status = CallStatus.Ongoing;

            await _callRepository.CreateOrUpdateCallAsync(callId, call);
        }



        /// <summary>
        /// Bir çağrıyı sonlandırır.
        /// </summary>
        /// <param name="userId">Çağrıyı sonlandıran kullanıcının kimliği.</param>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <param name="callStatus">Çağrının sonlandırılma durumu.</param>
        /// <param name="createdDate">Çağrının başlama tarihi.</param>
        /// <returns>Çağrı bilgilerini içeren bir sözlük döndürülür.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının çağrı üzerinde yetkisi yoksa fırlatılır.</exception>
        public async Task<Dictionary<string, Call>> EndCallAsync(string userId, string callId, CallStatus callStatus, DateTime? createdDate)
        {
            FieldValidationHelper.ValidateRequiredFields((callId, "callId"), (callStatus.ToString(), "callStatus"));

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



        /// <summary>
        /// Kullanıcının belirli bir çağrıyı silmesini sağlar.
        /// </summary>
        /// <param name="userId">Çağrıyı silen kullanıcının kimliği.</param>
        /// <param name="callId">Silinecek çağrının kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir <see cref="Task"/> nesnesi.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının çağrı üzerinde yetkisi yoksa fırlatılır.</exception>
        public async Task DeleteCallAsync(string userId, string callId)
        {
            FieldValidationHelper.ValidateRequiredFields((callId, "callId"));

            var call = await _callRepository.GetCallByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

            if (!call.Participants.Contains(userId))
            {
                throw new ForbiddenException("Çağrı üzerinde yetkiniz yok.");
            }

            call.DeletedFor.Add(userId, DateTime.UtcNow);

            await _callRepository.CreateOrUpdateCallAsync(callId, call);
        }



        /// <summary>
        /// Belirli bir çağrıya katılan kullanıcıların kimliklerini döndürür.
        /// </summary>
        /// <param name="userId">Kullanıcının kimliği.</param>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <returns>Çağrıya katılan kullanıcıların kimlik listesi.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının çağrı üzerinde yetkisi yoksa fırlatılır.</exception>
        public async Task<List<string>> GetCallParticipantsAsync(string userId, string callId)
        {
            var callParticipants = await _callRepository.GetCallParticipantsByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

            if (!callParticipants.Contains(userId))
            {
                throw new ForbiddenException("Çağrı üzerinde yetkiniz yok.");
            }

            return callParticipants;
        }


        /// <summary>
        /// Kullanıcının çağrı geçmişini getirir.
        /// </summary>
        /// <param name="userId">Kullanıcının kimliği.</param>
        /// <returns>Çağrı geçmişi ve alıcı kimliklerini içeren bir tuple döndürür.</returns>
        public async Task<(Dictionary<string, Dictionary<string, Call>>, List<string>)> GetCallLogs(string userId)
        {
            var callSnapshot = await _callRepository.GetCallsAsync();

            var userCalls = callSnapshot
                .Where(call =>
                    call.Object.Participants.Contains(userId)
                    &&
                    !call.Object.DeletedFor!.ContainsKey(userId)
                )
                .OrderBy(call => call.Object.CreatedDate)
                .ToDictionary(
                    call => call.Key,
                    call => call.Object
                );

            var callRecipientIds = userCalls
                .SelectMany(call => call.Value.Participants)
                .Where(participantId => !participantId.Equals(userId))
                .ToList();



            return (new Dictionary<string, Dictionary<string, Call>>
            {
                { "Call", userCalls },
            },
            callRecipientIds
            );
        }



        /// <summary>
        /// Kullanıcının belirli bir çağrısını getirir.
        /// </summary>
        /// <param name="userId">Çağrıyı sorgulayan kullanıcının kimliği.</param>
        /// <param name="callId">Çağrı kimliği.</param>
        /// <returns>Çağrı nesnesi.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        public async Task<Call> GetCallAsync(string userId, string callId)
        {
            var call = await _callRepository.GetCallByIdAsync(callId) ?? throw new NotFoundException("Çağrı bulunamadı");

            return call;
        }
    }
}