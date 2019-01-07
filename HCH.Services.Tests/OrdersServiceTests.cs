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
    public class OrdersServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public OrdersServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<IOrdersService, OrdersService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }

        //AddFoodSupplementToOrderOnGivenDateAsync
        [Fact]
        public void AddFoodSupplementToOrderOnGivenDate_ShouldReturnTrueForExistCheck()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

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
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(order);
            this.context.SaveChanges();

            var foodSupplement = new FoodSupplement
            {
                Id = 11,
                Description = "Description",
                Name = "FoodSupplement",
                Price = 20
            };
            this.context.FoodSupplements.Add(foodSupplement);
            this.context.SaveChanges();

            //Act
            var date = order.OrderDate;
            foodSupplementsService.AddFoodSupplementToOrderOnGivenDateAsync(date, "ClientId", 11, 1).Wait();

            //Assert
            var productsFromOrderDb = this.context.Orders.FirstOrDefault(x => x.Id == 1)
                .FoodSupplements.ToList();

            var count = productsFromOrderDb.Count();

            bool productExists = productsFromOrderDb.Any(x => x.FoodSupplementId == 11);

            count.ShouldBe(2);
            productExists.ShouldBe(true);
        }

        //AddOrderOnGivenDateToClient
        [Fact]
        public void AddOrderOnGivenDateToClient_ShouldReturnTrueForExistsOrderForClient()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data

            //Act
            var date = DateTime.Today;
            foodSupplementsService.AddOrderOnGivenDateToClient(date, "ClientId");

            //Assert

            bool orderForClientExists = this.context.Orders
                .Any(x => x.ClientId == "ClientId");

            orderForClientExists.ShouldBe(true);
        }

        //IsThereAnyOrdersForClientOnGivenDate
        [Fact]
        public void IsThereAnyOrdersForClientOnGivenDate_ShouldReturnTrueForExistingClientOnGivenDate()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

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
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act
            var date = DateTime.Today;
            bool result = foodSupplementsService.IsThereAnyOrdersForClientOnGivenDate(date, "ClientId");

            //Assert

            result.ShouldBe(true);
        }

        //AllClientOrdersAsync
        [Fact]
        public void AllClientOrders_ShouldReturnAllOrdersForGivenClient()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

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
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act
            var resultCollection = foodSupplementsService.AllClientOrdersAsync("ClientId").Result;

            //Assert

            resultCollection.Count().ShouldBe(1);

            var orderClient = resultCollection.FirstOrDefault(x => x.ClientId == "ClientId");

            orderClient.ShouldBeSameAs(order);
        }

        //GetOrderByIdAsync
        [Fact]
        public void GetOrderById_ShouldReturnOrderForExistingId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

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
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act
            var resultOrder = foodSupplementsService.GetOrderByIdAsync(1).Result;

            //Assert

            resultOrder.ShouldBeSameAs(order);
        }

        [Fact]
        public void GetOrderById_ShouldReturnNullForNonExistingId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

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
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(order);
            this.context.SaveChanges();

            //Act
            var resultOrder = foodSupplementsService.GetOrderByIdAsync(45).Result;

            //Assert

            resultOrder.ShouldBeNull();
        }

        //GetOrderProductsByOrderIdAsync
        [Fact]
        public void GetOrderProductsByOrderId_ShouldReturnOrderProductsForExistingId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstFoodSupplement = new OrderFoodSupplement
            {
                Id = 4,
                FoodSupplementId = 6,
                OrderId = 1,
                ProductCount = 2
            };
            this.context.OrderFoodSupplements.Add(firstFoodSupplement);
            this.context.SaveChanges();

            var secondFoodSupplement = new OrderFoodSupplement
            {
                Id = 6,
                FoodSupplementId = 6,
                OrderId = 2,
                ProductCount = 1
            };
            this.context.OrderFoodSupplements.Add(secondFoodSupplement);
            this.context.SaveChanges();

            //Act
            var resultCollection = foodSupplementsService.GetOrderProductsByOrderIdAsync(1).Result;

            //Assert

            resultCollection.Count().ShouldBe(1);
        }

        [Fact]
        public void GetOrderProductsByOrderId_ShouldReturnEmptyCollectionForNonExistingId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstFoodSupplement = new OrderFoodSupplement
            {
                Id = 4,
                FoodSupplementId = 6,
                OrderId = 1,
                ProductCount = 2
            };
            this.context.OrderFoodSupplements.Add(firstFoodSupplement);
            this.context.SaveChanges();

            var secondFoodSupplement = new OrderFoodSupplement
            {
                Id = 6,
                FoodSupplementId = 6,
                OrderId = 2,
                ProductCount = 1
            };
            this.context.OrderFoodSupplements.Add(secondFoodSupplement);
            this.context.SaveChanges();

            //Act
            var resultCollection = foodSupplementsService.GetOrderProductsByOrderIdAsync(50).Result;

            //Assert

            resultCollection.Count().ShouldBe(0);
        }

        //AllOrdersAsync
        [Fact]
        public void AllOrders_ShouldReturnAllOrdersFromDb()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstOrder = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(firstOrder);
            this.context.SaveChanges();

            var secondOrder = new Order
            {
                Id = 2,
                ClientId = "OtherClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 7,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(secondOrder);
            this.context.SaveChanges();

            //Act
            var resultCollection = foodSupplementsService.AllOrdersAsync().Result;

            //Assert

            resultCollection.Count().ShouldBe(2);
        }

        //RemoveOrderAsync
        [Fact]
        public void RemoveOrder_ShouldReturnFalseForExistCheckForProperId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstOrder = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(firstOrder);
            this.context.SaveChanges();

            var secondOrder = new Order
            {
                Id = 2,
                ClientId = "OtherClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 7,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(secondOrder);
            this.context.SaveChanges();

            //Act
            foodSupplementsService.RemoveOrderAsync(2).Wait();

            bool orderExists = this.context.Orders.Any(x => x.Id == 2);

            var count = this.context.Orders.Count();

            //Assert
            orderExists.ShouldBe(false);

            count.ShouldBe(1);
        }

        [Fact]
        public void RemoveOrder_ShouldReturnTheSameCountOfOrdersForWrongId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstOrder = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(firstOrder);
            this.context.SaveChanges();

            var secondOrder = new Order
            {
                Id = 2,
                ClientId = "OtherClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 7,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(secondOrder);
            this.context.SaveChanges();

            //Act
            foodSupplementsService.RemoveOrderAsync(20).Wait();

            var count = this.context.Orders.Count();

            //Assert

            count.ShouldBe(2);
        }

        //OrderExists
        [Fact]
        public void OrderExists_ShouldReturnTrueForExistingId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstOrder = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(firstOrder);
            this.context.SaveChanges();

            var secondOrder = new Order
            {
                Id = 2,
                ClientId = "OtherClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 7,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(secondOrder);
            this.context.SaveChanges();

            //Act
             bool result = foodSupplementsService.OrderExists(1);

            //Assert

            result.ShouldBe(true);
        }

        [Fact]
        public void OrderExists_ShouldReturnFalseForNonExistingId()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstOrder = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(firstOrder);
            this.context.SaveChanges();

            var secondOrder = new Order
            {
                Id = 2,
                ClientId = "OtherClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 7,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(secondOrder);
            this.context.SaveChanges();

            //Act
            bool result = foodSupplementsService.OrderExists(100);

            //Assert

            result.ShouldBe(false);
        }

        //GetOrderFromGivenDateAsync
        [Fact]
        public void GetOrderFromGivenDate_ShouldReturnOrderForGivenClientAndDate()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstOrder = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(firstOrder);
            this.context.SaveChanges();

            var secondOrder = new Order
            {
                Id = 2,
                ClientId = "OtherClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 7,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(secondOrder);
            this.context.SaveChanges();

            //Act
            var date = DateTime.Today;
            var result = foodSupplementsService.GetOrderFromGivenDateAsync(date, "ClientId").Result;

            //Assert

            result.ShouldBeSameAs(firstOrder);
        }

        [Fact]
        public void GetOrderFromGivenDate_ShouldReturnNullForNonExistingClientAndDate()
        {
            //Arrange
            var foodSupplementsService = this.serviceProvider.GetService<IOrdersService>();

            //seed data
            var firstOrder = new Order
            {
                Id = 1,
                ClientId = "ClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 4,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(firstOrder);
            this.context.SaveChanges();

            var secondOrder = new Order
            {
                Id = 2,
                ClientId = "OtherClientId",
                OrderDate = DateTime.Today,
                FoodSupplements = new List<OrderFoodSupplement>
                { new OrderFoodSupplement
                    {   Id = 7,
                        FoodSupplementId = 6,
                        OrderId = 1,
                        ProductCount = 2
                    }
                }
            };
            this.context.Orders.Add(secondOrder);
            this.context.SaveChanges();

            //Act
            var date = DateTime.Today;
            var result = foodSupplementsService.GetOrderFromGivenDateAsync(date, "OtherId").Result;

            //Assert

            result.ShouldBeNull();
        }
    }
}

