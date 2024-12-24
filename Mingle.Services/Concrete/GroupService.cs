using Mingle.DataAccess.Abstract;
using Mingle.Entities.Enums;
using Mingle.Entities.Models;
using Mingle.Services.Abstract;
using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;
using Mingle.Services.Exceptions;
using Mingle.Services.Utilities;
using System.Text.Json;


namespace Mingle.Services.Concrete
{
    public sealed class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ICloudRepository _cloudRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;


        public GroupService(IGroupRepository groupRepository, ICloudRepository cloudRepository, IChatRepository chatRepository, IUserRepository userRepository)
        {
            _groupRepository = groupRepository;
            _cloudRepository = cloudRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }


        public async Task<string> CreateGroupAsync(string userId, CreateGroup dto)
        {
            var groupParticipants = JsonSerializer.Deserialize<Dictionary<string, GroupParticipant>>(dto.Participants)
                                    ?? throw new BadRequestException("Grup katılımcıları hatalı.");

            string groupId = Guid.NewGuid().ToString();
            Uri photoUrl;

            if (dto.Photo != null)
            {
                FileValidationHelper.ValidateProfilePhoto(dto.Photo);
                photoUrl = await _cloudRepository.UploadPhotoAsync(userId, $"Group/{groupId}", "group_photo", dto.Photo);
            }
            else
            {
                photoUrl = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185322/DefaultGroupProfileImage.png");
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

            return groupId;
        }


        public async Task EditGroupAsync(string userId, string groupId, CreateGroup dto)
        {
            var groupParticipants = JsonSerializer.Deserialize<Dictionary<string, GroupParticipant>>(dto.Participants)
                                    ?? throw new BadRequestException("Grup katılımcıları hatalı.");

            if (String.IsNullOrEmpty(groupId))
            {
                throw new BadRequestException("groupId gereklidir.");
            }

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


            // Geçersiz üye kontrolü


            Uri photoUrl;

            if (dto.Photo != null)
            {
                FileValidationHelper.ValidateProfilePhoto(dto.Photo);
                photoUrl = await _cloudRepository.UploadPhotoAsync(userId, $"Group/{groupId}", "group_photo", dto.Photo);
            }
            else
            {
                if (dto.PhotuUrl != null)
                {
                    photoUrl = new Uri(dto.PhotuUrl);
                }
                else
                {
                    photoUrl = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185322/DefaultGroupProfileImage.png");
                }
            }

            var newGroup = new Group
            {
                Name = dto.Name,
                Description = String.IsNullOrEmpty(dto.Description)
                    ? "Merhaba, bu bir Mingle grubudur."
                    : dto.Description,
                Photo = photoUrl,
                Participants = groupParticipants
                    .ToDictionary(
                        participant => participant.Key,
                        participant => participant.Value
                    ),
                CreatedBy = group.CreatedBy,
                CreatedDate = group.CreatedDate
            };

            await _groupRepository.CreateOrUpdateGroupAsync(groupId, group);
        }


        public async Task<Dictionary<string, GroupProfile>> GetGroupProfileAsync(string userId, string chatId)
        {
            if (String.IsNullOrEmpty(chatId))
            {
                throw new BadRequestException("chatId gereklidir.");
            }

            var chatParticipants = await _chatRepository.GetChatParticipantsAsync("Group", chatId) ?? throw new NotFoundException("Sohbet bulunamadı.");

            string groupId = chatParticipants[0];

            var group = await _groupRepository.GetGroupByIdAsync(groupId) ?? throw new NotFoundException("Grup bulunamadı.");

            if (!group.Participants.ContainsKey(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            Dictionary<string, ParticipantProfile> groupUsers = [];

            foreach (var participantId in group.Participants.Keys)
            {
                var user = await _userRepository.GetUserByIdAsync(participantId);

                var participant = new ParticipantProfile
                {
                    DisplayName = user.DisplayName,
                    ProfilePhoto = user.ProfilePhoto,
                    Role = group.Participants[participantId]
                };

                groupUsers.Add(participantId, participant);
            }

            var groupProfile = new Dictionary<string, GroupProfile>
            {
                { 
                    groupId,
                    new GroupProfile 
                    {
                        Name = group.Name,
                        Description = group.Description,
                        PhotoUrl = group.Photo,
                        Participants = groupUsers,
                        CreatedDate = group.CreatedDate,
                    }
                }
            };

            return groupProfile;
        }


        public async Task LeaveGroupAsync(string userId, string groupId)
        {
            if (String.IsNullOrEmpty(groupId))
            {
                throw new BadRequestException("groupId gereklidir.");
            }

            var group = await _groupRepository.GetGroupByIdAsync(groupId) ?? throw new NotFoundException("Grup bulunamadı.");

            if (!group.Participants.ContainsKey(userId))
            {
                throw new ForbiddenException("Sohbet üzerinde yetkiniz yok.");
            }

            group.Participants.Remove(userId);

            if (!group.Participants.Count.Equals(0) && !group.Participants.ContainsValue(GroupParticipant.Admin))
            {
                group.Participants[group.Participants.Keys.ElementAt(new Random().Next(group.Participants.Count))] = GroupParticipant.Admin;
            }

            await _groupRepository.UpdateGroupParticipantsAsync(groupId, group.Participants);
        }
    }
}