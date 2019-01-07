using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HCH.Services.Tests
{
    public class ProfilesServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public ProfilesServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<IProfilesService, ProfilesService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }

        //GetProfileById
        [Fact]
        public void GetProfileById_ShouldReturnProfileForExistingId()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };

            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            var otherProfile = new Profile
            {
                Id = "OtherProfileId",
                Name = "OtherProfile",
                Description = "OtherDescription"
            };

            this.context.Profiles.Add(otherProfile);
            this.context.SaveChanges();

            //Act
            var result = profilesService.GetProfileById("ProfileId").Result;

            //Assert
            result.ShouldBeSameAs(profile);
        }

        [Fact]
        public void GetProfileById_ShouldReturnNullForNonExistingId()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };

            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            var otherProfile = new Profile
            {
                Id = "OtherProfileId",
                Name = "OtherProfile",
                Description = "OtherDescription"
            };

            this.context.Profiles.Add(otherProfile);
            this.context.SaveChanges();

            //Act
            var result = profilesService.GetProfileById("OtherId").Result;

            //Assert
            result.ShouldBeNull();
        }

        //GetProfileByName
        [Fact]
        public void GetProfileByName_ShouldReturnProfileForExistingName()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };

            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            var otherProfile = new Profile
            {
                Id = "OtherProfileId",
                Name = "OtherProfile",
                Description = "OtherDescription"
            };

            this.context.Profiles.Add(otherProfile);
            this.context.SaveChanges();

            //Act
            var result = profilesService.GetProfileByName("Profile");

            //Assert
            result.ShouldBeSameAs(profile);
        }

        [Fact]
        public void GetProfileByName_ShouldReturnNullForNonExistingName()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };

            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            var otherProfile = new Profile
            {
                Id = "OtherProfileId",
                Name = "OtherProfile",
                Description = "OtherDescription"
            };

            this.context.Profiles.Add(otherProfile);
            this.context.SaveChanges();

            //Act
            var result = profilesService.GetProfileByName("OtherName");

            //Assert
            result.ShouldBeNull();
        }

        //All
        [Fact]
        public void All_ShouldReturnAllprofilesDb()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };

            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            var otherProfile = new Profile
            {
                Id = "OtherProfileId",
                Name = "OtherProfile",
                Description = "OtherDescription"
            };

            this.context.Profiles.Add(otherProfile);
            this.context.SaveChanges();

            //Act
            var resultCollection = profilesService.All().Result;
            var count = resultCollection.Count();

            bool containesAll = resultCollection.Contains(profile) && resultCollection.Contains(otherProfile);

            var countDb = this.context.Profiles.Count();

            //Assert
            count.ShouldBe(countDb);

            containesAll.ShouldBeTrue();
        }

        //AddProfileAsync
        [Fact]
        public void AddProfile_ShouldReturnTrueForExistCheck()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };

            //Act
            profilesService.AddProfileAsync(profile).Wait();

            bool existProfiledb = this.context.Profiles.Contains(profile);

            //Assert
            existProfiledb.ShouldBeTrue();
        }

        //RemoveProfileAsync
        [Fact]
        public void RemoveProfile_ShouldReturnFalseForExistCheck()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };
            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            //Act
            profilesService.RemoveProfileAsync("ProfileId").Wait();

            bool existProfiledb = this.context.Profiles.Contains(profile);

            //Assert
            existProfiledb.ShouldBeFalse();
        }

        //UpdateProfileAsync
        [Fact]
        public void UpdateProfile_ShouldReturnUpdatedProfile()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };
            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            //Act
            profilesService.UpdateProfileAsync("ProfileId", "NewProfile", "NewDescription").Wait();

            var profileDb = this.context.Profiles.Find("ProfileId");

            //Assert
            profileDb.Name.ShouldBe("NewProfile");
            profileDb.Description.ShouldBe("NewDescription");
        }

        //ProfileExists
        [Fact]
        public void ProfileExists_ShouldReturnTrueForExistingId()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };
            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            //Act
            var result = profilesService.ProfileExists("ProfileId");

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void ProfileExists_ShouldReturnFalseForNonExistingId()
        {
            //Arrange
            var profilesService = this.serviceProvider.GetService<IProfilesService>();

            //seed data
            var profile = new Profile
            {
                Id = "ProfileId",
                Name = "Profile",
                Description = "Description"
            };
            this.context.Profiles.Add(profile);
            this.context.SaveChanges();

            //Act
            var result = profilesService.ProfileExists("OtherId");

            //Assert
            result.ShouldBeFalse();
        }
    }
}

