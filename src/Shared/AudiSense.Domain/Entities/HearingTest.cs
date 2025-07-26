namespace AudiSense.Domain.Entities;

public class HearingTest
{
    public int Id { get; set; }
    public string TesterName { get; set; } = string.Empty;
    public DateTime DateConducted { get; set; }
    public string Result { get; set; } = string.Empty;
}
