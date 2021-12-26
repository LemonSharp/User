using System.ComponentModel.DataAnnotations;

namespace LemonSharp.User.API.Models;

/// <summary>
/// RegisterViewModel
/// </summary>
public class RegisterViewModel
{
    /// <summary>
    /// Email
    /// </summary>
    [Required]
    [RegularExpression("^1[2-9][0-9]{9}$")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}
