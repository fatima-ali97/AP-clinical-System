using Microsoft.EntityFrameworkCore;
using AP_clinical_system.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AP_clinical_system.Models.sql_Context
{
    public class AP_Context : IdentityDbContext<system_user, IdentityRole<Guid>, Guid>
    {
        public AP_Context(DbContextOptions<AP_Context> options) : base(options) { }
        public DbSet<appointment> appointments { get; set; }
        public DbSet<doctor_information> doctor_informations { get; set; }
        public DbSet<doctor_information_specialization_mtm> doctor_information_specialization_mtms { get; set; }
        public DbSet<doctor_specialization> doctor_specializations { get; set; }
        public DbSet<prescription> prescriptions { get; set; }
        public DbSet<system_user> system_users { get; set; }
        public DbSet<visit_record> visit_records { get; set; }
        public DbSet<doctor_leave> doctor_leaves { get; set; }
        public DbSet<doctor_schedule> doctor_schedules { get; set; }
        public DbSet<notification> notifications { get; set; }
        public DbSet<autonumber> autonumbers { get; set; }
        public DbSet<clinic_manager_information> clinic_manager_informations { get; set; }
        public DbSet<patient_information> patient_informations { get; set; }
        public DbSet<receptionist_information> receptionist_informations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Required — lets Identity configure its tables

            builder.Entity<system_user>(entity =>
            {
                entity.ToTable("system_users");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
                entity.Property(e => e.PasswordHash).HasColumnName("hashed_password");
            });

            builder.Entity<IdentityRole<Guid>>(entity => entity.ToTable("system_roles"));
            builder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("system_user_roles"));
            builder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("system_user_claims"));
            builder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("system_user_logins"));
            builder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("system_user_tokens"));
            builder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("system_role_claims"));
        }
    }
}
