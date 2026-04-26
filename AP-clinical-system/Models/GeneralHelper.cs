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
           
            var specializations = context.doctor_information_specialization_mtms.Where(d => d.doctor_information_ref == doctorInformation.id).Select(d => d.doctor_specialization_ref).ToList();

            if (specializations == null || specializations.Count == 0)
            {
                return new DoctorObject { Success = false, Message = "Doctor has no specializations" };
            }

            var specializationNames = context.doctor_specializations.Where(s => specializations.Contains(s.id)).Select(s => s.specialization_name).ToList();

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



    }
}
