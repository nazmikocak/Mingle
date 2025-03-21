using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;


namespace Mingle.Services.Concrete
{
    /// <summary>
    /// Sohbet işlemlerini yöneten servis sınıfıdır.
    /// Bireysel ve grup sohbetlerinin oluşturulması, görüntülenmesi ve arşivlenmesi gibi işlemleri içerir.
    /// </summary>
    public sealed class ChatService : IChatService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;



        /// <summary>
        /// ChatService sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="groupRepository">Grup yönetimi için kullanılan repository.</param>
        /// <param name="chatRepository">Sohbet yönetimi için kullanılan repository.</param>
        /// <param name="userRepository">Kullanıcı yönetimi için kullanılan repository.</param>
        public ChatService(IGroupRepository groupRepository, IChatRepository chatRepository, IUserRepository userRepository)
        {
            _groupRepository = groupRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }



        /// <summary>
        /// Yeni bir sohbet oluşturur.
        /// </summary>
        /// <param name="userId">Sohbeti başlatan kullanıcının kimliği.</param>
        /// <param name="chatType">Sohbet türü (bireysel veya grup).</param>
        /// <param name="recipientId">Sohbetin alıcısının kimliği.</param>
        /// <returns>Yeni oluşturulmuş sohbeti içeren bir sözlük.</returns>
        /// <exception cref="NotFoundException">Kullanıcı veya grup bulunamadığında fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz sohbet türü verildiğinde fırlatılır.</exception>
        public async Task<Dictionary<string, Chat>> CreateChatAsync(string userId, string chatType, string recipientId)
        {
            FieldValidationHelper.ValidateRequiredFields((chatType, "chatType"), (recipientId, "recipientId"));

            if (chatType.Equals("Individual"))
            {
                var user = _userRepository.GetUserByIdAsync(recipientId) ?? throw new NotFoundException("Kullanıcı bulunamadı.");

                var chatsSnapshot = await _chatRepository.GetChatsAsync(chatType);

                var oldChat = chatsSnapshot
                    .Where(chat =>
                        chat.Object.Participants.Contains(userId)
                        &&
                        chat.Object.Participants.Contains(recipientId)
                    )
                    .ToDictionary(
                        chat => chat.Key,
                        chat => chat.Object
                    );

                if (oldChat.Count.Equals(0))
                {
                    string chatId = Guid.NewGuid().ToString();

                    var newchat = new Chat
                    {
                        Participants = [userId, recipientId],
                        CreatedDate = DateTime.UtcNow,
                    };

                    await _chatRepository.CreateChatAsync(chatType, chatId, newchat);

                    return new Dictionary<string, Chat> { { chatId, newchat } };
                }

                return oldChat;
            }
            else if (chatType.Equals("Group"))
            {
                var group = await _groupRepository.GetGroupByIdAsync(recipientId) ?? throw new NotFoundException("Grup bulunamadı.");

                var chatsSnapshot = await _chatRepository.GetChatsAsync(chatType);

                var oldChat = chatsSnapshot
                    .Where(chat =>
                        chat.Object.Participants.Contains(recipientId)
                    )
                    .ToDictionary(
                        chat => chat.Key,
                        chat => chat.Object
                    );

                if (oldChat.Count.Equals(0))
                {
                    string chatId = Guid.NewGuid().ToString();

                    var newchat = new Chat
                    {
                        Participants = [recipientId],
                        CreatedDate = DateTime.UtcNow,
                    };

                    await _chatRepository.CreateChatAsync(chatType, chatId, newchat);

                    return new Dictionary<string, Chat> { { chatId, newchat } };
                }

                return oldChat;
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }
        }



        /// <summary>
        /// Kullanıcının tüm sohbetlerini getirir.
        /// </summary>
        /// <param name="userId">Kullanıcının kimliği.</param>
        /// <returns>Kullanıcının bireysel ve grup sohbetlerini içeren bir sözlük.</returns>
        public async Task<(Dictionary<string, Dictionary<string, Chat>>, List<string>, List<string>)> GetAllChatsAsync(string userId)
        {
            var individualChatsTask = _chatRepository.GetChatsAsync("Individual");
            var groupChatsTask = _chatRepository.GetChatsAsync("Group");
            var groupsTask = _groupRepository.GetAllGroupAsync();

            await Task.WhenAll(individualChatsTask, groupChatsTask, groupsTask);

            var individualChats = await individualChatsTask;
            var groupChats = await groupChatsTask;
            var groups = await groupsTask;

            var userIndividualChats = individualChats
                .Where(chat => chat.Object.Participants.Contains(userId))
                .ToDictionary(
                    chat => chat.Key,
                    chat =>
                    {
                        chat.Object.Messages = chat.Object.Messages
                            .Where(message => !message.Value.DeletedFor!.ContainsKey(userId))
                            .OrderBy(message => message.Value.Status.Sent.Values.First())
                            .ToDictionary(message => message.Key, message => message.Value);

                        return chat.Object;
                    }
                );

            var userGroupIds = groups
                .Where(group =>
                    group.Object.Participants.ContainsKey(userId)
                    &&
                    group.Object.Participants[userId] != GroupParticipant.Former
                )
                .Select(group => group.Key)
                .ToList();

            var userGroupChats = groupChats
                .Where(chat => userGroupIds.Contains(chat.Object.Participants.First()))
                .ToDictionary(chat =>
                    chat.Key,
                    chat =>
                    {
                        chat.Object.Messages = chat.Object.Messages
                            .Where(message => !message.Value.DeletedFor!.ContainsKey(userId))
                            .OrderBy(message => message.Value.Status.Sent.Values.First())
                            .ToDictionary(message => message.Key, message => message.Value);

                        return chat.Object;
                    }
                );

            var chatsRecipientIds = userIndividualChats
                .Select(chat => chat.Value.Participants.FirstOrDefault(participant => !participant.Equals(userId))!)
                .ToList();

            return (new Dictionary<string, Dictionary<string, Chat>>
            {
                { "Individual", userIndividualChats },
                { "Group", userGroupChats }
            },
            chatsRecipientIds,
            userGroupIds
            );
        }



