namespace AmbulanceRider.Models;

public class UpdateTripStatusDto
{
    public int Id { get; set; }
    public TripStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public bool ForceComplete { get; set; }
}
