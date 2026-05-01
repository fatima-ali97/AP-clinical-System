```
Views/
├── Shared/
│   ├── _AppLayout.cshtml       ← authenticated shell (sidebar + topbar)
│   ├── _Layout.cshtml          ← public shell (bare, full-screen)
│   ├── _Navigation.cshtml      ← role-based nav partial (used by _AppLayout)
│   ├── _LoginPartial.cshtml
│   └── _ValidationScriptsPartial.cshtml
│
├── Home/                       ← uses _Layout (marketing/landing pages)
│   └── Index.cshtml
│
├── Account/                    ← uses _Layout (login, register)
│   ├── Login.cshtml
│   └── Register.cshtml
│
├── Dashboard/                  ← uses _AppLayout
│   └── Index.cshtml
│
├── Manager/                    ← uses _AppLayout
├── Doctor/                     ← uses _AppLayout
├── Patient/                    ← uses _AppLayout
└── Receptionist/               ← uses _AppLayout
```




_naviagtion.cshtml:


@using AP_clinical_system.Services
@using AP_clinical_system.Models.Navigation
@* Views/Shared/_Navigation.cshtml *@
@using Microsoft.AspNetCore.Identity

@{
    // Determine active role
    var role = User.IsInRole("Manager") ? "Manager"
             : User.IsInRole("Doctor") ? "Doctor"
             : User.IsInRole("Receptionist") ? "Receptionist"
             : User.IsInRole("Patient") ? "Patient"
             : null;

    var activePage = ViewData["ActivePage"]?.ToString();
}

@* ── MANAGER NAV ── *@
@if (role == "Manager")
{
    <ul class="sidebar-nav">
        <li>
            <div class="sidebar-section">Main Menu</div>
        </li>

        <li class="sidebar-item @(activePage == "Dashboard" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Manager" asp-action="Index">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <rect x="3" y="3" width="7" height="7" />
                    <rect x="14" y="3" width="7" height="7" />
                    <rect x="3" y="14" width="7" height="7" />
                    <rect x="14" y="14" width="7" height="7" />
                </svg>
                Dashboard
            </a>
        </li>

        <li>
            <div class="sidebar-section">Manage</div>
        </li>

        <li class="sidebar-item @(activePage == "Accounts" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Manager" asp-action="ManageAccounts">
                <i class="fa-solid fa-user-gear"></i> Accounts
            </a>
        </li>

        <li class="sidebar-item @(activePage == "Doctors" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Manager" asp-action="Doctors">
                <i class="fa-solid fa-stethoscope"></i> Doctors
            </a>
        </li>

        <li class="sidebar-item @(activePage == "Availability" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Manager" asp-action="Availability">
                <i class="fa-solid fa-user-clock"></i> Availability Hours
            </a>
        </li>

        <li>
            <div class="sidebar-section">Analytics</div>
        </li>

        <li class="sidebar-item @(activePage == "Reports" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Manager" asp-action="Reports">
                <i class="fa-solid fa-chart-line"></i> Generate Reports
            </a>
        </li>
    </ul>
}

@* ── DOCTOR NAV ── *@
@if (role == "Doctor")
{
    <ul class="sidebar-nav">
        <li>
            <div class="sidebar-section">Main Menu</div>
        </li>

        <li class="sidebar-item @(activePage == "Dashboard" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Doctor" asp-action="Index">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <rect x="3" y="3" width="7" height="7" />
                    <rect x="14" y="3" width="7" height="7" />
                    <rect x="3" y="14" width="7" height="7" />
                    <rect x="14" y="14" width="7" height="7" />
                </svg>
                Dashboard
            </a>
        </li>

        <li>
            <div class="sidebar-section">My Work</div>
        </li>

        <li class="sidebar-item @(activePage == "Schedule" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Doctor" asp-action="Schedule">
                <i class="fa-solid fa-calendar-check"></i> My Schedule
            </a>
        </li>

        <li class="sidebar-item @(activePage == "Appointments" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Doctor" asp-action="Appointments">
                <i class="fa-solid fa-calendar-days"></i> Appointments
            </a>
        </li>

        <li class="sidebar-item @(activePage == "Patients" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Doctor" asp-action="Patients">
                <i class="fa-solid fa-person-dots-from-line"></i> My Patients
            </a>
        </li>

        <li>
            <div class="sidebar-section">Records</div>
        </li>

        <li class="sidebar-item @(activePage == "Prescriptions" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Doctor" asp-action="Prescriptions">
                <i class="fa-solid fa-capsules"></i> Prescriptions
            </a>
        </li>
    </ul>
}

@* ── RECEPTIONIST NAV ── *@
@if (role == "Receptionist")
{
    <ul class="sidebar-nav">
        <li>
            <div class="sidebar-section">Main Menu</div>
        </li>

        <li class="sidebar-item @(activePage == "Dashboard" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Receptionist" asp-action="Index">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <rect x="3" y="3" width="7" height="7" />
                    <rect x="14" y="3" width="7" height="7" />
                    <rect x="3" y="14" width="7" height="7" />
                    <rect x="14" y="14" width="7" height="7" />
                </svg>
                Dashboard
            </a>
        </li>

        <li>
            <div class="sidebar-section">Appointments</div>
        </li>

        <li class="sidebar-item @(activePage == "BookAppointment" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Receptionist" asp-action="BookAppointment">
                <i class="fa-solid fa-calendar-plus"></i> Book Appointment
            </a>
        </li>

        <li class="sidebar-item @(activePage == "Queue" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Receptionist" asp-action="Queue">
                <i class="fa-solid fa-display"></i> Live Queue
            </a>
        </li>

        <li>
            <div class="sidebar-section">Patients</div>
        </li>

        <li class="sidebar-item @(activePage == "Patients" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Receptionist" asp-action="Patients">
                <i class="fa-solid fa-users"></i> Patients
            </a>
        </li>
    </ul>
}

@* ── PATIENT NAV ── *@
@if (role == "Patient")
{
    <ul class="sidebar-nav">
        <li>
            <div class="sidebar-section">Main Menu</div>
        </li>

        <li class="sidebar-item @(activePage == "Dashboard" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Patient" asp-action="Index">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <rect x="3" y="3" width="7" height="7" />
                    <rect x="14" y="3" width="7" height="7" />
                    <rect x="3" y="14" width="7" height="7" />
                    <rect x="14" y="14" width="7" height="7" />
                </svg>
                Dashboard
            </a>
        </li>

        <li>
            <div class="sidebar-section">My Health</div>
        </li>

        <li class="sidebar-item @(activePage == "Appointments" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Patient" asp-action="Appointments">
                <i class="fa-solid fa-calendar"></i> My Appointments
            </a>
        </li>

        <li class="sidebar-item @(activePage == "Records" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Patient" asp-action="Records">
                <i class="fa-solid fa-file-medical"></i> My Records
            </a>
        </li>

        <li class="sidebar-item @(activePage == "Prescriptions" ? "active" : "")">
            <a class="sidebar-link" asp-controller="Patient" asp-action="Prescriptions">
                <i class="fa-solid fa-capsules"></i> Prescriptions
            </a>
        </li>
    </ul>
}