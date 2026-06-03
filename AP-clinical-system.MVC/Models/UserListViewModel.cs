using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using AP_clinical_system.Models.Enums;

namespace AP_clinical_system.Models
{
    public class UserListViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string CPR { get; set; } = "";
        public string UserNo { get; set; } = "";
        public int UserRole { get; set; }
        public DateTime CreatedOn { get; set; }

        public string RoleLabel => ((user_role)UserRole).ToString().Replace("_", " ");

        public string RoleBadgeClass => UserRole switch
        {
            (int)user_role.system_admin => "bg-danger",
            (int)user_role.clinic_manager => "bg-indigo",
            (int)user_role.doctor => "bg-teal",
            (int)user_role.receptionist => "bg-blue",
            (int)user_role.patient => "bg-green",
            _ => "bg-secondary"
        };

    }

}