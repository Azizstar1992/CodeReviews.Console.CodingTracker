using CodingTracker.Models;
using CodingTracker.Services;
using CodingTracker.Data;
using Dapper;

internal class SessionRepository : ISessionRepository
{
    public List<int> GetAvailableYears()
    {
        using var conn = Database.CreateConnection();

        var sql = @"
            SELECT DISTINCT CAST(strftime('%Y', StartTime) AS INT) AS Year
            FROM CodingSessions
            ORDER BY Year DESC;
        ";

        return conn.Query<int>(sql).ToList();
    }

    public List<int> GetAvailableMonths(int year)
    {
        using var conn = Database.CreateConnection();

        var sql = @"
            SELECT DISTINCT CAST(strftime('%m', StartTime) AS INT) AS Month
            FROM CodingSessions
            WHERE strftime('%Y', StartTime) = @year
            ORDER BY Month ASC;
        ";

        return conn.Query<int>(sql, new { year = year.ToString("D4") }).ToList();
    }

    public void Insert(CodingSession session)
    {
        using var conn = Database.CreateConnection();

        var durationMinutes = (int)(session.EndTime - session.StartTime).TotalMinutes;

        var sql = @"
        INSERT INTO CodingSessions (StartTime, EndTime, DurationMinutes)
        VALUES (@StartTime, @EndTime, @DurationMinutes);
    ";

        conn.Execute(sql, new
        {
            session.StartTime,
            session.EndTime,
            DurationMinutes = durationMinutes
        });
    }

    public void Update(int id, DateTime newStart, DateTime newEnd)
    {
        using var conn = Database.CreateConnection();

        var sql = @"
            UPDATE CodingSessions
            SET StartTime = @newStart,
                EndTime = @newEnd
            WHERE Id = @id;
        ";

        conn.Execute(sql, new { id, newStart, newEnd });
    }

    public void Delete(int id)
    {
        using var conn = Database.CreateConnection();

        var sql = @"
            DELETE FROM CodingSessions
            WHERE Id = @id;
        ";

        conn.Execute(sql, new { id });
    }

    public List<CodingSession> GetByMonth(int year, int month)
    {
        using var conn = Database.CreateConnection();

        var sql = @"
            SELECT Id, StartTime, EndTime
            FROM CodingSessions
            WHERE strftime('%Y', StartTime) = @year
              AND strftime('%m', StartTime) = @month
            ORDER BY StartTime;
        ";

        return [.. conn.Query<CodingSession>(sql, new {
            year = year.ToString("D4"),
            month = month.ToString("D2")
        })];
    }
}
