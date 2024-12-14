using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;

namespace HospitalApp{

public class AvailableTime
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Doctor")]
    public long DoctorId { get; set; }

    public Doctor Doctor { get; set; } // Navigation Property

    [Required]
    public DateTime Date { get; set; } // Gün

    [Required]
    public TimeSpan StartTime { get; set; } // Başlangıç zamanı

    [Required]
    public TimeSpan EndTime { get; set; } // Bitiş zamanı
}
}