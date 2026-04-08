namespace AP_clinical_system.Models.Enums
{
    public enum appointment_status
    {
        requested = 1000,
        confirmed = 1001,
        checked_in = 1002,
        in_progress = 1003,
        completed = 1004,
        cancelled = 1005,
        no_show = 1006
    }

    public enum appointment_time_slot
    {
        slot_08_00 = 1000,
        slot_08_30 = 1001,
        slot_09_00 = 1002,
        slot_09_30 = 1003,
        slot_10_00 = 1004,
        slot_10_30 = 1005,
        slot_11_00 = 1006,
        slot_11_30 = 1007,
        slot_12_00 = 1008,
        slot_12_30 = 1009,
        slot_13_00 = 1010,
        slot_13_30 = 1011,
        slot_14_00 = 1012,
        slot_14_30 = 1013,
        slot_15_00 = 1014,
        slot_15_30 = 1015,
        slot_16_00 = 1016,
        slot_16_30 = 1017,
        slot_17_00 = 1018
    }

    public enum user_role
    {
        system_admin = 1000,
        clinic_manager = 1001,
        doctor = 1002,
        receptionist = 1003,
        patient = 1004
    }
}
