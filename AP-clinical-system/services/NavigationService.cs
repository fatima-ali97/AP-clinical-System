namespace AP_clinical_system.Services
{
    using AP_clinical_system.Models.Navigation;
    public static class NavigationService
    {
        public static List<NavItem> GetNavForRole(string role) => role switch
        {
            "Manager" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Manager",      Action = "Index",         Icon = "bi-grid" },
            new() { Label = "Doctors",      Controller = "Manager",      Action = "Doctors",       Icon = "bi-person-badge" },
            new() { Label = "Appointments", Controller = "Manager",      Action = "Appointments",  Icon = "bi-calendar" },
            new() { Label = "Reports",      Controller = "Manager",      Action = "Reports",       Icon = "bi-bar-chart" },
            new() { Label = "Users",        Controller = "Manager",      Action = "Users",         Icon = "bi-people" },
        },

            "Doctor" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Doctor",       Action = "Index",         Icon = "bi-grid" },
            new() { Label = "My Schedule",  Controller = "Doctor",       Action = "Schedule",      Icon = "bi-calendar-check" },
            new() { Label = "My Patients",  Controller = "Doctor",       Action = "Patients",      Icon = "bi-person-heart" },
            new() { Label = "Prescriptions",Controller = "Doctor",       Action = "Prescriptions", Icon = "bi-capsule" },
        },

            "Receptionist" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Receptionist", Action = "Index",         Icon = "bi-grid" },
            new() { Label = "Book Appointment", Controller = "Receptionist", Action = "Book",      Icon = "bi-calendar-plus" },
            new() { Label = "Live Queue",   Controller = "Receptionist", Action = "Queue",         Icon = "bi-display" },
            new() { Label = "Notifications",     Controller = "Receptionist", Action = "Patients",      Icon = "bi-people" },
        },

            "Patient" => new List<NavItem>
        {
            new() { Label = "Dashboard",    Controller = "Patient",      Action = "Index",         Icon = "bi-grid" },
            new() { Label = "My Appointments", Controller = "Patient",   Action = "Appointments",  Icon = "bi-calendar" },
            new() { Label = "Prescriptions",Controller = "Patient",      Action = "Prescriptions", Icon = "bi-capsule" },
            new() { Label = "Notifications",Controller = "Patient",      Action = "Prescriptions", Icon = "bi-capsule" },
            new() { Label = "History",   Controller = "Patient",      Action = "Records",       Icon = "bi-file-medical" },
        },

            _ => new List<NavItem>()
        };
    }
}
