using HCH.Data;
using HCH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<ICollection<HCHWebUser>> GetTherapistsByProfile(string profile)
        {
            return await this.context.Users
                .Include(x => x.Profile)
                .Where(x => x.Profile.Name == profile)
                .ToListAsync();
        }

        public async Task<ICollection<HCHWebUser>> GetAllUsersWithoutProfile()
        {
            return await this.context.Users
                .Include(x => x.Profile)
                .Where(x => x.Profile.Id == null)
                .ToListAsync();
        }

        public HCHWebUser GetUserById(string id)
        {
            return this.context.Users.Include(x => x.Profile).FirstOrDefault(x => x.Id == id);
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
                
        public HCHWebUser GetUserByFullName(string fullName)
        {
            return this.context.Users.FirstOrDefault(x => (x.FirstName + " " + x.LastName) == fullName);
        }

        public async Task<ICollection<HCHWebUser>> GetTherapistsByProfileId(string profileId)
        {
            return await this.context.Users
                .Include(x => x.Profile)
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
