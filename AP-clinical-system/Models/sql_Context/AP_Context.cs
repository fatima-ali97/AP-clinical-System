using Microsoft.EntityFrameworkCore;
using AP_clinical_system.Models.Entities;

namespace AP_clinical_system.Models.sql_Context
{
    public class AP_Context : DbContext
    {
        public AP_Context(DbContextOptions<AP_Context> options) : base(options) { }
        public DbSet<appointment> appointments { get; set; }
        public DbSet<doctor_information> doctor_informations { get; set; }
        public DbSet<doctor_information_specialization_mtm> doctor_information_specialization_mtms { get; set; }
        public DbSet<doctor_specialization> doctor_specializations { get; set; }
        public DbSet<prescription> prescriptions { get; set; }
        public DbSet<system_user> system_users { get; set; }
        public DbSet<visit_record> visit_records { get; set; }

        
    }
}
