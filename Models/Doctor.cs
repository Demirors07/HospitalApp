using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Models
{
    public class Doctor
    {
        public long Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ClinicId")]
        public long ClinicId { get; set; } // Foreign Key
        public Clinic Clinic { get; set; }
        public List<AvailableTime> AvailableTimes { get; set; } = new();
    }
}