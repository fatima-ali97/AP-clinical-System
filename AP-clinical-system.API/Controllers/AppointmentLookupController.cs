using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using static AP_clinical_system.Models.GeneralHelper;

namespace AP_clinical_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentLookupController : ControllerBase
    {
        private readonly AP_Context context;

        public AppointmentLookupController(AP_Context _context)
        {
            context = _context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AnonAppLookup([FromBody] JsonElement body)
        {
            try
            {
                var cpr = body.TryGetProperty("cpr", out var cprElement) ? cprElement.GetString() : null;
                var record_no = body.TryGetProperty("record_no", out var recordNoElement) ? recordNoElement.GetString() : null;

                if (cpr == null && record_no == null)
                {
                    return BadRequest(new { error = "At least one of the fields must be provided." });
                }

                // initialize the needed vars
                var patientUserRecord = new system_user();
                var patientInformationRecord = new patient_information();
                var patient = new PatientObject();

                // if user provided cpr, find the patient system user record using cpr and then use it to find the patient information record
                if (cpr != null)
                {
                    patientUserRecord = context.system_users.FirstOrDefault(u => u.cpr == cpr);
                    patientInformationRecord = context.patient_informations.FirstOrDefault(p => p.system_user_ref == patientUserRecord.Id);
                }

                // if user provided his user_no, find the patient information record using the record_no and then use it to find the patient system user record
                if (record_no != null) 
                { 
                    patientInformationRecord = context.patient_informations.FirstOrDefault(p => p.record_no == record_no);
                    patientUserRecord = context.system_users.FirstOrDefault(u => u.Id == patientInformationRecord.system_user_ref);
                }

                // fill the patient object for returning it
                if (patientUserRecord != null && patientInformationRecord != null)
                {
                    patient.id = patientUserRecord.Id;
                    patient.CPR = patientUserRecord.cpr;
                    patient.FirstName = patientUserRecord.first_name;
                    patient.LastName = patientUserRecord.last_name;
                    patient.Email = patientUserRecord.Email;
                    patient.Phone = patientUserRecord.PhoneNumber;
                    patient.Success = true;
                } 
                else
                {
                    return NotFound(new { error = "No patient found with the provided information." });
                }

                // get all appointments 
                var allAppointments = context.appointments
                    .Where(a => a.patient_ref == patientInformationRecord.id)
                    .Select(a => new
                    {
                        a.id,
                        a.appointment_no,
                        a.appointment_reason,
                        a.appointment_status,
                        a.appointment_time_slot,
                        a.date
                    })
                    .ToList();

                // filter appointments into upcoming and past based on their status
                var upcomingAppointments = allAppointments.Where(a => a.appointment_status == (int)appointment_status.confirmed).ToList();
                var pastAppointments = allAppointments.Where(a => a.appointment_status == (int)appointment_status.completed).ToList();

                var response = new
                {
                    patient,
                    upcomingAppointments,
                    pastAppointments
                };

                return Ok(response);

            } catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }
    }
}
