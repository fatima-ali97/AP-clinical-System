using Microsoft.EntityFrameworkCore;
using AP_clinical_system.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace AP_clinical_system.Models.sql_Context
{
    public class AP_Context : IdentityDbContext
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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        
    }
}
