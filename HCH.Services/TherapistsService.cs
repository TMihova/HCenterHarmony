using HCH.Data;
using HCH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class TherapistsService : ITherapistsService
    {
        private readonly HCHWebContext context;
        private readonly UserManager<HCHWebUser> userManager;

        public TherapistsService(HCHWebContext context,
            UserManager<HCHWebUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<HCHWebUser>> GetTherapistsByProfile(string profile)
        {
            return await this.context.Users
                .Where(x => x.Profile != null && x.Profile.Name == profile)
                .ToListAsync();
        }

        public async Task<IEnumerable<HCHWebUser>> GetAllUsersWithoutProfile()
        {
            return await this.context.Users
                .Where(x => x.Profile.Id == null)
                .ToListAsync();
        }

        public async Task<HCHWebUser> GetUserByIdAsync(string id)
        {
            return await this.context.Users.FindAsync(id);
        }

        public void AddProfileToUser(HCHWebUser user, string profile)
        {
            var profileDb = this.context.Profiles.FirstOrDefault(x => x.Name == profile);

            if (profileDb != null)
            {
                user.ProfileId = profileDb.Id;
                user.Profile = profileDb;
                this.context.Update(user);
                this.context.SaveChanges();
            }
        }

        public async Task<IEnumerable<HCHWebUser>> GetTherapistsByProfileId(string profileId)
        {
            return await this.context.Users
                .Where(x => x.Profile.Id == profileId)
                .ToListAsync();
        }

        public string GetProfileNameByProfileId(string profileId)
        {
            return this.context.Profiles
                .Where(x => x.Id == profileId)
                .Select(x => x.Name)
                .SingleOrDefault();
        }

        public void RemoveProfileFromUser(HCHWebUser user)
        {
            user.ProfileId = null;
            user.Profile = null;
            this.context.SaveChanges();
        }
    }
}
