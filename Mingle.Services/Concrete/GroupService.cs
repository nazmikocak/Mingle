using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using Mingle.Shared.DTOs.Request;
using Mingle.Shared.DTOs.Response;
using System.Text.Json;


namespace Mingle.Services.Concrete
{
    /// <summary>
    /// Sohbet işlemlerini yöneten servis sınıfıdır.
    /// Bireysel ve grup sohbetlerinin oluşturulması, görüntülenmesi ve arşivlenmesi gibi işlemleri içerir.
    /// </summary>
    public sealed class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IUserRepository _userRepository;



        /// <summary>
        /// ChatService sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="groupRepository">Grup yönetimi için kullanılan repository.</param>
        /// <param name="chatRepository">Sohbet yönetimi için kullanılan repository.</param>
        /// <param name="userRepository">Kullanıcı yönetimi için kullanılan repository.</param>
        public GroupService(IGroupRepository groupRepository, ICloudRepository cloudRepository, IUserRepository userRepository)
        {
            _groupRepository = groupRepository;
            _cloudRepository = cloudRepository;
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
        public async Task<Dictionary<string, GroupProfile>> CreateGroupAsync(string userId, CreateGroup dto)
        {
            var groupParticipants = JsonSerializer.Deserialize<Dictionary<string, GroupParticipant>>(dto.Participants)
                                    ?? throw new BadRequestException("Grup katılımcıları hatalı.");

            string groupId = Guid.NewGuid().ToString();
            Uri photoUrl;

            if (dto.Photo != null)
            {
                photoUrl = await _cloudRepository.UploadPhotoAsync(groupId, $"Groups/{groupId}", "group_photo", FileValidationHelper.ValidatePhoto(dto.Photo));
            }
            else
            {
                photoUrl = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1744980063/DefaultGroupProfileImage.png");
            }

            var group = new Group
            {
                Name = dto.Name,
                Description = String.IsNullOrEmpty(dto.Description)
                    ? "Merhaba, bu bir Mingle grubudur."
                    : dto.Description,
                Photo = photoUrl,
                Participants = new Dictionary<string, GroupParticipant>
                {
                    { userId, GroupParticipant.Admin }
                },
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
            };

            foreach (var participant in groupParticipants)
            {
                group.Participants[participant.Key] = participant.Value;
            }

            await _groupRepository.CreateOrUpdateGroupAsync(groupId, group);

            var users = await _userRepository.GetAllUsersAsync();

            var participants = users
                .Where(user => group.Participants.ContainsKey(user.Key))
                .ToDictionary(
                    participant => participant.Key,
                    participant => new ParticipantProfile
                    {
                        DisplayName = participant.Object.DisplayName,
                        ProfilePhoto = participant.Object.ProfilePhoto,
                        Role = group.Participants[participant.Key],
                        LastConnectionDate = participant.Object.LastConnectionDate,
                    }
                );

            var groupProfile = new GroupProfile
            {
                Name = group.Name,
                Description = group.Description!,
                PhotoUrl = group.Photo,
                Participants = participants,
                CreatedDate = group.CreatedDate
            };

            return new Dictionary<string, GroupProfile> { { groupId, groupProfile } };
        }



        /// <summary>
        /// Kullanıcının tüm sohbetlerini getirir.
        /// </summary>
        /// <param name="userId">Kullanıcının kimliği.</param>
        /// <returns>Kullanıcının bireysel ve grup sohbetlerini içeren bir sözlük.</returns>
        public async Task<Dictionary<string, GroupProfile>> EditGroupAsync(string userId, string groupId, CreateGroup dto)
        {
            FieldValidationHelper.ValidateRequiredFields((groupId, "groupId"));

            var groupParticipants = JsonSerializer.Deserialize<Dictionary<string, GroupParticipant>>(dto.Participants)
                                    ?? throw new BadRequestException("Grup katılımcıları hatalı.");

            var group = await _groupRepository.GetGroupByIdAsync(groupId) ?? throw new NotFoundException("Grup bulunamadı.");

            if (!group.Participants.Any(participant => participant.Key.Equals(userId) && participant.Value.Equals(GroupParticipant.Admin)))
            {
                throw new ForbiddenException("Bu işlemi yapma yetkiniz yok.");
            }

            if (!groupParticipants.Any(participant => participant.Key.Equals(group.CreatedBy) && participant.Value.Equals(GroupParticipant.Admin)))
            {
                if (group.Participants.ContainsKey(group.CreatedBy))
                {
                    throw new BadRequestException("Grup kurucusu yöneticilikten ve gruptan çıkarılamaz.");
                }
            }

            foreach (var participant in group.Participants.Keys)
            {
                if (!groupParticipants.ContainsKey(participant))
                {
                    throw new BadRequestException("Grup katılımcıları gruptan çıkarılamaz. Rolü değiştirilebilir.");
                }
            }

            foreach (var participant in groupParticipants.Keys)
            {
                var user = await _userRepository.GetUserByIdAsync(participant) ?? throw new BadRequestException("Bazı kullanıcılar geçersiz.");
            }

            Uri photoUrl;

            if (dto.Photo != null)
            {
                photoUrl = await _cloudRepository.UploadPhotoAsync(groupId, $"Groups/{groupId}", "group_photo", FileValidationHelper.ValidatePhoto(dto.Photo));
            }
            else
            {
                if (dto.PhotoUrl != null)
                {
                    photoUrl = new Uri(dto.PhotoUrl);
                }
                else
                {
                    photoUrl = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1744980063/DefaultGroupProfileImage.png");
                }
            }

            var newGroup = new Group
            {
                Name = dto.Name,
                Description = dto.Description!,
                Photo = photoUrl,
                Participants = groupParticipants
                    .ToDictionary(
                        participant => participant.Key,
                        participant => participant.Value
                    ),
                CreatedBy = group.CreatedBy,
                CreatedDate = group.CreatedDate
            };

            await _groupRepository.CreateOrUpdateGroupAsync(groupId, newGroup);

            var users = await _userRepository.GetAllUsersAsync();

            var participants = users
                .Where(user => groupParticipants.ContainsKey(user.Key))
                .ToDictionary(
                    participant => participant.Key,
                    participant => new ParticipantProfile
                    {
                        DisplayName = participant.Object.DisplayName,
                        ProfilePhoto = participant.Object.ProfilePhoto,
                        Role = groupParticipants[participant.Key],
                        LastConnectionDate = participant.Object.LastConnectionDate,
                    }
                );

            var groupProfile = new GroupProfile
            {
                Name = newGroup.Name,
                Description = newGroup.Description!,
                PhotoUrl = newGroup.Photo,
                Participants = participants,
                CreatedDate = newGroup.CreatedDate
            };

            return new Dictionary<string, GroupProfile> { { groupId, groupProfile } };
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
        public async Task<Dictionary<string, GroupProfile>> GetGroupProfilesAsync(List<string> userGroupIds)
        {
            var groups = await _groupRepository.GetAllGroupAsync();
            var users = await _userRepository.GetAllUsersAsync();

            Dictionary<string, GroupProfile> groupProfile = [];

            foreach (var group in groups.Where(group => userGroupIds.Contains(group.Key)))
            {
                Dictionary<string, ParticipantProfile> groupUsers = [];

                foreach (var participant in group.Object.Participants)
                {
                    var user = users.FirstOrDefault(x => x.Key.Equals(participant.Key))!;

                    var participantProfile = new ParticipantProfile
                    {
                        DisplayName = user.Object.DisplayName,
                        ProfilePhoto = user.Object.ProfilePhoto,
                        Role = participant.Value,
                        LastConnectionDate = user.Object.LastConnectionDate,
                    };

                    groupUsers.Add(participant.Key, participantProfile);
                }

                groupProfile.Add(group.Key, new GroupProfile
                {
                    Name = group.Object.Name,
                    Description = group.Object.Description,
                    PhotoUrl = group.Object.Photo,
                    Participants = groupUsers,
                    CreatedDate = group.Object.CreatedDate
                });
            }

            return groupProfile;
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
        public async Task<List<string>> GetGroupParticipantsAsync(string userId, string groupId)
        {
            FieldValidationHelper.ValidateRequiredFields((groupId, "groupId"));

            var groupParticipants = await _groupRepository.GetGroupParticipantsIdsAsync(groupId) ?? throw new NotFoundException("Grup bulunamadı.");

            if (!groupParticipants.Contains(userId))
            {
                throw new ForbiddenException("Bu işlemi yapma yetkiniz yok.");
            }

            return groupParticipants;
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
        public async Task<Dictionary<string, GroupProfile>> LeaveGroupAsync(string userId, string groupId)
        {
            FieldValidationHelper.ValidateRequiredFields((groupId, "groupId"));

            var group = await _groupRepository.GetGroupByIdAsync(groupId) ?? throw new NotFoundException("Grup bulunamadı.");

            if (!group.Participants.ContainsKey(userId) || group.Participants[userId].Equals(GroupParticipant.Former))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            group.Participants[userId] = GroupParticipant.Former;

            if (!group.Participants.Count.Equals(0) && !group.Participants.ContainsValue(GroupParticipant.Admin))
            {
                group.Participants[group.Participants.Keys.ElementAt(new Random().Next(group.Participants.Count))] = GroupParticipant.Admin;
            }

            await _groupRepository.UpdateGroupParticipantsAsync(groupId, group.Participants);

            var users = await _userRepository.GetAllUsersAsync();

            var participants = users
                .Where(user => group.Participants.ContainsKey(user.Key))
                .ToDictionary(
                    participant => participant.Key,
                    participant => new ParticipantProfile
                    {
                        DisplayName = participant.Object.DisplayName,
                        ProfilePhoto = participant.Object.ProfilePhoto,
                        Role = group.Participants[participant.Key],
                        LastConnectionDate = participant.Object.LastConnectionDate
                    }
                );

            var groupProfile = new GroupProfile
            {
                Name = group.Name,
                Description = group.Description!,
                PhotoUrl = group.Photo,
                Participants = participants,
                CreatedDate = group.CreatedDate
            };

            return new Dictionary<string, GroupProfile> { { groupId, groupProfile } };
        }
    }
}