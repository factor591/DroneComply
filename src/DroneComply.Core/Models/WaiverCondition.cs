namespace DroneComply.Core.Models;

public class WaiverCondition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WaiverId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsSatisfied { get; set; }
    public DateTime? SatisfiedOn { get; set; }
}
