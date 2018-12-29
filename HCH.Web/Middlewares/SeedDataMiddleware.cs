using HCH.Data;
using HCH.Models;
using HCH.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Eventures.Web.Middlewares
{
    public class SeedDataMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IOptions<AdminSettings> config;
        private readonly IOptions<UserRoles> configRoles;

        public SeedDataMiddleware(RequestDelegate next,
            IOptions<AdminSettings> config,
            IOptions<UserRoles> configRoles)
        {
            this.next = next;
            this.config = config;
            this.configRoles = configRoles;
        }

        public async Task InvokeAsync(HttpContext httpContext, HCHWebContext dbContext, IServiceProvider serviceProvider)
        {
            await this.SeedUserRoles(serviceProvider);

            await this.CreateAdmin(serviceProvider);

            //if (!dbContext.Events.Any())
            //{
            //    SeedData(dbContext);
            //}

            await this.next(httpContext);
        }

        private async Task CreateAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<HCHWebUser>>();
            //creating a super user who could maintain the web app
            var adminUser = new HCHWebUser
            {
                UserName = config.Value.UserName,
                Email = config.Value.UserEmail,
                FirstName = config.Value.FirstName,
                LastName = config.Value.LastName
                
            };

            string UserPassword = config.Value.UserPassword;
            var _user = await userManager.FindByNameAsync(adminUser.UserName);

            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, UserPassword);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the "Admin" role 
                    await userManager.AddToRoleAsync(adminUser, configRoles.Value.AdminRole);

                }
            }
        }

        private async Task SeedUserRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<HCHWebUser>>();

            

            string[] roleNames = { configRoles.Value.AdminRole,
                configRoles.Value.TherapistRole,
                configRoles.Value.UserRole };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {

                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private void SeedData(HCHWebContext dbContext)
        {
            //for (int i = 1; i <= 10; i++)
            //{
            //    var sampleEvent = new Event
            //    {
            //        Name = $"Sample Event{i}",
            //        Place = $"Sample Place{i}",
            //        Start = DateTime.UtcNow.Add(TimeSpan.FromDays(i)),
            //        End = DateTime.UtcNow.Add(TimeSpan.FromDays(i+1)),
            //        Total = i*50,
            //        PricePerTicket = i*5
            //    };

            //    dbContext.Events.Add(sampleEvent);
            //}

            //dbContext.SaveChanges();
        }
    }
}
