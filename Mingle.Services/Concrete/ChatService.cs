using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;


namespace Mingle.Services.Concrete
{
    public sealed class ChatService : IChatService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;


        public ChatService(IGroupRepository groupRepository, IChatRepository chatRepository, IUserRepository userRepository)
        {
            _groupRepository = groupRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }


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
                    chat => new Chat
                    {
                        Participants = chat.Object.Participants,
                        ArchivedFor = chat.Object.ArchivedFor,
                        CreatedDate = chat.Object.CreatedDate,
                        Messages = chat.Object.Messages
                            .Where(message => !message.Value.DeletedFor!.ContainsKey(userId))
                            .OrderBy(x => x.Value.Status.Sent.Values.First())
                            .ToDictionary(x => x.Key, x => x.Value)
                    }
                );

            var userGroupIds = groups
                .Where(group => group.Object.Participants.ContainsKey(userId) && group.Object.Participants[userId] != GroupParticipant.Former)
                .Select(group => group.Key)
                .ToList();

            var userGroupChats = groupChats
                .Where(chat => userGroupIds.Contains(chat.Object.Participants.First()))
                .ToDictionary(chat =>
                    chat.Key,
                    chat => new Chat
                    {
                        Participants = chat.Object.Participants,
                        ArchivedFor = chat.Object.ArchivedFor,
                        CreatedDate = chat.Object.CreatedDate,
                        Messages = chat.Object.Messages
                            .OrderBy(x => x.Value.Status.Sent.Values.First())
                            .ToDictionary(x => x.Key, x => x.Value)
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