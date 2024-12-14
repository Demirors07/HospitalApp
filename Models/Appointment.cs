using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApp.Models
{
    public class Appointment
    {
        [Key] // Primary Key
        public long Id { get; set; }

        [Required]
        [ForeignKey("Clinic")] // Foreign Key to Clinic
        public long ClinicId { get; set; }

        public Clinic Clinic { get; set; } // Navigation Property

        [Required]
        [ForeignKey("Doctor")] // Foreign Key to Doctor
        public long DoctorId { get; set; }
        public string DoctorName { get; set; } // Yeni Alan

        public Doctor Doctor { get; set; } // Navigation Property

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [DataType(DataType.Time)]
        public int AppointmentTimeInMinutes { get; set; }

    // TimeSpan ile kolay kullanım için dönüşüm
    [NotMapped]
    public TimeSpan AppointmentTime
    {
        get => TimeSpan.FromMinutes(AppointmentTimeInMinutes);
        set => AppointmentTimeInMinutes = (int)value.TotalMinutes;
    }

         [Required]
    public string PatientId { get; set; } // User ID from Identity
    }
}
