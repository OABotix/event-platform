using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels;

public class RegisterVm
{
    [Required(ErrorMessage = "Full name is required")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number")]
    [Display(Name = "Phone Number (optional)")]
    public string? PhoneNumber { get; set; }
}