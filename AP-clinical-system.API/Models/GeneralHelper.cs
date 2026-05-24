using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace AP_clinical_system.Models
{
    public static class GeneralHelper
    {

        public static DoctorObject GetDoctorObjectByID(AP_Context context, Guid doctorId)
        {
            var doctorUser = context.system_users.Where(s => s.Id == doctorId).FirstOrDefault();

            if (doctorUser == null)
            {
                return new DoctorObject { Success = false, Message = "User not found" };
            }

            var doctorInformation = context.doctor_informations.Where(d => d.system_user_ref == doctorUser.Id).FirstOrDefault();

            if (doctorInformation == null)
            {
                return new DoctorObject { Success = false, Message = "User is not a doctor" };
            }
           
            var specializationIDs = context.doctor_information_specialization_mtms
                .Where(d => d.doctor_information_ref == doctorInformation.id)
                .Select(d => d.doctor_specialization_ref)
                .ToList();

            if (specializationIDs == null || specializationIDs.Count == 0)
            {
                return new DoctorObject { Success = false, Message = "Doctor has no specializations" };
            }

            var specializations = context.doctor_specializations
                .Where(s => specializationIDs.Contains(s.id))
                .Select(s => new
                {
                  s.id,
                  s.specialization_name
                })
                .ToList<dynamic>();

            DoctorObject doctor = new DoctorObject()
            {
                id = doctorUser.Id,
                CPR = doctorUser.cpr,
                FirstName = doctorUser.first_name,
                LastName = doctorUser.last_name,
                Email = doctorUser.Email,
                Phone = doctorUser.PhoneNumber,
                JobTitle = doctorInformation.job_title,
                Specializations = specializations,
                Success = true,
                Message = "Doctor information retrieved successfully"
            };

            return doctor;
        }

        // FIX THIS LATER
        

        public static PatientObject GetPatientObjectByID(AP_Context context, Guid patientId)
        {
            var patientUser = context.system_users.Where(s => s.Id == patientId).FirstOrDefault();

            if (patientUser == null)
            {
                return new PatientObject { Success = false, Message = "User not found" };
            }

            var patientInformation = context.patient_informations.Where(d => d.system_user_ref == patientUser.Id).FirstOrDefault();

            if (patientInformation == null)
            {
                return new PatientObject { Success = false, Message = "User is not a patient" };
            }

            PatientObject patient = new PatientObject()
            {
                id = patientUser.Id,
                CPR = patientUser.cpr,
                FirstName = patientUser.first_name,
                LastName = patientUser.last_name,
                Email = patientUser.Email,
                Phone = patientUser.PhoneNumber,
                Success = true,
                Message = "Patient information retrieved successfully"
            };

            return patient;
        }

        public static ReceptionistObject GetReceptionistObjectByID(AP_Context context, Guid receptionistId)
        {
            var receptionistUser = context.system_users.Where(s => s.Id == receptionistId).FirstOrDefault();

            if (receptionistUser == null)
            {
                return new ReceptionistObject { Success = false, Message = "User not found" };
            }

            var receptionistInformation = context.receptionist_informations.Where(d => d.system_user_ref == receptionistUser.Id).FirstOrDefault();

            if (receptionistInformation == null)
            {
                return new ReceptionistObject { Success = false, Message = "User is not a receptionist" };
            }

            ReceptionistObject receptionist = new ReceptionistObject()
            {
                id = receptionistUser.Id,
                CPR = receptionistUser.cpr,
                FirstName = receptionistUser.first_name,
                LastName = receptionistUser.last_name,
                Email = receptionistUser.Email,
                Phone = receptionistUser.PhoneNumber,
                JobTitle = receptionistInformation.job_title,
                Success = true,
                Message = "Receptionist information retrieved successfully"
            };

            return receptionist;
        }

        public static ClinicManagerObject GetClinicManagerObjectByID(AP_Context context, Guid managerId)
        {
            var managerUser = context.system_users.Where(s => s.Id == managerId).FirstOrDefault();

            if (managerUser == null)
            {
                return new ClinicManagerObject { Success = false, Message = "User not found" };
            }

            var managerInformation = context.clinic_manager_informations.Where(d => d.system_user_ref == managerUser.Id).FirstOrDefault();

            if (managerInformation == null)
            {
                return new ClinicManagerObject { Success = false, Message = "User is not a clinic manager" };
            }

            ClinicManagerObject manager = new ClinicManagerObject()
            {
                id = managerUser.Id,
                CPR = managerUser.cpr,
                FirstName = managerUser.first_name,
                LastName = managerUser.last_name,
                Email = managerUser.Email,
                Phone = managerUser.PhoneNumber,
                JobTitle = managerInformation.job_title,
                Success = true,
                Message = "Clinic manager information retrieved successfully"
            };

            return manager;
        }

        public static Guid GetUserIdByJWT(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        public static string GetFullNameFromJWT(ClaimsPrincipal user)
        {
            var firstName = user.FindFirst("FirstName")?.Value ?? string.Empty;
            var lastName = user.FindFirst("LastName")?.Value ?? string.Empty;
            return $"{firstName} {lastName}".Trim();
        }

        public static string GetUserFullNameByID(AP_Context context, Guid userId)
        {
            var user = context.system_users.FirstOrDefault(s => s.Id == userId);
            if (user != null)
            {
                return $"{user.first_name} {user.last_name}".Trim();
            }
            return "N/A";
        }

        public static async Task<string> GetAutonumber(AP_Context context, string entityName)
        {
            var autonumberList = await context.autonumbers
                .Where(a => a.entity_name == entityName)
                .ToListAsync();

            if (autonumberList.Any())
            {
                var autonumberRecord = autonumberList.First();
                var fieldName = autonumberRecord.field_name;
                var pattern = autonumberRecord.pattern;
                var lastNumber = autonumberRecord.last_number;

                var autogeneratedString = string.Format(pattern.Replace("{0:D5}", "{0:D5}"), lastNumber);
                autonumberRecord.last_number = lastNumber + 1;

                return autogeneratedString;
            }
            else
            {
                return "ERR:0000X";
            }
        }

        public static bool VerifyUserType(AP_Context context, Guid Id, int userRole)
        {
           var user = context.system_users.First(u=>u.Id == Id && u.user_role == userRole && u.inactive != true);

            if (user == null)
            {
                return false;
            }

            else
            {
                return true;
            }
        }

        public class DoctorObject
        {
            public Guid id { get; set; }
            public string CPR { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string JobTitle { get; set; }
            public List<dynamic> Specializations { get; set; }

            // For error handling
            public bool Success { get; set; } = true;
            public string? Message { get; set; }

        }

        public class PatientObject
        {
            public Guid id { get; set; }
            public string CPR { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

            // For error handling
            public bool Success { get; set; } = true;
            public string? Message { get; set; }
        }

        public class ReceptionistObject
        {
            public Guid id { get; set; }
            public string CPR { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string JobTitle { get; set; }

            // For error handling
            public bool Success { get; set; } = true;
            public string? Message { get; set; }
        }

        public class ClinicManagerObject
        {
            public Guid id { get; set; }
            public string CPR { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string JobTitle { get; set; }

            // For error handling
            public bool Success { get; set; } = true;
            public string? Message { get; set; }
        }

    }
}
