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
    public class TherapiesServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public TherapiesServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<ITherapiesService, TherapiesService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }

        //GetTherapyByIdAsync
        [Fact]
        public void GetTherapyById_ShouldReturnTherapyForExistingId()
        {
            //Arrange
            var therapiesService = this.serviceProvider.GetService<ITherapiesService>();

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

            var therapy = new Therapy
            {
                Id = "TherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "PatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "FirstTreatmentId"},
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "SecondTreatmentId"}
                }
            };

            this.context.Therapies.Add(therapy);
            this.context.SaveChanges();

            var otherTherapy = new Therapy
            {
                Id = "OtherTherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "OtherPatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "OtherTherapyId", TreatmentId = "FirstTreatmentId"}
                }
            };
            this.context.Therapies.Add(otherTherapy);
            this.context.SaveChanges();

            //Act
            var result = therapiesService.GetTherapyByIdAsync("TherapyId").Result;

            //Assert
            result.ShouldBeSameAs(therapy);
        }

        [Fact]
        public void GetTherapyById_ShouldReturnNullForNonExistingId()
        {
            //Arrange
            var therapiesService = this.serviceProvider.GetService<ITherapiesService>();

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

            var therapy = new Therapy
            {
                Id = "TherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "PatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "FirstTreatmentId"},
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "SecondTreatmentId"}
                }
            };

            this.context.Therapies.Add(therapy);
            this.context.SaveChanges();

            var otherTherapy = new Therapy
            {
                Id = "OtherTherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "OtherPatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "OtherTherapyId", TreatmentId = "FirstTreatmentId"}
                }
            };
            this.context.Therapies.Add(otherTherapy);
            this.context.SaveChanges();

            //Act
            var result = therapiesService.GetTherapyByIdAsync("OtherId").Result;

            //Assert
            result.ShouldBeNull();
        }
        //AddTherapyAsync
        [Fact]
        public void AddTherapy_ShouldReturnTrueForExistCheckInDb()
        {
            //Arrange
            var therapiesService = this.serviceProvider.GetService<ITherapiesService>();

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

            var therapy = new Therapy
            {
                Id = "TherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "PatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "FirstTreatmentId"},
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "SecondTreatmentId"}
                }
            };

            //Act
            therapiesService.AddTherapyAsync(therapy).Wait();

            bool therapyExists = this.context.Therapies.Contains(therapy);

            //Assert
            therapyExists.ShouldBeTrue();
        }

        //UpdateTherapy
        [Fact]
        public void UpdateTherapy_ShouldReturnUpdatedTherapy()
        {
            //Arrange
            var therapiesService = this.serviceProvider.GetService<ITherapiesService>();

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

            var therapy = new Therapy
            {
                Id = "TherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "PatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "FirstTreatmentId"},
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "SecondTreatmentId"}
                }
            };
            this.context.Therapies.Add(therapy);
            this.context.SaveChanges();

            //Act
            therapy.EndDate = DateTime.Today.AddDays(5);
            therapy.Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "SecondTreatmentId"}
                };

            therapiesService.UpdateTherapy(therapy).Wait();

            var therapyDb = this.context.Therapies.Find("TherapyId");

            //Assert
            therapyDb.EndDate.ShouldBe(therapy.EndDate);
            therapyDb.Treatments.ShouldBeSameAs(therapy.Treatments);
        }

        //TherapyExists
        [Fact]
        public void TherapyExists_ShouldReturnTrueForExistingId()
        {
            //Arrange
            var therapiesService = this.serviceProvider.GetService<ITherapiesService>();

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

            var therapy = new Therapy
            {
                Id = "TherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "PatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "FirstTreatmentId"},
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "SecondTreatmentId"}
                }
            };
            this.context.Therapies.Add(therapy);
            this.context.SaveChanges();

            //Act
            bool exists = therapiesService.TherapyExists("TherapyId");

            //Assert
            exists.ShouldBeTrue();
        }

        [Fact]
        public void TherapyExists_ShouldReturnFalseForNonExistingId()
        {
            //Arrange
            var therapiesService = this.serviceProvider.GetService<ITherapiesService>();

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

            var therapy = new Therapy
            {
                Id = "TherapyId",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                PatientId = "PatientId",
                TherapistId = "TherapistId",
                Treatments = new List<TherapyTreatment>
                {
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "FirstTreatmentId"},
                    new TherapyTreatment
                    { TherapyId = "TherapyId", TreatmentId = "SecondTreatmentId"}
                }
            };
            this.context.Therapies.Add(therapy);
            this.context.SaveChanges();

            //Act
            bool exists = therapiesService.TherapyExists("OtherId");

            //Assert
            exists.ShouldBeFalse();
        }
    }
}

