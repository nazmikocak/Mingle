//using AutoMapper;
//using Mingle.DataAccess.Abstract;
//using Mingle.Entities.Enums;
//using Mingle.Entities.Models;
//using Mingle.Services.Abstract;
//using Mingle.Services.DTOs.Request;
//using Mingle.Services.Exceptions;
//using Mingle.Services.Utilities;
//using System.Reflection;
//using System.Text.Json;

//namespace Mingle.Services.Concrete
//{
//    public sealed class GroupService : IGroupService
//    {
//        private readonly IGroupRepository _groupChatRepository;
//        private readonly IUserRepository _userRepository;
//        private readonly ICloudRepository _cloudRepository;
//        private readonly IMapper _mapper;



//        public GroupService(IGroupRepository groupChatRepository, IUserRepository userRepository, ICloudRepository cloudRepository, IMapper mapper)
//        {
//            _groupChatRepository = groupChatRepository;
//            _userRepository = userRepository;
//            _cloudRepository = cloudRepository;
//            _mapper = mapper;
//        }


//        public async Task GroupAsync(string userId, CreateGroup dto)
//        {
//            var requestParticipants = JsonSerializer.Deserialize<Dictionary<string, GroupParticipant>>(dto.Participants);

//            if (requestParticipants.ContainsValue(GroupParticipant.Admin) || requestParticipants.ContainsValue(GroupParticipant.Member))
//            {
//                throw new BadRequestException("Geçersiz bir rol seçildi.");
//            }

//            string groupId = Guid.NewGuid().ToString();
//            Uri groupPhotoUrl;

//            if (dto.Photo != null)
//            {
//                const int maxFileSize = 5 * 1024 * 1024;
//                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg", "webp " };
//                FileValidationHelper.ValidatePhoto(dto.Photo, maxFileSize, allowedExtensions);

//                groupPhotoUrl = await _cloudRepository.UploadPhotoAsync(groupId, "Groups", "group_photo", dto.Photo);
//            }
//            else
//            {
//                groupPhotoUrl = new Uri("https://res.cloudinary.com/mingle-realtime-messaging-app/image/upload/v1734185322/DefaultGroupProfileImage.png");
//            }

//            var group = new Group
//            {
//                Name = dto.Name,
//                Description = String.IsNullOrEmpty(dto.Description)
//                    ? "Merhaba, bu bir Mingle grubudur."
//                    : dto.Description,
//                Photo = groupPhotoUrl,
//                Participants = new Dictionary<string, GroupParticipant>
//                {
//                    { $"{userId}", GroupParticipant.Admin }
//                },
//                CreatedBy = userId,
//                CreatedDate = DateTime.Now,
//            };

//            foreach (var participant in requestParticipants)
//            {
//                group.Participants[participant.Key] = participant.Value;
//            }

//            await _groupChatRepository.CreateGroupAsync(groupId, group);

//            string chatId = Guid.NewGuid().ToString();

//            var chat = new Chat
//            {
//                Participants = [groupId],
//                CreatedDate = DateTime.UtcNow,
//            };


//        }
//    }
//}