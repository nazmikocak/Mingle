using Mingle.Services.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.Abstract
{
    public interface IGroupService
    {
        Task CreateGroupAsync(string userId, CreateGroup dto);
    }
}