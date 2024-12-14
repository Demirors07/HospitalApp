using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


// namespace HospitalApp.Models{
//     public class Doctor{

//         public int DoctorID {get; set;}

//         public string Name {get; set;} = String.Empty;

//         public string Surname {get; set;} = String.Empty;

//         public long PhoneNumber {get; set;}

//         public string Clinic {get; set;} = String.Empty;
        
//         //public long ProductCategoryID {get; set;}
//     }
// }

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