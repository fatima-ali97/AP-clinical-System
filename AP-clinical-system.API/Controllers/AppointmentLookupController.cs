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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentLookupController : ControllerBase
    {
        private readonly AP_Context context;

        public AppointmentLookupController(AP_Context _context)
        {
            context = _context;
        }

        // GET: api/AppointmentLookup/AnonAppLookup
        [HttpPost]
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

                // initialize the needed vars as null so the null-check below is reliable
                system_user? patientUserRecord = null;
                patient_information? patientInformationRecord = null;
                var patient = new PatientObject();

                // if user provided cpr, find the patient system user record using cpr and then use it to find the patient information record
                if (cpr != null)
                {
                    patientUserRecord = context.system_users.FirstOrDefault(u => u.cpr == cpr);
                    if (patientUserRecord != null)
                    {
                        patientInformationRecord = context.patient_informations.FirstOrDefault(p => p.system_user_ref == patientUserRecord.Id);
                    }
                }

                // if user provided his record_no, find the patient information record using the record_no and then use it to find the patient system user record
                else if (record_no != null)
                {
                    patientInformationRecord = context.patient_informations.FirstOrDefault(p => p.record_no == record_no);
                    if (patientInformationRecord != null && patientInformationRecord.system_user_ref.HasValue)
                    {
                        var refId = patientInformationRecord.system_user_ref.Value;
                        patientUserRecord = context.system_users.FirstOrDefault(u => u.Id == refId);
                    }
                }

                // fill the patient object for returning it
                if (patientUserRecord == null || patientInformationRecord == null)
                {
                    return StatusCode(404,new { error = "No patient found with the provided information.", message = "No patient found with the provided information." });
                }

                patient.id = patientUserRecord.Id;
                patient.CPR = patientUserRecord.cpr;
                patient.FirstName = patientUserRecord.first_name;
                patient.LastName = patientUserRecord.last_name;
                patient.Email = patientUserRecord.Email;
                patient.Phone = patientUserRecord.PhoneNumber;
                patient.Success = true;

                // get all appointments
                var patientInfoId = patientInformationRecord.id;
                var allAppointments = context.appointments
                    .Where(a => a.patient_ref == patientInfoId && a.inactive != true)
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
