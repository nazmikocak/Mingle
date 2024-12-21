using Mingle.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mingle.Services.Abstract
{
    public interface IUserService
    {
        Task<UserProfileResponse> GetUserProfileAsync(string userId);

        Task RemoveProfilePhotoAsync(string userId);
    }
}
