using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface ITherapistsService
    {
        Task<ICollection<HCHWebUser>> GetTherapistsByProfile(string profile);

        Task<ICollection<HCHWebUser>> GetAllUsersWithoutProfile();

        HCHWebUser GetUserById(string id);
        void AddProfileToUser(HCHWebUser user, string profile);
        HCHWebUser GetUserByFullName(string fullName);
        Task<ICollection<HCHWebUser>> GetTherapistsByProfileId(string profileId);
        string GetProfileNameByProfileId(string profileId);
        void RemoveProfileFromUser(HCHWebUser user);
    }
}