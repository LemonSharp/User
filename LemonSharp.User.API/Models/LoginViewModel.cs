using System.ComponentModel.DataAnnotations;

namespace LemonSharp.User.API.Models;

/// <summary>
/// LoginViewModel
/// </summary>
public class LoginViewModel
{
    [Required]
    public string PhoneNumer { get; set; } = null!;

    [Required]
    [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
