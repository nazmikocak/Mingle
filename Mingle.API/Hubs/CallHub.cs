using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.Entities.Enums;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    /// <summary>
    /// Gerçek zamanlı çağrı işlemlerini yöneten SignalR hub sınıfıdır.
    /// Kullanıcı bağlantılarını, çağrı başlatma, bitirme ve WebRTC sinyalleme işlemlerini yönetir.
    /// </summary>
    [Authorize]
    public sealed class CallHub : Hub
    {
        private readonly IUserService _userService;
        private readonly ICallService _callService;



        /// <summary>
        /// Geçerli kullanıcının kimliğini (UserId) döndürür.
        /// Kullanıcının kimliği, JWT içindeki <see cref="ClaimTypes.NameIdentifier"/> değerinden alınır.
        /// </summary>
        /// <returns>Geçerli kullanıcının benzersiz kimliği.</returns>
        /// <exception cref="NullReferenceException">
        /// Eğer kullanıcı kimliği bulunamazsa veya bir null değer ile karşılaşılırsa fırlatılır.
        /// </exception>
        private string UserId
        {
            get
            {
                var identity = Context?.User?.Identity as ClaimsIdentity;
                return identity?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value!;
            }
        }



        /// <summary>
        /// <see cref="CallHub"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="userService">Kullanıcı işlemleri için <see cref="IUserService"/> bağımlılığı.</param>
        /// <param name="callService">Çağrı işlemleri için <see cref="ICallService"/> bağımlılığı.</param>
        public CallHub(IUserService userService, ICallService callService)
        {
            _userService = userService;
            _callService = callService;
        }



        /// <summary>
        /// Kullanıcı bağlantısı kurulduğunda çağrılır. Kullanıcının önceki çağrı geçmişi ve alıcı profilleri istemciye iletilir.
        /// </summary>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public override async Task OnConnectedAsync()
        {
            var (calls, callRecipientIds) = await _callService.GetCallLogs(UserId);
            var recipientProfiles = await _userService.GetUserProfilesAsync(callRecipientIds);

            await Clients.Caller.SendAsync("ReceiveInitialCalls", calls);
            await Clients.Caller.SendAsync("ReceiveInitialCallRecipientProfiles", recipientProfiles);

            await base.OnConnectedAsync();
        }



        /// <summary>
        /// Kullanıcı bağlantısı kesildiğinde çağrılır. Bağlantının neden kesildiği isteğe bağlı bir <see cref="Exception"/> ile sağlanabilir.
        /// </summary>
        /// <param name="exception">Bağlantının kesilmesine neden olan hata (varsa).</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }



        /// <summary>
        /// Kullanıcılar arasında yeni bir çağrı başlatır ve hem arayan hem de alıcı kullanıcıya çağrı bilgilerini iletir.
        /// </summary>
        /// <param name="recipientId">Çağrının alıcısı olan kullanıcının benzersiz kimliği.</param>
        /// <param name="callType">Çağrı türünü belirtir (sesli veya görüntülü).</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Kullanıcı veya alıcı bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcı çağrıyı başlatmak için yetkili değilse fırlatılır.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task StartCall(string recipientId, CallType callType)
        {
            try
            {
                var callId = await _callService.StartCallAsync(UserId, recipientId, callType);
                List<string> callParticipants = [UserId, recipientId];
                var recipientProfiles = await _userService.GetUserProfilesAsync(callParticipants);

                await Clients.User(UserId).SendAsync("ReceiveOutgoingCall", new Dictionary<string, object>
                    {
                        { "callId", callId },
                        { "callType", callType },
                        { recipientProfiles.Keys.Last(), recipientProfiles.Values.Last() }
                    }
                );

                await Clients.User(recipientId).SendAsync("ReceiveIncomingCall", new Dictionary<string, object>
                    {
                        { "callId", callId },
                        { "callType", callType },
                        { recipientProfiles.Keys.First(), recipientProfiles.Values.First() }
                    }
                );
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Belirtilen çağrıyı kabul eder ve istemciye çağrının kabul edildiğine dair bildirim gönderir.
        /// </summary>
        /// <param name="callId">Kabul edilecek çağrının kimliği.</param>
        /// <returns>Çağrı kabul edildiğinde istemciye geri bildirim gönderir.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz bir istek yapıldığında fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Yetkisiz erişim durumunda fırlatılır.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmeyen bir hata oluştuğunda fırlatılır.</exception>
        public async Task AcceptCall(string callId) 
        {
            try
            {
                await _callService.AcceptCallAsync(UserId, callId);
                await Clients.User(UserId).SendAsync("ReceiveAcceptCall", true);
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Belirtilen çağrıyı sonlandırır ve tüm katılımcılara çağrının sona erdiğini bildirir.
        /// </summary>
        /// <param name="callId">Sonlandırılacak çağrının kimliği.</param>
        /// <param name="callStatus">Çağrının sona erme durumu.</param>
        /// <param name="createdDate">Çağrının oluşturulma tarihi (isteğe bağlı).</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının çağrıyı sonlandırma yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task EndCall(string callId, CallStatus callStatus, DateTime? createdDate)
        {
            try
            {
                var call = await _callService.EndCallAsync(UserId, callId, callStatus, createdDate);
                var callParticipants = call.Values.First().Participants;
                var recipientProfiles = await _userService.GetUserProfilesAsync(callParticipants);

                for (int i = 0; i < callParticipants.Count; i++)
                {
                    var profileToSend = callParticipants[i].Equals(UserId) ? recipientProfiles[callParticipants[1]] : recipientProfiles[UserId];

                    await Clients.User(callParticipants[i]).SendAsync("ReceiveEndCall", new Dictionary<string, object>
                        {
                            { "call", call },
                            { profileToSend.Equals(recipientProfiles[callParticipants[1]]) ? callParticipants[1] : UserId, profileToSend }
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcının belirttiği çağrıyı siler ve istemciye bildirim gönderir.
        /// </summary>
        /// <param name="callId">Silinecek çağrının kimliği.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının çağrıyı silme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task DeleteCall(string callId)
        {
            try
            {
                await _callService.DeleteCallAsync(UserId, callId);
                await Clients.User(UserId).SendAsync("ReceiveDeleteCall", callId);
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// WebRTC bağlantısı için SDP (Session Description Protocol) bilgisini çağrının diğer katılımcısına iletir.
        /// </summary>
        /// <param name="callId">SDP bilgisinin gönderileceği çağrının kimliği.</param>
        /// <param name="sdp">Gönderilecek SDP nesnesi.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task SendSdp(string callId, object sdp)
        {
            try
            {
                var call = await _callService.GetCallAsync(UserId, callId);

                foreach (var participant in call.Participants)
                {
                    if (!participant.Equals(UserId))
                    {
                        await Clients.User(participant).SendAsync("ReceiveSdp", new Dictionary<string, object>
                            {
                                {"sdp", sdp },
                                {"callType", call.Type }
                            }
                        );
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// WebRTC bağlantısı için ICE (Interactive Connectivity Establishment) adaylarını çağrının diğer katılımcısına iletir.
        /// </summary>
        /// <param name="callId">ICE adayının gönderileceği çağrının kimliği.</param>
        /// <param name="iceCandidate">Gönderilecek ICE adayı nesnesi.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Çağrı bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="FirebaseException">Firebase ile ilgili bir hata oluşursa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task SendIceCandidate(string callId, object iceCandidate)
        {
            try
            {
                var call = await _callService.GetCallAsync(UserId, callId);

                foreach (var participant in call.Participants)
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
                await Clients.Caller.SendAsync("Error", new { message = $"Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }
    }
}