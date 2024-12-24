using Mingle.Entities.Enums;
using Mingle.Entities.Models;

namespace Mingle.DataAccess.Abstract
{
    public interface IGroupRepository
    {
        Task CreateOrUpdateGroupAsync(string groupId, Group group);

        Task<Group> GetGroupByIdAsync(string groupId);

        Task UpdateGroupParticipantsAsync(string groupId, Dictionary<string, GroupParticipant> groupParticipants);
    }
}
