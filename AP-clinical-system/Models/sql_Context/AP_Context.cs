using Microsoft.EntityFrameworkCore;

namespace AP_clinical_system.Models.sql_Context
{
    public class AP_Context : DbContext
    {
        public AP_Context(DbContextOptions<AP_Context> options) : base(options) { }
        
    }
}
