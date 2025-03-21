using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using Mingle.Shared.DTOs.Request;

namespace Mingle.Services.Concrete
{
    /// <summary>
    /// Mesaj gönderme, silme ve durum güncelleme işlemlerini yöneten servis sınıfıdır.
    /// Sohbetler arası mesajlaşma işlemleri için gerekli işlemleri içerir.
    /// </summary>
    public sealed class MessageService : IMessageService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IChatRepository _chatRepository;



        /// <summary>
        /// MessageService sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="groupRepository">Grup yönetimi için kullanılan repository.</param>
        /// <param name="cloudRepository">Dosya yükleme işlemleri için kullanılan repository.</param>
        /// <param name="chatRepository">Sohbet yönetimi için kullanılan repository.</param>
        public MessageService(IGroupRepository groupRepository, ICloudRepository cloudRepository, IChatRepository chatRepository)
        {
            _groupRepository = groupRepository;
            _cloudRepository = cloudRepository;
            _chatRepository = chatRepository;
        }


        /// <summary>
        /// Bir mesajı gönderir.
        /// </summary>
        /// <param name="userId">Mesajı gönderen kullanıcının ID'si.</param>
        /// <param name="chatId">Mesajın gönderileceği sohbetin ID'si.</param>
        /// <param name="chatType">Sohbet tipi (Bireysel ya da Grup).</param>
        /// <param name="dto">Gönderilen mesajın içeriği.</param>
        /// <returns>Gönderilen mesajın bilgilerini içeren bir tuple.</returns>
        /// <exception cref="NotFoundException">Sohbet bulunamadığında fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcı sohbet üzerinde yetkiye sahip değilse fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler için fırlatılır.</exception>
        public async Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> SendMessageAsync(string userId, string chatId, string chatType, SendMessage dto)
        {
            var chatParticipants = await _chatRepository.GetChatParticipantsByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            if (chatType.Equals("Individual"))
            {
                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }
            }
            else if (chatType.Equals("Group"))
            {
                chatParticipants = await _groupRepository.GetGroupParticipantsIdsAsync(chatParticipants.First()) ?? throw new NotFoundException("Grup bulunamadı.");
                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }

            string messageId = Guid.NewGuid().ToString();
            string messageContent;
            long? fileSize = null;

            if (dto.ContentType.Equals(MessageContent.Text))
            {
                messageContent = dto.Content;
            }
            else if (dto.ContentType.Equals(MessageContent.Image))
            {
                var photoUrl = await _cloudRepository.UploadPhotoAsync(messageId, $"Chats/{chatId}", "image_message", FileValidationHelper.ValidatePhoto(dto.Content));
                messageContent = photoUrl.ToString() ?? throw new Exception("Dosya yüklenemedi.");
            }
            else if (dto.ContentType.Equals(MessageContent.Video))
            {
                var videoUrl = await _cloudRepository.UploadVideoAsync(messageId, $"Chats/{chatId}", "video_message", FileValidationHelper.ValidateVideo(dto.Content));
                messageContent = videoUrl.ToString() ?? throw new Exception("Dosya yüklenemedi.");
            }
            else if (dto.ContentType.Equals(MessageContent.Audio))
            {
                var audioUrl = await _cloudRepository.UploadAudioAsync(messageId, $"Chats/{chatId}", "audio_message", FileValidationHelper.ValidateVideo(dto.Content));
                messageContent = audioUrl.ToString() ?? throw new Exception("Dosya yüklenemedi.");
            }
            else if (dto.ContentType.Equals(MessageContent.File))
            {
                var (fileUrl, size) = await _cloudRepository.UploadFileAsync(messageId, $"Chats/{chatId}", "file_message", FileValidationHelper.ValidateFile(dto.Content));
                messageContent = fileUrl.ToString() ?? throw new Exception("Dosya yüklenemedi.");
                fileSize = size;
            }
            else
            {
                throw new BadRequestException("contentType geçersiz.");
            }


            var message = new Message
            {
                Content = messageContent,
                FileName = dto.FileName,
                FileSize = fileSize,
                Type = dto.ContentType,
                Status = new MessageStatus
                {
                    Sent = new Dictionary<string, DateTime>
                    {
                        { userId, DateTime.UtcNow }
                    }
                }
            };

            var messageVM = new Dictionary<string, Dictionary<string, Dictionary<string, Message>>>
            {
                {
                    chatType, new Dictionary<string, Dictionary<string, Message>>
                    {
                        {
                            chatId, new Dictionary<string, Message>
                            {
                                { messageId, message }
                            }
                        }
                    }
                }
            };

