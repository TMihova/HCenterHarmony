using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface IUsersService
    {
        Task<ICollection<HCHWebUser>> AllAsync();
        HCHWebUser GetUserById(string id);
        IList<string> GetRolesAsync(HCHWebUser user);
        Task AddToRoleAsync(HCHWebUser user, string adminRole);
        Task RemoveFromRoleAsync(HCHWebUser user, string adminRole);
        HCHWebUser GetUserByUsername(string username);
        bool IsInRole(string username, string role);
    }
}