using System.ComponentModel.DataAnnotations;

namespace BookingWeb.Models;

/// <summary>Client contact details entered on the last booking step.</summary>
public class BookingForm
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter your phone number.")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(30, ErrorMessage = "Phone number is too long.")]
    public string Phone { get; set; } = "";
}
