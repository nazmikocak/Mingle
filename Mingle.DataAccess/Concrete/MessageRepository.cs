using Firebase.Database;
using Firebase.Database.Query;
using Mingle.DataAccess.Abstract;
using Mingle.DataAccess.Configurations;
using Mingle.Entities.Models;

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
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).PutAsync(message);
        }


        public async Task UpdateMessageDeletedForAsync(string chatType, string chatId, string messageId, Dictionary<string, DateTime> deletedFor)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).Child("DeletedFor").PutAsync(deletedFor);
        }


        public async Task UpdateMessageStatusAsync(string chatType, string chatId, string messageId, string fieldName, object fieldData)
        {
            await _databaseClient.Child("Chats").Child(chatType).Child(chatId).Child("Messages").Child(messageId).Child("Status").Child(fieldName).PutAsync(fieldData);
        }
    }
}