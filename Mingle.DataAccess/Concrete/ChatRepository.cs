using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Concrete
{
    public sealed class ChatRepository : IChatRepository
    {
        private readonly FirebaseClient _databaseClient;


        public ChatRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }

        public async Task CreateChatAsync(string chatType, string chatId, Chat chat)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).PutAsync(chat);
        }


        public async Task<IReadOnlyCollection<FirebaseObject<Chat>>> GetChatsAsync(string chatType)
        {
            return await _databaseClient.Child("Chats").Child(chatType).OnceAsync<Chat>();
        }


        public async Task<Chat> GetChatByIdAsync(string chatType, string chatId)
        {
            return await _databaseClient.Child("Chats").Child(chatType).Child(chatId).OnceSingleAsync<Chat>();
        }


        public async Task<List<string>> GetChatParticipantsAsync(string chatType, string chatId)
        {
            return await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Participants").OnceSingleAsync<List<string>>();
        }


        public async Task UpdateChatArchivedForAsync(string chatType, string chatId, Dictionary<string, DateTime> archivedFor)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).PatchAsync(new { ArchivedFor = archivedFor });
        }
    }
}