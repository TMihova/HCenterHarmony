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
    public class TreatmentsServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public TreatmentsServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<ITreatmentsService, TreatmentsService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }

        //AddTreatmentAsync
        [Fact]
        public void AddTreatment_ShouldReturnTrueForExistCheckInDb()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "ProfileId"
            };

            //Act
            treatmentsService.AddTreatmentAsync(firstTreatment).Wait();

            var count = this.context.Treatments.Count();

            bool exists = this.context.Treatments.Contains(firstTreatment);

            //Assert
            count.ShouldBe(1);
            exists.ShouldBeTrue();
        }

        //AllAsync
        [Fact]
        public void All_ShouldReturnAllTreatmentsFromDb()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "ProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "ProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            var resultCollection = treatmentsService.AllAsync().Result;

            var countDb = this.context.Treatments.Count();

            var countResult = resultCollection.Count();

            bool resultContains = resultCollection.Contains(firstTreatment)
                && resultCollection.Contains(secondTreatment);

            //Assert
            countResult.ShouldBe(countDb);
            resultContains.ShouldBeTrue();
        }

        [Fact]
        public void All_ShouldReturnEmptyCollectionFromEmptyDb()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            

            //Act
            var resultCollection = treatmentsService.AllAsync().Result;

            var countResult = resultCollection.Count();

            //Assert
            countResult.ShouldBe(0);
        }

        //AllFromProfileAsync
        [Fact]
        public void AllFromProfile_ShouldReturnAllTreatmentsFromGivenProfile()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            var resultCollection = treatmentsService.AllFromProfileAsync("SecondProfileId").Result;

            var countDb = this.context.Treatments
                .Where(x => x.ProfileId == "SecondProfileId").Count();

            var countResult = resultCollection.Count();

            bool resultContains = resultCollection.Contains(secondTreatment);

            //Assert
            countResult.ShouldBe(countDb);
            resultContains.ShouldBeTrue();
        }

        [Fact]
        public void AllFromProfile_ShouldReturnEmptyCollrctionForNonExistingProfile()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            var resultCollection = treatmentsService.AllFromProfileAsync("NonExistingId").Result;

            var countResult = resultCollection.Count();

            //Assert
            countResult.ShouldBe(0);
        }

        //GetTreatmentById
        [Fact]
        public void GetTreatmentById_ShouldReturnTreatmentWithGivenId()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            var result = treatmentsService.GetTreatmentById("FirstTreatmentId").Result;

            //Assert
            result.ShouldBeSameAs(firstTreatment);
        }

        [Fact]
        public void GetTreatmentById_ShouldReturnNullForNonExistingId()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            var result = treatmentsService.GetTreatmentById("NonExistingId").Result;

            //Assert
            result.ShouldBeNull();
        }

        //RemoveTreatmentAsync
        [Fact]
        public void RemoveTreatment_ShouldReturnFalseForExistCheck()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            treatmentsService.RemoveTreatmentAsync(firstTreatment).Wait();

            bool treatmentExist = this.context.Treatments.Contains(firstTreatment);

            //Assert
            treatmentExist.ShouldBeFalse();
        }

        [Fact]
        public void RemoveTreatment_ShouldReturnFalseForExistCheckForNonExistingTreatment()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };

            //Act
            treatmentsService.RemoveTreatmentAsync(secondTreatment).Wait();

            bool treatmentExist = this.context.Treatments.Contains(secondTreatment);

            //Assert
            treatmentExist.ShouldBeFalse();
        }

        //TreatementExists
        [Fact]
        public void TreatementExists_ShouldReturnTrueForExistingTreatmentInDb()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            var result = treatmentsService.TreatementExists("FirstTreatmentId");

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TreatementExists_ShouldReturnFalseForNonExistingTreatmentInDb()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            var result = treatmentsService.TreatementExists("NonExistingId");

            //Assert
            result.ShouldBeFalse();
        }

        //UpdateTreatmentAsync
        [Fact]
        public void UpdateTreatment_ShouldReturnUpdatedTreatmentInDb()
        {
            //Arrange
            var treatmentsService = this.serviceProvider.GetService<ITreatmentsService>();

            //seed data
            var firstTreatment = new Treatment
            {
                Id = "FirstTreatmentId",
                Name = "FirstTreatment",
                Description = "FirstDescription",
                Price = 5,
                ProfileId = "FirstProfileId"
            };
            this.context.Treatments.Add(firstTreatment);
            this.context.SaveChanges();

            var secondTreatment = new Treatment
            {
                Id = "SecondTreatmentId",
                Name = "SecondTreatment",
                Description = "SecondDescription",
                Price = 5,
                ProfileId = "SecondProfileId"
            };
            this.context.Treatments.Add(secondTreatment);
            this.context.SaveChanges();

            //Act
            firstTreatment.Name = "NewName";
            firstTreatment.Price = 15;

            treatmentsService.UpdateTreatmentAsync(firstTreatment);

            var treatmentDb = this.context.Treatments.Find("FirstTreatmentId");

            //Assert
            treatmentDb.Name.ShouldBe("NewName");
            treatmentDb.Price.ShouldBe(15);
        }
    }
}

