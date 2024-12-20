using System.ComponentModel.DataAnnotations;

namespace HospitalApp.ViewModels {
    public class AvailableTimeViewModel
{
    public long DoctorId { get; set; }
    public string DoctorName { get; set; } // Yeni Alan
    public DateTime AppointmentDate { get; set; }
 
    public string PatientID { get; set; } // Yeni Alan
}

}
