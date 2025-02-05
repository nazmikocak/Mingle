﻿using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;

namespace Mingle.Services.Concrete
{
    public sealed class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IChatRepository _chatRepository;


        public MessageService(IMessageRepository messageRepository, IGroupRepository groupRepository, ICloudRepository cloudRepository, IChatRepository chatRepository)
        {
            _messageRepository = messageRepository;
            _groupRepository = groupRepository;
            _cloudRepository = cloudRepository;
            _chatRepository = chatRepository;
        }


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

            if (dto.ContentType.Equals(MessageContent.Text))
            {
                messageContent = dto.Content;
            }
            else if (dto.ContentType.Equals(MessageContent.Image))
            {
                var photoBytes = Convert.FromBase64String(dto.Content);

                var photo = new MemoryStream(photoBytes);
                FileValidationHelper.ValidatePhoto(photo);

                var photoUrl = await _cloudRepository.UploadPhotoAsync(messageId, $"Chats/{chatId}", "image_message", photo);
                messageContent = photoUrl.ToString();
            }
            else if (dto.ContentType.Equals(MessageContent.Video))
            {
                var videoBytes = Convert.FromBase64String(dto.Content);

                var video = new MemoryStream(videoBytes);
                FileValidationHelper.ValidateVideo(video);

                var videoUrl = await _cloudRepository.UploadVideoAsync(messageId, $"Chats/{chatId}", "video_message", video);
                messageContent = videoUrl.ToString();
            }
            else if (dto.ContentType.Equals(MessageContent.Audio))
            {
                var audioBytes = Convert.FromBase64String(dto.Content);

                var audio = new MemoryStream(audioBytes);
                FileValidationHelper.ValidateVideo(audio);

                var audioUrl = await _cloudRepository.UploadAudioAsync(messageId, $"Chats/{chatId}", "audio_message", audio);
                messageContent = audioUrl.ToString();
            }
            else if (dto.ContentType.Equals(MessageContent.File))
            {
                var fileBytes = Convert.FromBase64String(dto.Content);

                var file = new MemoryStream(fileBytes);
                FileValidationHelper.ValidateFile(file);

                var fileUrl = await _cloudRepository.UploadFileAsync(messageId, $"Chats/{chatId}", "file_message", file);
                messageContent = fileUrl.ToString();
            }
            else
            {
                throw new BadRequestException("contentType geçersiz.");
            }



            var message = new Message
            {
                Content = messageContent,
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
                message.DeletedFor.Add(userId, DateTime.UtcNow);
                message.Content = "";
            }
            else if (deletionType.Equals(1))
            {
                foreach (var participant in chatParticipants)
                {
                    message.DeletedFor.Add(participant, DateTime.UtcNow);
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
                message.Status.Delivered.Add(userId, DateTime.UtcNow);
            }
            else if (fieldName.Equals("Read"))
            {
                message.Status.Read.Add(userId, DateTime.UtcNow);
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