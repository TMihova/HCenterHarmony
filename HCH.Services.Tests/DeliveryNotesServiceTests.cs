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
    public class DeliveryNotesServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public DeliveryNotesServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<IDeliveryNotesService, DeliveryNotesService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }

        //AllAsync
        [Fact]
        public void All_ShouldReturnAllDeliveryNotes()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var newDeliveryNote = new DeliveryNote
            {
                Id = 1,
                IssueDate = DateTime.Today,
                Cost = 200,
                Discount = 0.0m,
                OrderId = 2
            };
            this.context.DeliveryNotes.Add(newDeliveryNote);
            this.context.SaveChanges();

            //Act            
            var resultCollection = deliveryNotesService.AllAsync().Result;
            var resultCount = resultCollection.ToList().Count;

            //Assert
            resultCount.ShouldBe(1);

            var deliveryNoteDb = resultCollection.FirstOrDefault();

            deliveryNoteDb.ShouldBeSameAs(newDeliveryNote);
        }

        //AddDeliveryNoteForOrder
        [Fact]
        public void AddDeliveryNoteForOrder_ShouldReturnDeliveryNoteWithGivenOrderId()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var order = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1
                    }
                }
            };
            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act  
            deliveryNotesService.AddDeliveryNoteForOrder(order.Id, 100, 0.0m).Wait();

            //Assert
            bool ThereIsDeliveryNoteForOrder = this.context.DeliveryNotes.Any(x => x.OrderId == order.Id);

            ThereIsDeliveryNoteForOrder.ShouldBe(true);
        }

        //GetDeliveryNoteForOrderAsync
        [Fact]
        public void GetDeliveryNoteForOrder_ShouldReturnDeliveryNoteWithOrderId()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            var deliveryNoteDb = deliveryNotesService.GetDeliveryNoteForOrderAsync(4).Result;

            //Assert
            deliveryNoteDb.OrderId.ShouldBe(4);
        }

        //GetDeliveryNoteByIdAsync
        [Fact]
        public void GetDeliveryNoteById_ShouldReturnDeliveryNoteWithGivenId()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            var deliveryNoteDb = deliveryNotesService.GetDeliveryNoteByIdAsync(1).Result;

            //Assert
            deliveryNoteDb.ShouldBeSameAs(deliveryNote);
        }

        [Fact]
        public void GetDeliveryNoteById_ShouldReturnNullForNotContainedEntity()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            var deliveryNoteDb = deliveryNotesService.GetDeliveryNoteByIdAsync(2).Result;

            //Assert
            deliveryNoteDb.ShouldBeNull();
        }

        //RemoveDeliveryNoteAsync
        [Fact]
        public void RemoveDeliveryNote_ShouldReturnFalseForExistCheck()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            deliveryNotesService.RemoveDeliveryNoteAsync(deliveryNote).Wait();

            //Assert

            bool deliveryNoteExists = this.context.DeliveryNotes.Any(x => x.Id == 1);
            deliveryNoteExists.ShouldBe(false);
        }

        //IsThereDeliveryNoteForOrder
        [Fact]
        public void IsThereDeliveryNoteForOrder_ShouldReturnTrueForProperOrderId()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            bool result = deliveryNotesService.IsThereDeliveryNoteForOrder(4);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public void IsThereDeliveryNoteForOrder_ShouldReturnFalseForWrongOrderId()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            bool result = deliveryNotesService.IsThereDeliveryNoteForOrder(5);

            //Assert
            result.ShouldBe(false);
        }

        //DeliveryNoteExists
        [Fact]
        public void DeliveryNoteExists_ShouldReturnTrueForExistingDeliveryNote()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            bool result = deliveryNotesService.DeliveryNoteExists(1);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public void DeliveryNoteExists_ShouldReturnFalseForNonExistingDeliveryNote()
        {
            //Arrange
            var deliveryNotesService = this.serviceProvider.GetService<IDeliveryNotesService>();

            //seed data
            var deliveryNote = new DeliveryNote
            {
                Id = 1,
                OrderId = 4,
                IssueDate = DateTime.Today,
                Cost = 60,
                Discount = 0.0m
            };
            this.context.DeliveryNotes.Add(deliveryNote);
            this.context.SaveChanges();

            //Act  
            bool result = deliveryNotesService.DeliveryNoteExists(2);

            //Assert
            result.ShouldBe(false);
        }
    }
}

