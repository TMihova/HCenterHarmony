using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace HCH.Services.Tests
{
    public class AppointmentsServiceTests
    {
        private readonly ServiceCollection service;
        private readonly IServiceProvider serviceProvider;
        private readonly HCHWebContext context;

        public AppointmentsServiceTests()
        {
            this.service = new ServiceCollection();

            service.AddDbContext<HCHWebContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            service.AddScoped<IAppointmentsService, AppointmentsService>();

            this.serviceProvider = service.BuildServiceProvider();

            this.context = serviceProvider.GetService<HCHWebContext>();
        }


        [Fact]
        public void AppointmentsForTherapist_ShuoldReturnAllExistingDataForTherapist()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };
            this.context.Appointments.Add(newAppointment);
            this.context.SaveChanges();

            var otherAppointment = new Appointment
            {
                Id = "otherAppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Понеделник,
                VisitingHour = "10:30",
                TherapistId = "OtherTherapistId"
            };
            this.context.Appointments.Add(otherAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();
            var resultCollection = appointmentsService.AppointmentsForTherapistAsync("TherapistId").Result.ToList();
            var resultCount = resultCollection.Count;

            //Assert
            resultCount.ShouldBe(1);

            var appointmentModel = resultCollection.FirstOrDefault();

            appointmentModel.ShouldBeSameAs(newAppointment);
        }

        [Fact]
        public void AddAppointment_ShuoldReturnTrueForContainingAppointment()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            var countBefore = this.context.Appointments.Count();

            appointmentsService.AddAppointmentAsync(newAppointment).Wait();

            var countAfter = this.context.Appointments.Count();

            var addedNumber = countAfter - countBefore;

            bool appointmentsExists = this.context.Appointments.Contains(newAppointment);

            //Assert
            addedNumber.ShouldBe(1);

            appointmentsExists.ShouldBe(true);
        }

        [Fact]
        public void ExistsAppointmentById_ShuoldReturnTrueForContainingAppointment()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(newAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            bool appointmentsExists = appointmentsService.ExistsAppointmentById("appointmentId");

            //Assert

            appointmentsExists.ShouldBe(true);
        }

        [Fact]
        public void ExistsAppointmentById_ShuoldReturnFalseForNotContainingAppointment()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(newAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            bool appointmentsExists = appointmentsService.ExistsAppointmentById("otherId");

            //Assert

            appointmentsExists.ShouldBe(false);
        }

        [Fact]
        public void GetAppointmentById_ShuoldReturnContainedAppointment()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(newAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            Appointment appointmentDb = appointmentsService.GetAppointmentByIdAsync("appointmentId").Result;

            //Assert

            appointmentDb.ShouldBeSameAs(newAppointment);
        }

        [Fact]
        public void GetAppointmentById_ShuoldReturnNullForNotContainedAppointment()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(newAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            Appointment appointmentDb = appointmentsService.GetAppointmentByIdAsync("otherId").Result;

            //Assert

            appointmentDb.ShouldBeNull();
        }

        //IsThereSuchAppointment
        [Fact]
        public void IsThereSuchAppointment_ShuoldReturnTrueContainedAppointment()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(newAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            bool testResult = appointmentsService.IsThereSuchAppointment("TherapistId", Models.Enums.DayOfWeekBg.Вторник, "10:30");

            //Assert

            testResult.ShouldBe(true);
        }

        [Fact]
        public void IsThereSuchAppointment_ShuoldReturnFalseContainedAppointment()
        {
            //Arrange

            //seed data
            var newAppointment = new Appointment
            {
                Id = "appointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(newAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            bool testResult = appointmentsService.IsThereSuchAppointment("OtherId", Models.Enums.DayOfWeekBg.Вторник, "10:30");

            //Assert

            testResult.ShouldBe(false);
        }

        //OccupiedAppointmentsForPatientAsync
        [Fact]
        public void OccupiedAppointmentsForPatient_ShuoldReturnOccupiedAppointmentsForPatient()
        {
            //Arrange

            //seed data
            var firstAppointment = new Appointment
            {
                Id = "firstAppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId",
                PatientId = "PatientId"
            };

            this.context.Appointments.Add(firstAppointment);
            this.context.SaveChanges();

            var secondAppointment = new Appointment
            {
                Id = "seccondAppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Сряда,
                VisitingHour = "10:30",
                TherapistId = "OtherTherapistId",
                PatientId = "PatientId"
            };

            this.context.Appointments.Add(secondAppointment);
            this.context.SaveChanges();

            var thirdAppointment = new Appointment
            {
                Id = "thirdAppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Четвъртък,
                VisitingHour = "11:30",
                TherapistId = "TherapistId",
                PatientId = "OtherPatientId"
            };

            this.context.Appointments.Add(thirdAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            var resultCollection = appointmentsService.OccupiedAppointmentsForPatientAsync("PatientId").Result;

            //Assert
            var numberOfElements = resultCollection.Count();

            numberOfElements.ShouldBe(2);

            resultCollection.Contains(firstAppointment).ShouldBe(true);
            resultCollection.Contains(secondAppointment).ShouldBe(true);
            resultCollection.Contains(thirdAppointment).ShouldBe(false);
        }

        //OccupiedAppointmentsForTherapistAsync
        [Fact]
        public void OccupiedAppointmentsForTherapist_ShuoldReturnOccupiedAppointmentsForTherapist()
        {
            //Arrange

            //seed data
            var firstAppointment = new Appointment
            {
                Id = "firstAppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId",
                PatientId = "PatientId"
            };

            this.context.Appointments.Add(firstAppointment);
            this.context.SaveChanges();

            var secondAppointment = new Appointment
            {
                Id = "seccondAppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Сряда,
                VisitingHour = "10:30",
                TherapistId = "OtherTherapistId",
                PatientId = "PatientId"
            };

            this.context.Appointments.Add(secondAppointment);
            this.context.SaveChanges();

            var thirdAppointment = new Appointment
            {
                Id = "thirdAppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Четвъртък,
                VisitingHour = "11:30",
                TherapistId = "TherapistId",
                PatientId = "OtherPatientId"
            };

            this.context.Appointments.Add(thirdAppointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            var resultCollection = appointmentsService.OccupiedAppointmentsForTherapistAsync("TherapistId").Result;

            //Assert
            var numberOfElements = resultCollection.Count();

            numberOfElements.ShouldBe(2);

            resultCollection.Contains(firstAppointment).ShouldBe(true);
            resultCollection.Contains(secondAppointment).ShouldBe(false);
            resultCollection.Contains(thirdAppointment).ShouldBe(true);
        }

        //TakeAppointmentForPatientAsync
        [Fact]
        public void TakeAppointment_ShuoldReturnPatientIdForAppointmentPatientId()
        {
            //Arrange

            //seed data
            var appointment = new Appointment
            {
                Id = "AppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(appointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            appointmentsService.TakeAppointmentForPatientAsync("AppointmentId", "PatientId").Wait();

            //Assert
            appointment.PatientId.ShouldBe("PatientId");
        }

        //ReleaseAppointmentAsync
        [Fact]
        public void ReleaseAppointment_ShuoldReturnNullForAppointmentPatientId()
        {
            //Arrange

            //seed data
            var appointment = new Appointment
            {
                Id = "AppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId",
                PatientId = "PatientId"
            };

            this.context.Appointments.Add(appointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            appointmentsService.ReleaseAppointmentAsync("AppointmentId").Wait();

            //Assert
            appointment.PatientId.ShouldBeNull();
        }

        //RemoveAppointmentAsync
        [Fact]
        public void RemoveAppointment_ShuoldReturnFalseForContainingAppointment()
        {
            //Arrange

            //seed data
            var appointment = new Appointment
            {
                Id = "AppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(appointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            appointmentsService.RemoveAppointmentAsync(appointment).Wait();

            //Assert
            bool appointmentExists = this.context.Appointments.Contains(appointment);

            appointmentExists.ShouldBe(false);
        }

        //UpdateAppointmentAsync
        [Fact]
        public void UpdateAppointment_ShuoldReturnUpdatedAppointmentDb()
        {
            //Arrange

            //seed data
            var appointment = new Appointment
            {
                Id = "AppointmentId",
                DayOfWeekBg = Models.Enums.DayOfWeekBg.Вторник,
                VisitingHour = "10:30",
                TherapistId = "TherapistId"
            };

            this.context.Appointments.Add(appointment);
            this.context.SaveChanges();

            //Act
            var appointmentsService = this.serviceProvider.GetService<IAppointmentsService>();

            
            appointmentsService.UpdateAppointmentAsync("AppointmentId", Models.Enums.DayOfWeekBg.Понеделник, "11:30").Wait();

            //Assert
            var appointmentDb = this.context.Appointments.Find("AppointmentId");

            appointmentDb.DayOfWeekBg.ShouldBe(Models.Enums.DayOfWeekBg.Понеделник);

            appointmentDb.VisitingHour.ShouldBe("11:30");
        }
    }
}

