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
    public class ExaminationsServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public ExaminationsServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<IExaminationsService, ExaminationsService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }

        //AddExaminationAsync
        [Fact]
        public void AddExamination_ShuoldReturnTrueForExistCheck()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var examination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "PatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };

            //Act 

            var countBefore = this.context.Examinations.Count();

            examinationsService.AddExaminationAsync(examination);

            var countAfter = this.context.Examinations.Count();

            var difference = countAfter - countBefore;

            bool examinationExists = this.context.Examinations.Any(x => x.Id == 1);

            //Assert
            difference.ShouldBe(1);

            examinationExists.ShouldBe(true);
        }

        //AllExaminationsForTherapist
        [Fact]
        public void AllExaminationsForTherapist_ShuoldReturnAllExaminationsForGivenTherapist()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            var secondExamination = new Examination
            {
                Id = 2,
                ExaminationDate = DateTime.Today,
                PatientId = "secondPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(secondExamination);
            this.context.SaveChanges();

            var thirdExamination = new Examination
            {
                Id = 3,
                ExaminationDate = DateTime.Today,
                PatientId = "thirdPatientId",
                TherapistId = "OtherTherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(thirdExamination);
            this.context.SaveChanges();

            var fourthExamination = new Examination
            {
                Id = 4,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(fourthExamination);
            this.context.SaveChanges();

            //Act 

            var resultCollection = examinationsService.AllExaminationsForTherapist("TherapistId").Result;

            bool anyWrongExaminations = resultCollection.Any(x => x.TherapistId != "TherapistId");

            //Assert
            resultCollection.Count().ShouldBe(3);

            anyWrongExaminations.ShouldBe(false);
        }

        [Fact]
        public void AllExaminationsForTherapist_ShuoldReturnEmptyCollectionForNonExistingId()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            var secondExamination = new Examination
            {
                Id = 2,
                ExaminationDate = DateTime.Today,
                PatientId = "secondPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(secondExamination);
            this.context.SaveChanges();

            var thirdExamination = new Examination
            {
                Id = 3,
                ExaminationDate = DateTime.Today,
                PatientId = "thirdPatientId",
                TherapistId = "OtherTherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(thirdExamination);
            this.context.SaveChanges();

            var fourthExamination = new Examination
            {
                Id = 4,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(fourthExamination);
            this.context.SaveChanges();

            //Act 

            var resultCollection = examinationsService.AllExaminationsForTherapist("NonExistingId").Result;

            //Assert
            resultCollection.Count().ShouldBe(0);
        }

        //GetExaminationByIdAsync
        [Fact]
        public void GetExaminationById_ShuoldReturnExistingExaminationWithGivenId()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            var secondExamination = new Examination
            {
                Id = 2,
                ExaminationDate = DateTime.Today,
                PatientId = "secondPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(secondExamination);
            this.context.SaveChanges();

            var thirdExamination = new Examination
            {
                Id = 3,
                ExaminationDate = DateTime.Today,
                PatientId = "thirdPatientId",
                TherapistId = "OtherTherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(thirdExamination);
            this.context.SaveChanges();

            var fourthExamination = new Examination
            {
                Id = 4,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(fourthExamination);
            this.context.SaveChanges();

            //Act 

            var examination = examinationsService.GetExaminationByIdAsync(1).Result;

            //Assert
            examination.ShouldBeSameAs(firstExamination);
        }

        [Fact]
        public void GetExaminationById_ShuoldReturnNullForNonExistingId()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            var secondExamination = new Examination
            {
                Id = 2,
                ExaminationDate = DateTime.Today,
                PatientId = "secondPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(secondExamination);
            this.context.SaveChanges();

            var thirdExamination = new Examination
            {
                Id = 3,
                ExaminationDate = DateTime.Today,
                PatientId = "thirdPatientId",
                TherapistId = "OtherTherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(thirdExamination);
            this.context.SaveChanges();

            var fourthExamination = new Examination
            {
                Id = 4,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(fourthExamination);
            this.context.SaveChanges();

            //Act 

            var examination = examinationsService.GetExaminationByIdAsync(10).Result;

            //Assert
            examination.ShouldBeNull();
        }

        //UpdateExaminationAsync
        [Fact]
        public void UpdateExaminationAsync_ShuoldReturnUpdatedExaminationDb()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            //Act 

            firstExamination.Anamnesis = "New Anamnesis";

            examinationsService.UpdateExaminationAsync(firstExamination);

            var examinationDb = examinationsService.GetExaminationByIdAsync(1).Result;

            //Assert
            examinationDb.ShouldBeSameAs(firstExamination);
        }

        //ExaminationExists
        [Fact]
        public void ExaminationExists_ShuoldReturnTrueForExistingExamination()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            //Act
            var exists = examinationsService.ExaminationExists(1);

            //Assert
            exists.ShouldBe(true);
        }

        [Fact]
        public void ExaminationExists_ShuoldReturnFalseForNonExistingExamination()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            //Act
            var exists = examinationsService.ExaminationExists(11);

            //Assert
            exists.ShouldBe(false);
        }

        //AllExaminationsForPatientAsync
        [Fact]
        public void AllExaminationsForPatient_ShuoldReturnAllExaminationsForGivenPatient()
        {
            //Arrange
            var examinationsService = this.serviceProvider.GetService<IExaminationsService>();

            //seed data
            var firstExamination = new Examination
            {
                Id = 1,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(firstExamination);
            this.context.SaveChanges();

            var secondExamination = new Examination
            {
                Id = 2,
                ExaminationDate = DateTime.Today,
                PatientId = "secondPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(secondExamination);
            this.context.SaveChanges();

            var thirdExamination = new Examination
            {
                Id = 3,
                ExaminationDate = DateTime.Today,
                PatientId = "thirdPatientId",
                TherapistId = "OtherTherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(thirdExamination);
            this.context.SaveChanges();

            var fourthExamination = new Examination
            {
                Id = 4,
                ExaminationDate = DateTime.Today,
                PatientId = "firstPatientId",
                TherapistId = "TherapistId",
                Price = 40,
                Anamnesis = "Anamnesis"
            };
            this.context.Examinations.Add(fourthExamination);
            this.context.SaveChanges();

            //Act
            var resultCollection = examinationsService.AllExaminationsForPatientAsync("firstPatientId").Result;

            //Assert
            var count = resultCollection.Count();
            bool wrongExaminations = resultCollection.Any(x => x.PatientId != "firstPatientId");

            count.ShouldBe(2);
            wrongExaminations.ShouldBe(false);
        }

        
    }
}

