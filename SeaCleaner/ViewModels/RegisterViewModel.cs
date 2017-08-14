using System.ComponentModel.DataAnnotations;

namespace SeaCleaner.ViewModels
{
    public sealed class RegisterViewModel
    {
        [Required]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Потверждение пароля")]
        public string ConfirmPassword { get; set; }
    }
}
