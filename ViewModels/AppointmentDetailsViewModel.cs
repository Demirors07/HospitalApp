using System.Collections.Generic;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public List<Appointment> PastAppointments { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; }
    }
}