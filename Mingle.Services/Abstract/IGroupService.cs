using Mingle.Services.DTOs.Request;
using Mingle.Services.DTOs.Response;

namespace Mingle.Services.Abstract
{
    public interface IGroupService
    {
        Task<List<string>> GetUserGroupsIdsAsync(string userId);

        Task<Dictionary<string, GroupProfile>> CreateGroupAsync(string userId, CreateGroup dto);

        Task<Dictionary<string, GroupProfile>> EditGroupAsync(string userId, string groupId, CreateGroup dto);

        Task<Dictionary<string, GroupProfile>> GetGroupProfilesAsync(List<string> userGroupIds);

        Task<List<string>> GetGroupParticipantsAsync(string userId, string groupId);

        Task<Dictionary<string, GroupProfile>> LeaveGroupAsync(string userId, string groupId);
    }
}