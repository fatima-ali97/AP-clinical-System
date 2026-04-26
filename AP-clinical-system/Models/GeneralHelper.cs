using AP_clinical_system.Models.sql_Context;
using System.Net.NetworkInformation;

namespace AP_clinical_system.Models
{
    public static class GeneralHelper
    {

        public static DoctorObject GetDoctorObjectByID(AP_Context context, Guid doctorId)
        {
            var doctorUser = context.system_users.Where(s => s.id == doctorId).FirstOrDefault();

            if (doctorUser == null)
            {
                return new DoctorObject { Success = false, Message = "User not found" };
            }

            var doctorInformation = context.doctor_informations.Where(d => d.system_user_ref == doctorUser.id).FirstOrDefault();

            if (doctorInformation == null)
            {
                return new DoctorObject { Success = false, Message = "User is not a doctor" };
            }
           
            var specializationIDs = context.doctor_information_specialization_mtms.Where(d => d.doctor_information_ref == doctorInformation.id).Select(d => d.doctor_specialization_ref).ToList();

            if (specializationIDs == null || specializationIDs.Count == 0)
            {
                return new DoctorObject { Success = false, Message = "Doctor has no specializations" };
            }

            var specializationNames = context.doctor_specializations.Where(s => specializationIDs.Contains(s.id)).Select(s => s.specialization_name).ToList();

            DoctorObject doctor = new DoctorObject()
            {
                id = doctorUser.id,
                CPR = doctorUser.cpr,
                FirstName = doctorUser.first_name,
                LastName = doctorUser.last_name,
                Email = doctorUser.email,
                Phone = doctorUser.phone_number,
                JobTitle = doctorInformation.job_title,
                Specializations = specializationNames,
                Success = true,
                Message = "Doctor information retrieved successfully"
            };

            return doctor;
        }

        public static PatientObject GetPatientObjectByID(AP_Context context, Guid patientId)
        {
            var patientUser = context.system_users.Where(s => s.id == patientId).FirstOrDefault();

            if (patientUser == null)
            {
                return new PatientObject { Success = false, Message = "User not found" };
            }

            var patientInformation = context.patient_informations.Where(d => d.system_user_ref == patientUser.id).FirstOrDefault();

            if (patientInformation == null)
            {
                return new PatientObject { Success = false, Message = "User is not a patient" };
            }

            PatientObject patient = new PatientObject()
            {
                id = patientUser.id,
                CPR = patientUser.cpr,
                FirstName = patientUser.first_name,
                LastName = patientUser.last_name,
                Email = patientUser.email,
                Phone = patientUser.phone_number,
                Success = true,
                Message = "Patient information retrieved successfully"
            };

            return patient;
        }

        public static ReceptionistObject GetReceptionistObjectByID(AP_Context context, Guid receptionistId)
        {
            var receptionistUser = context.system_users.Where(s => s.id == receptionistId).FirstOrDefault();

            if (receptionistUser == null)
            {
                return new ReceptionistObject { Success = false, Message = "User not found" };
            }

            var receptionistInformation = context.receptionist_informations.Where(d => d.system_user_ref == receptionistUser.id).FirstOrDefault();

            if (receptionistInformation == null)
            {
                return new ReceptionistObject { Success = false, Message = "User is not a receptionist" };
            }

            ReceptionistObject receptionist = new ReceptionistObject()
            {
                id = receptionistUser.id,
                CPR = receptionistUser.cpr,
                FirstName = receptionistUser.first_name,
                LastName = receptionistUser.last_name,
                Email = receptionistUser.email,
                Phone = receptionistUser.phone_number,
                JobTitle = receptionistInformation.job_title,
                Success = true,
                Message = "Receptionist information retrieved successfully"
            };

            return receptionist;
        }

        public static ClinicManagerObject GetClinicManagerObjectByID(AP_Context context, Guid managerId)
        {
            var managerUser = context.system_users.Where(s => s.id == managerId).FirstOrDefault();

            if (managerUser == null)
            {
                return new ClinicManagerObject { Success = false, Message = "User not found" };
            }

            var managerInformation = context.clinic_manager_informations.Where(d => d.system_user_ref == managerUser.id).FirstOrDefault();

            if (managerInformation == null)
            {
                return new ClinicManagerObject { Success = false, Message = "User is not a clinic manager" };
            }

            ClinicManagerObject manager = new ClinicManagerObject()
            {
                id = managerUser.id,
                CPR = managerUser.cpr,
                FirstName = managerUser.first_name,
                LastName = managerUser.last_name,
                Email = managerUser.email,
                Phone = managerUser.phone_number,
                JobTitle = managerInformation.job_title,
                Success = true,
                Message = "Clinic manager information retrieved successfully"
            };

            return manager;
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
            public List<string> Specializations { get; set; }

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
