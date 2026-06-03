namespace AP_clinical_system.Services
{
    using AP_clinical_system.Models.Navigation;
    public static class NavigationService
    {
        public static List<NavItem> GetNavForRole(string role) => role switch
        {
            "Manager" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Manager",      Action = "Index",         Icon = "fa-solid fa-table-cells-large" },
            new() { Label = "Doctors Availability",      Controller = "Manager",      Action = "Doctors",       Icon = "fa-solid fa-user-doctor" },
            new() { Label = "Reports",      Controller = "Manager",      Action = "Reports",       Icon = "fa-solid fa-chart-line" },
            new() { Label = "Users",        Controller = "Manager",      Action = "Users",         Icon = "fa-solid fa-user-gear" },
        },

            "Doctor" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Doctor",       Action = "Index",         Icon = "fa-solid fa-table-cells-large" },
            new() { Label = "My Schedule",  Controller = "Doctor",       Action = "Schedule",      Icon = "fa-solid fa-calendar"},
            new() { Label = "My Appointments",  Controller = "Doctor",       Action = "Schedule",      Icon = "fa-solid fa-calendar"},
            new() { Label = "My Patients",  Controller = "Doctor",       Action = "Patients",      Icon = "fa-solid fa-bed-pulse" },
            new() { Label = "Prescriptions",Controller = "Doctor",       Action = "Prescriptions", Icon = "fa-solid fa-capsules" },
            new() { Label = "Notifications",Controller = "Doctor",       Action = "Notifications", Icon = "fa-solid fa-bell" },

        },

            "Receptionist" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Receptionist", Action = "Index",         Icon = "fa-solid fa-table-cells-large" },
            new() { Label = "Book Appointment", Controller = "Receptionist", Action = "Book",      Icon = "fa-solid fa-plus" },
            new() { Label = "Calendar", Controller = "Receptionist", Action = "Calendar",      Icon = "fa-solid fa-calendar-days" },
            new() { Label = "Notifications",     Controller = "Receptionist", Action = "Notifications",      Icon = "fa-solid fa-bell"  },
        },

            "Patient" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Patient",      Action = "Index",         Icon = "fa-solid fa-table-cells-large" },
            new() { Label = "My Appointments", Controller = "Patient",   Action = "Appointments",  Icon = "fa-solid fa-calendar" },
            new() { Label = "Prescriptions",Controller = "Patient",      Action = "Prescriptions", Icon = "fa-solid fa-capsules" },
            new() { Label = "Notifications",Controller = "Patient",      Action = "Notifications", Icon = "fa-solid fa-bell" },
            new() { Label = "History",   Controller = "Patient",      Action = "History",       Icon = "fa-solid fa-clock-rotate-left" },
        },

            _ => new List<NavItem>()
        };
    }
}