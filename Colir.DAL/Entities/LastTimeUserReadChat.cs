namespace DAL.Entities;

public class LastTimeUserReadChat : BaseEntity
{
    public long RoomId { get; set; }
    public long UserId { get; set; }
    public DateTime Timestamp { get; set; }
}