using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface ITherapistsService
    {
        Task<IEnumerable<HCHWebUser>> GetTherapistsByProfile(string profile);

        Task<IEnumerable<HCHWebUser>> GetAllUsersWithoutProfile();

        Task<IEnumerable<HCHWebUser>> GetTherapistsByProfileId(string profileId);

        Task<HCHWebUser> GetUserByIdAsync(string id);

        void AddProfileToUser(HCHWebUser user, string profile);                

        string GetProfileNameByProfileId(string profileId);

        void RemoveProfileFromUser(HCHWebUser user);
    }
}