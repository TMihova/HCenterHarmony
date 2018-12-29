using HCH.Data;
using HCH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class UsersService : IUsersService
    {
        private readonly HCHWebContext context;
        private readonly UserManager<HCHWebUser> userManager;

        public UsersService(HCHWebContext context,
            UserManager<HCHWebUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task AddToRoleAsync(HCHWebUser user, string role)
        {
            await this.userManager.AddToRoleAsync(user, role);
        }

        public async Task<ICollection<HCHWebUser>> AllAsync()
        {
            return await context.Users.Include(x => x.Profile).ToListAsync();
        }

        public IList<string> GetRolesAsync(HCHWebUser user)
        {
            var roles = this.userManager.GetRolesAsync(user).Result;

            return roles;
        }

        public HCHWebUser GetUserById(string id)
        {
            return this.context.Users.Find(id);
        }

        public HCHWebUser GetUserByUsername(string username)
        {
            return this.context.Users.FirstOrDefault(x => x.UserName == username);
        }

        public bool IsInRole(string username, string role)
        {
            var user = this.GetUserByUsername(username);

            return this.userManager.IsInRoleAsync(user, role).Result;
        }

        public async Task RemoveFromRoleAsync(HCHWebUser user, string role)
        {
            await this.userManager.RemoveFromRoleAsync(user, role);
        }
    }
}
