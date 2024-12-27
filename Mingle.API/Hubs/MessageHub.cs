using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.DataAccess.Abstract;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.Exceptions;
using System.Diagnostics;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    [Authorize]
    public sealed class MessageHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatService _chatService;
        private readonly IUserService _userService;


        private string UserId
        {
            get
            {
                var identity = Context.User!.Identity as ClaimsIdentity;
                return identity!
                    .Claims
                    .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)!
                    .Value;
            }
        }


        public MessageHub(IMessageService messageService, IMessageRepository messageRepository, IChatService chatService, IUserService userService)
        {
            _messageService = messageService;
            _messageRepository = messageRepository;
            _chatService = chatService;
            _userService = userService;
        }


        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            if (!userCS.ConnectionIds.Contains(connectionId))
            {
                userCS.ConnectionIds.Add(connectionId);
                userCS.LastConnectionDate = null;
                await _userService.SaveConnectionSettingsAsync(UserId, userCS);
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var userCS = await _userService.GetConnectionSettingsAsync(UserId);

            if (!userCS.ConnectionIds.Count.Equals(0) && userCS.ConnectionIds.Contains(connectionId))
            {
                userCS.ConnectionIds.Remove(connectionId);
                userCS.LastConnectionDate = DateTime.UtcNow;

                await _userService.SaveConnectionSettingsAsync(UserId, userCS);
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendDemoMessage(string chatId, SendMessage dto)
        {
            Console.WriteLine($"\nYeni Metot İle Deneme");

            var stopwatch = Stopwatch.StartNew();

            stopwatch.Restart();
            // Mesaj oluşturma ve kaydetme
            var (message, recipientId) = await _messageService.SendMessageAsync(UserId, chatId, "Individual", dto);
            Console.WriteLine($"Mesaj oluşturma: {stopwatch.ElapsedMilliseconds} ms");


            stopwatch.Restart();
            // Paralel işler: RecipientId ve kullanıcı ayarlarını alma
            var createMessageTask = _messageRepository.CreateMessageAsync(UserId, "Individual", chatId, message.Keys.First(), message.Values.First());
            Console.WriteLine($"Paralel işler (RecipientId ve kullanıcı ayarlarını alma süresi): {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            // Caller istemcisine mesaj gönderimi
            await Clients.Caller.SendAsync("ReceiveGetMessages", new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                }
            );
            Console.WriteLine($"Caller istemcisine mesaj gönderim süresi: {stopwatch.ElapsedMilliseconds} ms");


            stopwatch.Restart();
            // Alıcı istemcisine mesaj gönderimi
            var userCS = await _userService.GetConnectionSettingsAsync(recipientId);
            Console.WriteLine($"recipientId ve userCS: {stopwatch.ElapsedMilliseconds} ms");



            stopwatch.Restart();
            if (!userCS.ConnectionIds.Count.Equals(0))
            {
                var sendTasks = userCS.ConnectionIds
                    .Select(connectionId => Clients.Client(connectionId).SendAsync("ReceiveGetMessages", new Dictionary<string, Dictionary<string, Message>>
                    {
            { chatId, message }
                    }));
                await Task.WhenAll(sendTasks);
            }
            Console.WriteLine($"Alıcı istemcisine mesaj gönderim süresi: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            // Mesajın veri tabanına kaydedilmesi
            await createMessageTask;
            Console.WriteLine($"Mesajın veri tabanına kaydedilme süresi: {stopwatch.ElapsedMilliseconds} ms");
        }


        public async Task SendMessage(string chatId, SendMessage dto)
        {
            Console.WriteLine($"\nEski Metot İle Deneme");
            try
            {
                var stopwatch = Stopwatch.StartNew();


                stopwatch.Restart();
                var (message, recipientId) = await _messageService.SendMessageAsync(UserId, chatId, "Individual", dto);
                Console.WriteLine($"Mesaj oluşturma süresi: {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                var messageVM = new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                };
                Console.WriteLine($"VM haline getirme süresi: {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                await Clients.Caller.SendAsync("ReceiveGetMessages", messageVM);
                Console.WriteLine($"Caller istemcisine mesaj gönderim süresi: {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                var userCS = await _userService.GetConnectionSettingsAsync(recipientId);
                Console.WriteLine($"RecipientId ve kullanıcı ayarlarını alma süresi: {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                if (!userCS.ConnectionIds.Count.Equals(0))
                {
                    foreach (var connectionId in userCS.ConnectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveGetMessages", messageVM);
                    }
                }
                Console.WriteLine($"Alıcı istemcisine mesaj gönderim süresi: {stopwatch.ElapsedMilliseconds} ms");


                stopwatch.Restart();
                await _messageRepository.CreateMessageAsync(UserId, "Individual", chatId, message.Keys.First(), message.Values.First());
                Console.WriteLine($"Mesajın veri tabanına kaydedilme süresi: {stopwatch.ElapsedMilliseconds} ms");


                Console.WriteLine($"Eski metot ile ölçüm süresi: {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", $"Firebase ile ilgili bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Beklenmedik bir hata oluştu: {ex.Message}");
            }
        }


        public async Task DeliverMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var message = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Delivered");

                var messageVM = new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                };

                await Clients.Caller.SendAsync("ReceiveGetMessages", messageVM);

                var recipientId = await _chatService.GetChatRecipientIdAsync(UserId, "Individual", chatId);
                var userCS = await _userService.GetConnectionSettingsAsync(recipientId);

                if (!userCS.ConnectionIds.Count.Equals(0))
                {
                    foreach (var connectionId in userCS.ConnectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveGetMessages", messageVM);
                    }
                }

                await _messageRepository.UpdateMessageStatusAsync(chatType, chatId, messageId, "Delivered", message.Values.Select(x => x.Status.Delivered.Keys.SingleOrDefault(x => x.Equals(UserId))));
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", $"Firebase ile ilgili bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Beklenmedik bir hata oluştu: {ex.Message}");
            }
        }


        public async Task ReadMessage(string chatType, string chatId, string messageId)
        {
            try
            {
                var message = await _messageService.DeliverOrReadMessageAsync(UserId, chatType, chatId, messageId, "Read");

                var messageVM = new Dictionary<string, Dictionary<string, Message>>
                {
                    { chatId, message }
                };

                await Clients.Caller.SendAsync("ReceiveGetMessages", messageVM);

                var recipientId = await _chatService.GetChatRecipientIdAsync(UserId, "Individual", chatId);
                var userCS = await _userService.GetConnectionSettingsAsync(recipientId);

                if (!userCS.ConnectionIds.Count.Equals(0))
                {
                    foreach (var connectionId in userCS.ConnectionIds)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveGetMessages", messageVM);
                    }
                }

                await _messageRepository.UpdateMessageStatusAsync(chatType, chatId, messageId, "Read", message.Values.Select(x => x.Status.Read.Keys.SingleOrDefault(x => x.Equals(UserId))));
            }
            catch (NotFoundException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (BadRequestException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (FirebaseException ex)
            {
                await Clients.Caller.SendAsync("Error", $"Firebase ile ilgili bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Beklenmedik bir hata oluştu: {ex.Message}");
            }
        }
    }
}