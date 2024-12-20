using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApp.Models
{
    public class Appointment
    {
        [Key] // Primary Key
        public long Id { get; set; }

        [ForeignKey("Clinic")] // Foreign Key to Clinic
        public long ClinicId { get; set; }

        public Clinic Clinic { get; set; } // Navigation Property


        [ForeignKey("Doctor")] // Foreign Key to Doctor
        public long DoctorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        public Doctor Doctor { get; set; } // Navigation Property

        public TimeSpan AppointmentTime { get; set; }


        // [DataType(DataType.Time)]
        // public int AppointmentTimeInMinutes { get; set; }

    // TimeSpan ile kolay kullanım için dönüşüm
    // [NotMapped]
    // public TimeSpan AppointmentTime
    // {
    //     get => TimeSpan.FromMinutes(AppointmentTimeInMinutes);
    //     set => AppointmentTimeInMinutes = (int)value.TotalMinutes;
    // }
    public string PatientId { get; set; } // User ID from Identity
    }
}
