using HCH.Data;
using HCH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HCH.Services.Tests
{
    public class TherapistsServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;
        private readonly UserManager<HCHWebUser> userManager;

        public TherapistsServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddIdentity<HCHWebUser, IdentityRole>()
            .AddEntityFrameworkStores<HCHWebContext>()
            .AddDefaultTokenProviders();

            service.AddScoped<ITherapistsService, TherapistsService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();

            this.userManager = serviceProvider.GetRequiredService<UserManager<HCHWebUser>>();
        }

        //GetTherapistsByProfile
        [Fact]
        public void GetTherapistsByProfile_ShouldReturnTherpistsFromProfile()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirtsProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var secondProfile = new Profile
            {
                Id = "SecondProfileId",
                Name = "SecondProfile",
                Description = "SecondDescription"
            };
            this.context.Profiles.Add(secondProfile);
            this.context.SaveChanges();

            var firstTherapist = new HCHWebUser
            {
                Id = "FirstTherapistId",
                Email = "firstTherapist@email.bg",
                UserName = "firstTherapist",
                FirstName = "Pesho",
                LastName = "Peshev",
                Profile = firstProfile,
                ProfileId = "FirtsProfileId"
            };
            this.context.Users.Add(firstTherapist);
            this.context.SaveChanges();

            var secondTherapist = new HCHWebUser
            {
                Id = "SecondTherapistId",
                Email = "secondTherapist@email.bg",
                UserName = "secondTherapist",
                FirstName = "Gosho",
                LastName = "Goshev",
                Profile = secondProfile,
                ProfileId = "SecondProfileId"
            };
            this.context.Users.Add(secondTherapist);
            this.context.SaveChanges();

            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            var resultCollection = therapistsService.GetTherapistsByProfile("FirtsProfile").Result;

            var countdb = this.context.Users.Where(x => x.Profile != null && x.Profile.Name == "FirtsProfile").Count();

            var countResult = resultCollection.Count();

            //Assert
            countResult.ShouldBe(countdb);
        }

        [Fact]
        public void GetTherapistsByProfile_ShouldReturnEmptyCollectionForNonExistingProfile()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirtsProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var secondProfile = new Profile
            {
                Id = "SecondProfileId",
                Name = "SecondProfile",
                Description = "SecondDescription"
            };
            this.context.Profiles.Add(secondProfile);
            this.context.SaveChanges();

            var firstTherapist = new HCHWebUser
            {
                Id = "FirstTherapistId",
                Email = "firstTherapist@email.bg",
                UserName = "firstTherapist",
                FirstName = "Pesho",
                LastName = "Peshev",
                Profile = firstProfile,
                ProfileId = "FirtsProfileId"
            };
            this.context.Users.Add(firstTherapist);
            this.context.SaveChanges();

            var secondTherapist = new HCHWebUser
            {
                Id = "SecondTherapistId",
                Email = "secondTherapist@email.bg",
                UserName = "secondTherapist",
                FirstName = "Gosho",
                LastName = "Goshev",
                Profile = secondProfile,
                ProfileId = "SecondProfileId"
            };
            this.context.Users.Add(secondTherapist);
            this.context.SaveChanges();

            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            var resultCollection = therapistsService.GetTherapistsByProfile("NonExistingProfile").Result;

            var countResult = resultCollection.Count();

            //Assert
            countResult.ShouldBe(0);
        }

        //GetAllUsersWithoutProfile
        [Fact]
        public void GetAllUsersWithoutProfile_ShouldReturnAllUsersWithoutProfile()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirtsProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var secondProfile = new Profile
            {
                Id = "SecondProfileId",
                Name = "SecondProfile",
                Description = "SecondDescription"
            };
            this.context.Profiles.Add(secondProfile);
            this.context.SaveChanges();

            var firstTherapist = new HCHWebUser
            {
                Id = "FirstTherapistId",
                Email = "firstTherapist@email.bg",
                UserName = "firstTherapist",
                FirstName = "Pesho",
                LastName = "Peshev",
                Profile = firstProfile,
                ProfileId = "FirtsProfileId"
            };
            this.context.Users.Add(firstTherapist);
            this.context.SaveChanges();

            var secondTherapist = new HCHWebUser
            {
                Id = "SecondTherapistId",
                Email = "secondTherapist@email.bg",
                UserName = "secondTherapist",
                FirstName = "Gosho",
                LastName = "Goshev",
                Profile = secondProfile,
                ProfileId = "SecondProfileId"
            };
            this.context.Users.Add(secondTherapist);
            this.context.SaveChanges();

            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            var resultCollection = therapistsService.GetAllUsersWithoutProfile().Result;

            var countdb = this.context.Users.Where(x => x.Profile == null).Count();

            var countResult = resultCollection.Count();

            bool existUserWithProfile = resultCollection.Any(x => x.ProfileId != null);

            //Assert
            existUserWithProfile.ShouldBeFalse();
        }

        //GetUserByIdAsync
        [Fact]
        public void GetUserById_ShouldReturnUserWithGivenId()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirtsProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var firstTherapist = new HCHWebUser
            {
                Id = "FirstTherapistId",
                Email = "firstTherapist@email.bg",
                UserName = "firstTherapist",
                FirstName = "Pesho",
                LastName = "Peshev",
                Profile = firstProfile,
                ProfileId = "FirtsProfileId"
            };
            this.context.Users.Add(firstTherapist);
            this.context.SaveChanges();
            

            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            var resultUser = therapistsService.GetUserByIdAsync("UserId").Result;

            var userDb = this.context.Users.Find("UserId");

            //Assert
            resultUser.ShouldBeSameAs(userDb);
        }

        [Fact]
        public void GetUserById_ShouldReturnNullForNonExistingId()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirtsProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var firstTherapist = new HCHWebUser
            {
                Id = "FirstTherapistId",
                Email = "firstTherapist@email.bg",
                UserName = "firstTherapist",
                FirstName = "Pesho",
                LastName = "Peshev",
                Profile = firstProfile,
                ProfileId = "FirtsProfileId"
            };
            this.context.Users.Add(firstTherapist);
            this.context.SaveChanges();


            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            var resultUser = therapistsService.GetUserByIdAsync("NonExistingId").Result;

            //Assert
            resultUser.ShouldBeNull();
        }

        //AddProfileToUser
        [Fact]
        public void AddProfileToUser_ShouldReturnTrueForCheckProfileOfUser()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirstProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();
            
            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            therapistsService.AddProfileToUser(user, "FirstProfile");

            bool userHasProfile = this.context.Users.Find("UserId").ProfileId == "FirtsProfileId";

            //Assert
            userHasProfile.ShouldBeTrue();
        }

        [Fact]
        public void AddProfileToUser_ShouldReturnFalseForCheckNonExistingProfileOfUser()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirstProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            therapistsService.AddProfileToUser(user, "NonExistingProfile");

            bool userHasProfile = this.context.Users.Find("UserId").ProfileId != null;

            //Assert
            userHasProfile.ShouldBeFalse();
        }

        //GetTherapistsByProfileId
        [Fact]
        public void GetTherapistsByProfileId_ShouldReturnTherapistsWithGivenProfile()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirstProfileId",
                Name = "FirstProfile",
                Description = "FirstDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var secondProfile = new Profile
            {
                Id = "SecondProfileId",
                Name = "SecondProfile",
                Description = "SecondDescription"
            };
            this.context.Profiles.Add(secondProfile);
            this.context.SaveChanges();

            var firstTherapist = new HCHWebUser
            {
                Id = "FirstTherapistId",
                Email = "firstTherapist@email.bg",
                UserName = "firstTherapist",
                FirstName = "Pesho",
                LastName = "Peshev",
                Profile = firstProfile,
                ProfileId = "FirstProfileId"
            };
            this.context.Users.Add(firstTherapist);
            this.context.SaveChanges();

            var secondTherapist = new HCHWebUser
            {
                Id = "SecondTherapistId",
                Email = "secondTherapist@email.bg",
                UserName = "secondTherapist",
                FirstName = "Gosho",
                LastName = "Goshev",
                Profile = secondProfile,
                ProfileId = "SecondProfileId"
            };
            this.context.Users.Add(secondTherapist);
            this.context.SaveChanges();

            //Act
            var resultTherapists = therapistsService.GetTherapistsByProfileId("FirstProfileId").Result.ToList();

            var countResult = resultTherapists.Count();

            var therapistsDb = this.context.Users.Where(x => x.ProfileId == "FirstProfileId").ToList();

            var countDb = therapistsDb.Count();

            var user = resultTherapists.FirstOrDefault();

            //Assert
            countResult.ShouldBe(countDb);

            user.ShouldBeSameAs(firstTherapist);
        }

        [Fact]
        public void GetTherapistsByProfileId_ShouldReturnEmptyCollectionForNonExistingProfile()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirstProfileId",
                Name = "FirstProfile",
                Description = "FirstDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var secondProfile = new Profile
            {
                Id = "SecondProfileId",
                Name = "SecondProfile",
                Description = "SecondDescription"
            };
            this.context.Profiles.Add(secondProfile);
            this.context.SaveChanges();

            var firstTherapist = new HCHWebUser
            {
                Id = "FirstTherapistId",
                Email = "firstTherapist@email.bg",
                UserName = "firstTherapist",
                FirstName = "Pesho",
                LastName = "Peshev",
                Profile = firstProfile,
                ProfileId = "FirtsProfileId"
            };
            this.context.Users.Add(firstTherapist);
            this.context.SaveChanges();

            var secondTherapist = new HCHWebUser
            {
                Id = "SecondTherapistId",
                Email = "secondTherapist@email.bg",
                UserName = "secondTherapist",
                FirstName = "Gosho",
                LastName = "Goshev",
                Profile = secondProfile,
                ProfileId = "SecondProfileId"
            };
            this.context.Users.Add(secondTherapist);
            this.context.SaveChanges();

            //Act
            var resultTherapists = therapistsService.GetTherapistsByProfileId("NonExistingProfileId").Result;

            var count = resultTherapists.Count();

            //Assert
            count.ShouldBe(0);
        }

        //RemoveProfileFromUser
        [Fact]
        public void RemoveProfileFromUser_ShouldReturnTrueForCheckNullProfileOfUser()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirstProfileId",
                Name = "FirstProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev",
                Profile = firstProfile,
                ProfileId = "FirstProfileId"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            therapistsService.RemoveProfileFromUser(user);

            bool userHasNullProfile = this.context.Users.Find("UserId").ProfileId == null;

            //Assert
            userHasNullProfile.ShouldBeTrue();
        }

        [Fact]
        public void RemoveProfileFromUser_ShouldReturnTrueForCheckNullProfileOfUserWithoutProfile()
        {
            //Arrange
            var therapistsService = this.serviceProvider.GetService<ITherapistsService>();

            //seed data
            var firstProfile = new Profile
            {
                Id = "FirtsProfileId",
                Name = "FirstProfile",
                Description = "FirtsDescription"
            };
            this.context.Profiles.Add(firstProfile);
            this.context.SaveChanges();

            var user = new HCHWebUser
            {
                Id = "UserId",
                Email = "user@email.bg",
                UserName = "user",
                FirstName = "Tosho",
                LastName = "Toshev"
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            therapistsService.RemoveProfileFromUser(user);

            bool userHasNullProfile = this.context.Users.Find("UserId").ProfileId == null;

            //Assert
            userHasNullProfile.ShouldBeTrue();
        }
    }
}

