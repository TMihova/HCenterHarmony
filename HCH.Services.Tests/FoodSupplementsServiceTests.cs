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
    public class FoodSupplementsServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public FoodSupplementsServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<IFoodSupplementsService, FoodSupplementsService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }

        //AddProductAsync
        [Fact]
        public void AddProduct_ShouldReturnTrueForExistCheck()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };

            //Act
            foodSupplementsService.AddProductAsync(foodSupplement).Wait();

            //Assert
            var foodSupplementDb = this.context.FoodSupplements.FirstOrDefault(x => x.Id == 1);
            var count = this.context.FoodSupplements.Count();

            count.ShouldBe(1);
            foodSupplementDb.ShouldBeSameAs(foodSupplement);
        }

        //AllAsync
        [Fact]
        public void All_ShouldReturnAllProducts()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            //Act
            var resultCollection = foodSupplementsService.AllAsync().Result;

            //Assert
            var foodSupplementDb = resultCollection.FirstOrDefault(x => x.Id == 1);
            var count = resultCollection.Count();

            count.ShouldBe(1);
            foodSupplementDb.ShouldBeSameAs(foodSupplement);
        }

        //GetProductById
        [Fact]
        public void GetProductById_ShouldReturnProductWithGivenId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            var otherFoodSupplement = new FoodSupplement
            {
                Id = 2,
                Description = "Description2",
                Name = "FoodSupplement2",
                Price = 20
            };
            this.context.FoodSupplements.Add(otherFoodSupplement);
            this.context.SaveChanges();

            //Act
            var product = foodSupplementsService.GetProductById(1).Result;

            //Assert
            product.ShouldBeSameAs(foodSupplement);
        }

        [Fact]
        public void GetProductById_ShouldReturnNullForNonExistingId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            var otherFoodSupplement = new FoodSupplement
            {
                Id = 2,
                Description = "Description2",
                Name = "FoodSupplement2",
                Price = 20
            };
            this.context.FoodSupplements.Add(otherFoodSupplement);
            this.context.SaveChanges();

            //Act
            var product = foodSupplementsService.GetProductById(21).Result;

            //Assert
            product.ShouldBeNull();
        }

        //RemoveProductAsync
        [Fact]
        public void RemoveProduct_ShouldReturnNullFromDbForGetRemovedProduct()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            var otherFoodSupplement = new FoodSupplement
            {
                Id = 2,
                Description = "Description2",
                Name = "FoodSupplement2",
                Price = 20
            };
            this.context.FoodSupplements.Add(otherFoodSupplement);
            this.context.SaveChanges();

            //Act
            foodSupplementsService.RemoveProductAsync(foodSupplement).Wait();

            var count = this.context.FoodSupplements.Count();
            var product = this.context.FoodSupplements.Find(1);

            //Assert
            count.ShouldBe(1);
            product.ShouldBeNull();
        }

        //UpdateProductAsync
        [Fact]
        public void UpdateProduct_ShouldReturnUpdatedProductDb()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            //Act
            foodSupplement.Description = "UpdatedDescription";

            foodSupplementsService.UpdateProductAsync(1, "FoodSupplement", 20, "UpdatedDescription").Wait();

            var foodSupplementDb = this.context.FoodSupplements.FirstOrDefault(x => x.Id == 1);

            //Assert
            foodSupplementDb.Description.ShouldBe("UpdatedDescription");
        }

        //FoodSupplementExists
        [Fact]
        public void FoodSupplementExists_ShouldReturnTrueForExistingProduct()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            //Act
            bool result = foodSupplementsService.FoodSupplementExists(1);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public void FoodSupplementExists_ShouldReturnFalseForNonExistingProduct()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IFoodSupplementsService>();

            //seed data
            var foodSupplement = new FoodSupplement
            {
                Id = 1,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            //Act
            bool result = foodSupplementsService.FoodSupplementExists(23);

            //Assert
            result.ShouldBe(false);
        }
    }
}

