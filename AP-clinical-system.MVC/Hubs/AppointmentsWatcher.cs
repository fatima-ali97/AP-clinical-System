using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.SignalR;

namespace AP_clinical_system.Hubs
{
    public class AppointmentsWatcher : Hub
    {
        private readonly AP_Context _context;

        public AppointmentsWatcher(AP_Context context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
            {
                var counts = CalculateCounts();
                await Clients.Caller.SendAsync("Updatestatuses", counts);
                await base.OnConnectedAsync();
            }

            private object CalculateCounts()
            {
                return new
                {
                    requested = _context.appointments.Count(a => a.appointment_status == (int)appointment_status.requested),
                    confirmed = _context.appointments.Count(a => a.appointment_status == (int)appointment_status.confirmed),
                    checked_in = _context.appointments.Count(a => a.appointment_status == (int)appointment_status.checked_in),
                    in_progress = _context.appointments.Count(a => a.appointment_status == (int)appointment_status.in_progress),
                    completed = _context.appointments.Count(a => a.appointment_status == (int)appointment_status.completed),
                    cancelled = _context.appointments.Count(a => a.appointment_status == (int)appointment_status.cancelled),
                    no_show = _context.appointments.Count(a => a.appointment_status == (int)appointment_status.no_show),
                };
            }
    }
}
