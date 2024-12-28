using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;

namespace Mingle.Services.Abstract
{
    public interface IGroupService
    {
        Task<string> CreateGroupAsync(string userId, CreateGroup dto);

        Task EditGroupAsync(string userId, string groupId, CreateGroup dto);

        Task<Dictionary<string, GroupProfile>> GetGroupProfileByIdAsync(string userId, string groupId);

        Task<Dictionary<string, GroupProfile>> GetGroupProfilesAsync(List<string> userGroupIds);

        Task<List<string>> GetGroupParticipantsAsync(string userId, string groupId);

        Task LeaveGroupAsync(string userId, string groupId);
    }
}