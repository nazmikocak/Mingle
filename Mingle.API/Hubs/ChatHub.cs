using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using Mingle.Shared.DTOs.Request;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    /// <summary>
    /// Gerçek zamanlı sohbet işlemlerini yöneten SignalR hub sınıfıdır.
    /// Kullanıcı bağlantılarını, sohbet başlatma, mesaj gönderme, sohbetleri listeleme ve grup işlemleri gibi işlemleri yönetir.
    /// </summary>
    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageService _messageService;
        private readonly IGroupService _groupService;
        private readonly IChatService _chatService;
        private readonly IUserService _userService;



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
        /// <see cref="ChatHub"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="messageRepository">Mesaj işlemleri için <see cref="IMessageRepository"/> bağımlılığı.</param>
        /// <param name="messageService">Mesaj işlemleri için <see cref="IMessageService"/> bağımlılığı.</param>
        /// <param name="groupService">Grup işlemleri için <see cref="IGroupService"/> bağımlılığı.</param>
        /// <param name="chatService">Sohbet işlemleri için <see cref="IChatService"/> bağımlılığı.</param>
        /// <param name="userService">Kullanıcı işlemleri için <see cref="IUserService"/> bağımlılığı.</param>
        public ChatHub(IMessageRepository messageRepository, IMessageService messageService, IGroupService groupService, IChatService chatService, IUserService userService)
        {
            _messageRepository = messageRepository;
            _messageService = messageService;
            _groupService = groupService;
            _chatService = chatService;
            _userService = userService;
        }



        /// <summary>
        /// Kullanıcı hub'a bağlandığında tetiklenen metod.
        /// Kullanıcıya ait tüm sohbetler, grup profilleri ve alıcı profilleri çekilir ve istemciye iletilir.
        /// </summary>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }



        /// <summary>
        /// Kullanıcı hub'dan ayrıldığında tetiklenen metod.
        /// </summary>
        /// <param name="exception">Bağlantı sırasında oluşan hata (varsa).</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


        public async Task Test() 
        {
            var chatsTask = _chatService.GetAllChatsAsync(UserId);
            var (chats, chatsRecipientIds, userGroupIds) = await chatsTask;

            var recipientProfilesTask = _userService.GetRecipientProfilesAsync(chatsRecipientIds);
            var groupProfilesTask = _groupService.GetGroupProfilesAsync(userGroupIds);

            var recipientProfiles = await recipientProfilesTask;
            var groupProfiles = await groupProfilesTask;

            var sendTasks = new[]
            {
                Clients.Caller.SendAsync("ReceiveInitialChats", chats),
                Clients.Caller.SendAsync("ReceiveInitialGroupProfiles", groupProfiles),
                Clients.Caller.SendAsync("ReceiveInitialRecipientChatProfiles", recipientProfiles),
            };

            await Task.WhenAll(sendTasks);
        }


        /// <summary>
        /// Yeni bir sohbet başlatır ve katılımcılara sohbet bilgilerini iletir.
        /// </summary>
        /// <param name="chatType">Sohbet tipi ("Individual" veya "Group").</param>
        /// <param name="recipientId">Sohbete katılacak alıcının kimliği.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task CreateChat(string chatType, string recipientId)
        {
            try
            {
                var chat = await _chatService.CreateChatAsync(UserId, chatType, recipientId);

                if (chatType.Equals("Individual"))
                {
                    var chatParticipants = chat.Values.Select(x => x.Participants).First();

                    foreach (var participant in chatParticipants)
                    {
                        await Clients.User(participant).SendAsync("ReceiveCreateChat", new Dictionary<string, Dictionary<string, Chat>> { { "Individual", chat } });
                    }

                    var recipientProfiles = await _userService.GetRecipientProfilesAsync(chatParticipants);

                    for (int i = 0; i < chatParticipants.Count; i++)
                    {
                        var profileToSend = chatParticipants[i].Equals(UserId) ? recipientProfiles[recipientId] : recipientProfiles[UserId];

                        await Clients.User(chatParticipants[i]).SendAsync("ReceiveRecipientProfiles", new Dictionary<string, object>
                            {
                                { profileToSend.Equals(recipientProfiles[recipientId]) ? recipientId : UserId, profileToSend }
                            }
                        );
                    }
                }
                else
                {
                    var groupParticipants = await _groupService.GetGroupParticipantsAsync(UserId, chat.Values.First().Participants.First());

                    foreach (var participant in groupParticipants)
                    {
                        await Clients.User(participant).SendAsync("ReceiveCreateChat", new Dictionary<string, Dictionary<string, Chat>> { { "Group", chat } });
                    }
                }
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Belirli bir sohbetin içeriğini temizler.
        /// </summary>
        /// <param name="chatType">Sohbet tipi ("Individual" veya "Group").</param>
        /// <param name="chatId">Temizlenecek sohbetin kimliği.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task ClearChat(string chatType, string chatId)
        {
            try
            {
                var chat = await _chatService.ClearChatAsync(UserId, chatType, chatId);
                await Clients.User(UserId).SendAsync("ReceiveClearChat", chat);
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Belirli bir sohbeti arşivler.
        /// </summary>
        /// <param name="chatId">Arşivlenecek sohbetin kimliği.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task ArchiveChat(string chatId)
        {
            try
            {
                var archivedFor = await _chatService.ArchiveIndividualChatAsync(UserId, chatId);
                await Clients.User(UserId).SendAsync("ReceiveArchiveChat", archivedFor);
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Belirli bir sohbeti arşivden çıkarır.
        /// </summary>
        /// <param name="chatId">Arşivden çıkarılacak sohbetin kimliği.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task UnarchiveChat(string chatId)
        {
            try
            {
                var archivedFor = await _chatService.UnarchiveIndividualChatAsync(UserId, chatId);
                await Clients.User(UserId).SendAsync("ReceiveUnarchiveChat", archivedFor);
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcıdan gelen mesajı belirtilen sohbetin katılımcılarına gönderir.
        /// </summary>
        /// <param name="chatType">Sohbet tipi ("Individual" veya "Group").</param>
        /// <param name="chatId">Mesajın gönderileceği sohbetin kimliği.</param>
        /// <param name="dto">Gönderilen mesajı temsil eden DTO.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet veya mesaj verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task SendMessage(string chatType, string chatId, SendMessage dto)
        {
            try
            {
                var calculator = new PackageSizeCalculationHelper();
                calculator.CalculateMessageSizeAndPacketEstimate(dto);

                var (message, chatParticipants) = await _messageService.SendMessageAsync(UserId, chatId, chatType, dto);

                var saveMessageTask = _messageRepository.CreateMessageAsync(UserId, chatType, chatId, message.First().Value.First().Value.First().Key, message.First().Value.First().Value.First().Value);

                foreach (var participant in chatParticipants)
                {
                    await Clients.User(participant).SendAsync("ReceiveGetMessages", message);
                }

                await saveMessageTask;
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }


        
        /// <summary>
        /// Bir mesajın teslim edildiğini işaretler ve sohbet katılımcılarına bildirir.
        /// </summary>
        /// <param name="chatType">Sohbet tipi ("Individual" veya "Group").</param>
        /// <param name="chatId">Mesajın teslim edileceği sohbetin kimliği.</param>
        /// <param name="messageId">Teslim edilecek mesajın kimliği.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet veya mesaj verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task DeliverMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var (message, chatParticipants) = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Delivered");

                var saveMessageTask = _messageRepository.UpdateMessageStatusAsync(chatType, chatId, message.First().Value.First().Value.First().Key, "Delivered", message.First().Value.First().Value.First().Value.Status.Delivered);

                foreach (var participant in chatParticipants)
                {
                    await Clients.User(participant).SendAsync("ReceiveGetMessages", message);
                }

                await saveMessageTask;
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Bir mesajın okunduğunu işaretler ve sohbet katılımcılarına bildirir.
        /// </summary>
        /// <param name="chatType">Sohbet tipi ("Individual" veya "Group").</param>
        /// <param name="chatId">Mesajın okunacağı sohbetin kimliği.</param>
        /// <param name="messageId">Okunacak mesajın kimliği.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet veya mesaj verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task ReadMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var (message, chatParticipants) = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Read");

                var saveMessageTask = _messageRepository.UpdateMessageStatusAsync(chatType, chatId, message.First().Value.First().Value.First().Key, "Read", message.First().Value.First().Value.First().Value.Status.Read);

                foreach (var participant in chatParticipants)
                {
                    await Clients.User(participant).SendAsync("ReceiveGetMessages", message);
                }

                await saveMessageTask;
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Belirtilen mesajı siler ve sohbet katılımcılarına bildirir.
        /// </summary>
        /// <param name="chatType">Sohbet tipi ("Individual" veya "Group").</param>
        /// <param name="chatId">Silinecek mesajın bulunduğu sohbetin kimliği.</param>
        /// <param name="messageId">Silinecek mesajın kimliği.</param>
        /// <param name="deletionType">Silme türünü belirtir ("0" veya "1").</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Sohbet veya mesaj verisi bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının bu işlemi gerçekleştirme yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task DeleteMessage(string chatType, string chatId, string messageId, byte deletionType)
        {
            try
            {
                var (message, chatParticipants) = await _messageService.DeleteMessageAsync(UserId, chatType, chatId, messageId, deletionType);

                var saveMessageTask = _messageRepository.UpdateMessageDeletedForAsync(chatType, chatId, messageId, message.Values.First().Values.First().Values.First().DeletedFor!);

                foreach (var participant in chatParticipants)
                {
                    await Clients.User(participant).SendAsync("ReceiveGetMessages", message);
                }

                await saveMessageTask;
            }
            catch (Exception ex) when (
                ex is NotFoundException ||
                ex is BadRequestException ||
                ex is ForbiddenException)
            {
                await Clients.Caller.SendAsync("ValidationError", new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }
    }
}