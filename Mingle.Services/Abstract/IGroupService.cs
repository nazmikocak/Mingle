using Mingle.Entities.Models;
using Mingle.Services.DTOs.Request;

namespace Mingle.Services.Abstract
{
    public interface IGroupService
    {
        Task<string> CreateGroupAsync(string userId, CreateGroup dto);

        Task EditGroupAsync(string userId, string groupId, CreateGroup dto);

        Task LeaveGroupAsync(string userId, string groupId);
    }
}