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
    public class UserssServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;
        private readonly UserManager<HCHWebUser> userManager;

        public UserssServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddIdentity<HCHWebUser, IdentityRole>()
            .AddEntityFrameworkStores<HCHWebContext>()
            .AddDefaultTokenProviders();

            service.AddScoped<IUsersService, UsersService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();

            this.userManager = serviceProvider.GetRequiredService<UserManager<HCHWebUser>>();
        }

        //AllAsync
        [Fact]
        public void All_ShouldReturnAllUsersDb()
        {
            //Arrange
            var usersService = this.serviceProvider.GetService<IUsersService>();

            //seed data 
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
            var resultCollection = usersService.AllAsync().Result;
            var count = resultCollection.Count();

            bool result = resultCollection.Contains(user);

            //Assert
            count.ShouldBe(1);
            result.ShouldBeTrue();
        }

        //GetUserById
        [Fact]
        public void GetUserById_ShouldReturnUserWithGivenIdDb()
        {
            //Arrange
            var usersService = this.serviceProvider.GetService<IUsersService>();

            //seed data 
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

            var secondUser = new HCHWebUser
            {
                Id = "SecondUserId",
                Email = "seconduser@email.bg",
                UserName = "seconduser",
                FirstName = "Pesho",
                LastName = "Peshev"
            };
            this.context.Users.Add(secondUser);
            this.context.SaveChanges();

            //Act
            var resultUser = usersService.GetUserById("SecondUserId");

            //Assert
            resultUser.ShouldBeSameAs(secondUser);
        }

        [Fact]
        public void GetUserById_ShouldReturnNullForNonExistingId()
        {
            //Arrange
            var usersService = this.serviceProvider.GetService<IUsersService>();

            //seed data 
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

            var secondUser = new HCHWebUser
            {
                Id = "SecondUserId",
                Email = "seconduser@email.bg",
                UserName = "seconduser",
                FirstName = "Pesho",
                LastName = "Peshev"
            };
            this.context.Users.Add(secondUser);
            this.context.SaveChanges();

            //Act
            var resultUser = usersService.GetUserById("NonExistingId");

            //Assert
            resultUser.ShouldBeNull();
        }

        //GetUserByUsername
        [Fact]
        public void GetUserByUsername_ShouldReturnUserWithGivenNameDb()
        {
            //Arrange
            var usersService = this.serviceProvider.GetService<IUsersService>();

            //seed data 
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

            var secondUser = new HCHWebUser
            {
                Id = "SecondUserId",
                Email = "seconduser@email.bg",
                UserName = "seconduser",
                FirstName = "Pesho",
                LastName = "Peshev"
            };
            this.context.Users.Add(secondUser);
            this.context.SaveChanges();

            //Act
            var resultUser = usersService.GetUserByUsername("seconduser");

            //Assert
            resultUser.ShouldBeSameAs(secondUser);
        }

        [Fact]
        public void GetUserByUsername_ShouldReturnNullForNonExistingUsername()
        {
            //Arrange
            var usersService = this.serviceProvider.GetService<IUsersService>();

            //seed data 
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

            var secondUser = new HCHWebUser
            {
                Id = "SecondUserId",
                Email = "seconduser@email.bg",
                UserName = "seconduser",
                FirstName = "Pesho",
                LastName = "Peshev"
            };
            this.context.Users.Add(secondUser);
            this.context.SaveChanges();

            //Act
            var resultUser = usersService.GetUserByUsername("nonexisting");

            //Assert
            resultUser.ShouldBeNull();
        }
    }
}

