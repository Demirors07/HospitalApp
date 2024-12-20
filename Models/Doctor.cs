using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Models
{
    public class Doctor
    {
        public long Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Clinic")]
        public long ClinicId { get; set; } // Foreign Key
        public Clinic Clinic { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}