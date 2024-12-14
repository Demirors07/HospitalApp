using System.ComponentModel.DataAnnotations;

namespace HospitalApp.ViewModels {
    public class LoginViewModel{
        [Required]
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        public string? ReturnUrl { get; set; } // Opsiyonel olarak tanımlayın
    }
}