
namespace CodingTracker.Models
{
public class CodingSession
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public int Duration => (int)(EndTime - StartTime).TotalSeconds;

    // Parameterless constructor for Dapper
    public CodingSession() { }

    public CodingSession(int id, DateTime start, DateTime end)
    {
        Id = id;
        StartTime = start;
        EndTime = end;
    }

    public string GetDurationAsString()
    {
        TimeSpan duration = EndTime - StartTime;
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
                             (int)duration.TotalHours,
                             duration.Minutes,
                             duration.Seconds);
    }
}
}