            return (messageVM, chatParticipants);
        }



        /// <summary>
        /// Bir mesajı siler.
        /// </summary>
        /// <param name="userId">Mesajı silen kullanıcının ID'si.</param>
        /// <param name="chatType">Sohbet tipi (Bireysel ya da Grup).</param>
        /// <param name="chatId">Sohbetin ID'si.</param>
        /// <param name="messageId">Silinecek mesajın ID'si.</param>
        /// <param name="deletionType">Silme türü (0: Sadece kendisi, 1: Herkes için).</param>
        /// <returns>Silinen mesajın bilgilerini içeren bir tuple.</returns>
        /// <exception cref="NotFoundException">Mesaj bulunamadığında fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcı sohbet üzerinde yetkiye sahip değilse fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler için fırlatılır.</exception>
        public async Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> DeleteMessageAsync(string userId, string chatType, string chatId, string messageId, byte deletionType)
        {
            FieldValidationHelper.ValidateRequiredFields((chatType, "chatType"), (chatId, "chatId"), (messageId, "messageId"));

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId);

            var chatParticipants = chat.Participants;


            if (chatType.Equals("Individual"))
            {
                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }
            }
            else if (chatType.Equals("Group"))
            {
                chatParticipants = await _groupRepository.GetGroupParticipantsIdsAsync(chatParticipants.First()) ?? throw new NotFoundException("Grup bulunamadı.");

                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }

            var message = chat.Messages.GetValueOrDefault(messageId) ?? throw new NotFoundException("Mesaj bulunamadı.");

            if (deletionType.Equals(0))
            {
                message.DeletedFor!.Add(userId, DateTime.UtcNow);
                message.Content = "";
            }
            else if (deletionType.Equals(1))
            {
                foreach (var participant in chatParticipants)
                {
                    message.DeletedFor!.Add(participant, DateTime.UtcNow);
                }

                message.Content = "Bu mesaj silindi.";
            }
            else
            {
                throw new BadRequestException("deletionType geçersiz.");
            }

            var messageVM = new Dictionary<string, Dictionary<string, Dictionary<string, Message>>>
            {
                {
                    chatType, new Dictionary<string, Dictionary<string, Message>>
                    {
                        {
                            chatId, new Dictionary<string, Message>
                            {
                                { messageId, message }
                            }
                        }
                    }
                }
            };

            return (messageVM, chatParticipants);
        }



        /// <summary>
        /// Bir mesajın teslim veya okunma durumunu günceller.
        /// </summary>
        /// <param name="userId">Durumu güncelleyen kullanıcının ID'si.</param>
        /// <param name="chatType">Sohbet tipi (Bireysel ya da Grup).</param>
        /// <param name="chatId">Sohbetin ID'si.</param>
        /// <param name="messageId">Durumu güncellenecek mesajın ID'si.</param>
        /// <param name="fieldName">Güncellenecek durum (Delivered veya Read).</param>
        /// <returns>Güncellenmiş mesajın bilgilerini içeren bir tuple.</returns>
        /// <exception cref="NotFoundException">Mesaj bulunamadığında fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcı sohbet üzerinde yetkiye sahip değilse fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz parametreler için fırlatılır.</exception>
        public async Task<(Dictionary<string, Dictionary<string, Dictionary<string, Message>>>, List<string>)> DeliverOrReadMessageAsync(string userId, string chatType, string chatId, string messageId, string fieldName)
        {
            FieldValidationHelper.ValidateRequiredFields((chatType, "chatType"), (chatId, "chatId"), (messageId, "messageId"));

            var chat = await _chatRepository.GetChatByIdAsync(chatType, chatId) ?? throw new NotFoundException("Sohbet bulunamadı!");

            var chatParticipants = chat.Participants;

            if (chatType.Equals("Individual"))
            {
                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }
            }
            else if (chatType.Equals("Group"))
            {
                chatParticipants = await _groupRepository.GetGroupParticipantsIdsAsync(chatParticipants.First()) ?? throw new NotFoundException("Grup bulunamadı.");

                if (!chatParticipants.Contains(userId))
                {
                    throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
                }
            }
            else
            {
                throw new BadRequestException("chatType geçersiz.");
            }

            var message = chat.Messages.GetValueOrDefault(messageId) ?? throw new NotFoundException("Mesaj bulunamadı.");

            if (fieldName.Equals("Delivered"))
            {
                if (!message.Status.Delivered.ContainsKey(userId))
                {
                    message.Status.Delivered.Add(userId, DateTime.UtcNow);
                }

                if (message.DeletedFor!.ContainsKey(userId))
                {
                    message.Content = "Bu mesaj silindi.";
                }
            }
            else if (fieldName.Equals("Read"))
            {
                if (!message.Status.Read.ContainsKey(userId))
                {
                    message.Status.Read.Add(userId, DateTime.UtcNow);
                }
            }
            else
            {
                throw new BadRequestException("fieldName geçersiz.");
            }

            var messageVM = new Dictionary<string, Dictionary<string, Dictionary<string, Message>>>
            {
                {
                    chatType, new Dictionary<string, Dictionary<string, Message>>
                    {
                        {
                            chatId, new Dictionary<string, Message>
                            {
                                { messageId, message }
                            }
                        }
                    }
                }
            };

            return (messageVM, chatParticipants);
        }
    }
}