        /// <summary>
        /// Bir sohbeti temizler.
        /// </summary>
        /// <param name="userId">Sohbeti temizleyen kullanıcının kimliği.</param>
        /// <param name="chatType">Sohbet türü (bireysel veya grup).</param>
        /// <param name="chatId">Temizlenecek sohbetin kimliği.</param>
        /// <returns>Temizlenmiş sohbeti içeren bir sözlük.</returns>
        /// <exception cref="NotFoundException">Sohbet bulunamadığında fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcı yetkisi olmadığında fırlatılır.</exception>
        public async Task<Dictionary<string, Dictionary<string, Chat>>> ClearChatAsync(string userId, string chatType, string chatId)
        {
            FieldValidationHelper.ValidateRequiredFields((chatType, "chatType"), (chatId, "chatId"));

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            if (chat.Messages != null)
            {
                foreach (var message in chat.Messages)
                {
                    if (!message.Value.DeletedFor!.ContainsKey(userId))
                    {
                        message.Value.DeletedFor.Add(userId, DateTime.UtcNow);
                    }
                }

                await _chatRepository.UpdateChatMessageAsync(chatType, chatId, chat.Messages);

                chat.Messages.Clear();


                return new Dictionary<string, Dictionary<string, Chat>>
                {
                    {chatType, new Dictionary<string, Chat> { { chatId, chat } }}
                };
            }
            else
            {
                throw new BadRequestException("Henüz silebileceğiniz bir mesaj yok.");
            }
        }



        /// <summary>
        /// Bireysel sohbeti arşivler.
        /// </summary>
        /// <param name="userId">Sohbeti arşivleyen kullanıcının kimliği.</param>
        /// <param name="chatId">Arşivlenecek sohbetin kimliği.</param>
        /// <returns>Arşivleme işlemini içeren bir sözlük.</returns>
        /// <exception cref="NotFoundException">Sohbet bulunamadığında fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcı yetkisi olmadığında fırlatılır.</exception>
        /// <exception cref="BadRequestException">Sohbet zaten arşivlenmişse fırlatılır.</exception>
        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>> ArchiveIndividualChatAsync(string userId, string chatId)
        {
            FieldValidationHelper.ValidateRequiredFields((chatId, chatId));

            var chat = await _chatRepository.GetChatByIdAsync("Individual", chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            if (!chat.ArchivedFor.ContainsKey(userId))
            {
                var archivedFor = chat.ArchivedFor;
                archivedFor.Add(userId, DateTime.UtcNow);

                await _chatRepository.UpdateChatArchivedForAsync("Individual", chatId, archivedFor);

                return new Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>
                {
                    {"Individual", new Dictionary<string, Dictionary<string, DateTime>> { { chatId, archivedFor } } }
                };
            }
            else
            {
                throw new BadRequestException("Sohbet zaten arşivlenmiş.");
            }
        }



        /// <summary>
        /// Bireysel sohbeti arşivden çıkarır.
        /// </summary>
        /// <param name="userId">Sohbeti arşivden çıkaran kullanıcının kimliği.</param>
        /// <param name="chatId">Arşivden çıkarılacak sohbetin kimliği.</param>
        /// <returns>Arşivden çıkarma işlemini içeren bir sözlük.</returns>
        /// <exception cref="NotFoundException">Sohbet bulunamadığında fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcı yetkisi olmadığında fırlatılır.</exception>
        /// <exception cref="BadRequestException">Sohbet zaten arşivde değilse fırlatılır.</exception>
        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>> UnarchiveIndividualChatAsync(string userId, string chatId)
        {
            FieldValidationHelper.ValidateRequiredFields((chatId, "chatId"));

            var chat = await _chatRepository.GetChatByIdAsync("Individual", chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (!chat.Participants.Contains(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            if (chat.ArchivedFor.ContainsKey(userId))
            {
                var archivedFor = chat.ArchivedFor;
                archivedFor.Remove(userId);

                await _chatRepository.UpdateChatArchivedForAsync("Individual", chatId, archivedFor);

                return new Dictionary<string, Dictionary<string, Dictionary<string, DateTime>>>
                {
                    {"Individual", new Dictionary<string, Dictionary<string, DateTime>> { { chatId, archivedFor } } }
                };
            }
            else
            {
                throw new BadRequestException("Sohbet zaten arşivde değil.");
            }
        }
    }
}