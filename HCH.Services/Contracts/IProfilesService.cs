using HCH.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HCH.Services
{
    public interface IProfilesService
    {
        Profile GetProfileByName(string profile);

        Task<ICollection<Profile>> All();
        Task AddAsync(Profile profile);
        Task<Profile> GetProfileById(string id);
        Task RemoveProfileAsync(string id);
        bool ProfileExists(string id);
        Task UpdateProfileAsync(string id, string profileName, string profileDescription);
    }
}