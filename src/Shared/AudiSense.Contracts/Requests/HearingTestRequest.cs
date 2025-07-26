using System.ComponentModel.DataAnnotations;

namespace AudiSense.Contracts.Requests;

public class HearingTestRequest
{
    [Required(ErrorMessage = "Tester name is required")]
    [StringLength(100, ErrorMessage = "Tester name cannot exceed 100 characters")]
    public string TesterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date conducted is required")]
    public DateTime DateConducted { get; set; }

    [Required(ErrorMessage = "Result is required")]
    [StringLength(500, ErrorMessage = "Result cannot exceed 500 characters")]
    public string Result { get; set; } = string.Empty;
}
