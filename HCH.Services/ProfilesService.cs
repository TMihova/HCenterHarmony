using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class ProfilesService : IProfilesService
    {
        private readonly HCHWebContext context;

        public ProfilesService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task<Profile> GetProfileById(string id)
        {
            return await this.context.Profiles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Profile GetProfileByName(string profile)
        {
            return this.context.Profiles.SingleOrDefault(x => x.Name == profile);
        }

        public async Task<ICollection<Profile>> All()
        {
            var profiles = await this.context.Profiles.ToListAsync();

            return profiles;
        }

        public async Task AddProfileAsync(Profile profile)
        {
            this.context.Profiles.Add(profile);
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveProfileAsync(string id)
        {
            var profile = await this.context.Profiles.FindAsync(id);
            this.context.Profiles.Remove(profile);
            await this.context.SaveChangesAsync();
        }        

        public async Task UpdateProfileAsync(string id, string profileName, string profileDescription)
        {
            var profileDb = await this.context.Profiles.FindAsync(id);

            profileDb.Name = profileName;
            profileDb.Description = profileDescription;

            this.context.Update(profileDb);
            await this.context.SaveChangesAsync();
        }

        public bool ProfileExists(string id)
        {
            return this.context.Profiles.Any(e => e.Id == id);
        }
    }
}
