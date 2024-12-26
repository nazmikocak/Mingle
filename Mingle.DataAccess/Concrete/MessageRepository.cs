using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;
using System.Xml.Linq;

namespace Mingle.DataAccess.Concrete
{
    public sealed class MessageRepository : IMessageRepository
    {
        private readonly FirebaseClient _databaseClient;


        public MessageRepository(FirebaseConfig firebaseConfig)
        {
            _databaseClient = firebaseConfig.DatabaseClient;
        }


        public async Task CreateMessageAsync(string userId, string chatType, string chatId, string messageId, Message message)
        {
            message.Status.Sent.Add(userId, DateTime.UtcNow);
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).PutAsync(message);
        }


        public async Task UpdateMessageDeletedForAsync(string chatType, string chatId, string messageId, Dictionary<string, DateTime> deletedFor)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).PatchAsync(new { DeletedFor = deletedFor });
        }


        public async Task<IReadOnlyCollection<FirebaseObject<Message>>> GetMessagesByChatIdAsync(string chatType, string chatId)
        {
            return await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").OnceAsync<Message>();
        }


        public async Task<Message> GetLastMessageByChatIdAsync(string chatType, string chatId)
        {
            var messages = await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").OnceAsync<Message>();

            var lastMessage = messages
                .Select(message => message.Object)
                .OrderBy(message => message.Status.Sent)
                .LastOrDefault();

            return lastMessage;
        }


        public async Task UpdateMessageStatusAsync(string chatType, string chatId, string messageId, string fieldName)
        {
            var fieldData = new Dictionary<string, DateTime>
            {
                { fieldName, DateTime.UtcNow }
            };

            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).Child("Status").Child(fieldName).PutAsync(fieldData);
        }


        public async Task<Message> GetMessageByIdAsync(string chatType, string chatId, string messageId)
        {
            return await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).OnceSingleAsync<Message>();
        }
    }
}