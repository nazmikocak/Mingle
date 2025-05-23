﻿using Firebase.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Concrete;
using Mingle.Services.Exceptions;
using System.Security.Claims;

namespace Mingle.API.Hubs
{
    /// <summary>
    /// Gerçek zamanlı bildirim işlemlerini yöneten SignalR hub sınıfıdır.
    /// Kullanıcı bağlantılarını, profil güncellemelerini ve kullanıcı aramalarını yönetir.
    /// </summary>
    [Authorize]
    public sealed class NotificationHub : Hub
    {
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
        /// <see cref="NotificationHub"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="userService">Kullanıcı işlemleri için <see cref="IUserService"/> bağımlılığı.</param>
        public NotificationHub(IUserService userService)
        {
            _userService = userService;
        }



        /// <summary>
        /// Kullanıcı bağlantı kurduğunda çağrılır.
        /// </summary>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="Exception">Bağlantı kurulurken bir hata oluşursa fırlatılır.</exception>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        

        /// <summary>
        /// Kullanıcı bağlantısını kestiğinde çağrılır. Kullanıcının son bağlantı tarihini günceller ve diğer kullanıcılara bildirir.
        /// </summary>
        /// <param name="exception">Bağlantı kesilme sırasında oluşan hata (varsa).</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="Exception">Bağlantı kesilme sırasında bir hata oluşursa fırlatılır.</exception>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            DateTime lastConnectionDate = DateTime.UtcNow;
            await _userService.UpdateLastConnectionDateAsync(UserId, lastConnectionDate);

            await Clients.Others.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, DateTime>> { { UserId, new Dictionary<string, DateTime> { { "lastConnectionDate", lastConnectionDate } } } });
            await base.OnDisconnectedAsync(exception);
        }



        /// <summary>
        /// Kullanıcının son bağlantı tarihini sıfırlayarak günceller ve diğer istemcilere bildirir.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir <see cref="Task"/> nesnesi.</returns>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task Initial()
        {
            try
            {
                DateTime lastConnectionDate = DateTime.MinValue;
                await _userService.UpdateLastConnectionDateAsync(UserId, lastConnectionDate!);

                await Clients.Others.SendAsync("ReceiveRecipientProfiles", new Dictionary<string, Dictionary<string, DateTime>>
                {
                    { UserId, new Dictionary<string, DateTime> { { "lastConnectionDate", lastConnectionDate } } }
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnexpectedError", new { message = "Beklenmedik bir hata oluştu!", errorDetails = ex.Message });
            }
        }



        /// <summary>
        /// Kullanıcılar arasında arama yapar ve sonuçları çağıran kullanıcıya gönderir.
        /// </summary>
        /// <param name="query">Arama terimi.</param>
        /// <returns>Bir <see cref="Task"/> nesnesi döner.</returns>
        /// <exception cref="NotFoundException">Arama sonuçları bulunamazsa fırlatılır.</exception>
        /// <exception cref="BadRequestException">Geçersiz arama parametresi sağlanırsa fırlatılır.</exception>
        /// <exception cref="ForbiddenException">Kullanıcının arama yapmaya yetkisi yoksa fırlatılır.</exception>
        /// <exception cref="Exception">Beklenmedik bir hata oluşursa fırlatılır.</exception>
        public async Task SearchUsers(string query)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(UserId, query);
                await Clients.Caller.SendAsync("ReceiveSearchUsers", new Dictionary<string, object>
                    {
                        {"query", query },
                        {"data", users }
                    }
                );
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