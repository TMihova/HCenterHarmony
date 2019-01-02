
using System;
using HCH.Models;
using HCH.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HCH.Data
{
    public class HCHWebContext : IdentityDbContext<HCHWebUser>
    {
        public HCHWebContext(DbContextOptions<HCHWebContext> options)
            : base(options)
        {
        }

        public DbSet<FoodSupplement> FoodSupplements { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderFoodSupplement> OrderFoodSupplements { get; set; }

        public DbSet<Treatment> Treatments { get; set; }

        public DbSet<Therapy> Therapies { get; set; }

        public DbSet<TherapyTreatment> TherapyTreatments { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<DeliveryNote> DeliveryNotes { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Examination> Examinations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<OrderFoodSupplement>()
                .HasOne(x => x.Order)
                .WithMany(x => x.FoodSupplements)
                .HasForeignKey(x => x.OrderId);

            builder.Entity<TherapyTreatment>()
                .HasOne(x => x.Treatment)
                .WithMany(x => x.Therapies)
                .HasForeignKey(x => x.TreatmentId);

            builder.Entity<TherapyTreatment>()
                .HasOne(x => x.Therapy)
                .WithMany(x => x.Treatments)
                .HasForeignKey(x => x.TherapyId);

            builder.Entity<TherapyTreatment>()
                .HasKey(x => new { x.TreatmentId, x.TherapyId });

            builder.Entity<Appointment>()
                .HasOne(x => x.Therapist)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.TherapistId);

            builder.Entity<Examination>()
                .HasOne(x => x.Therapist)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Examination>()
                .HasOne(x => x.Patient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
