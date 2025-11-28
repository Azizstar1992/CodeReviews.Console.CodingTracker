using CodingTracker.Models;
using CodingTracker.Services;



internal class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;

    public SessionService(ISessionRepository repository)
    {
        _repository = repository;
    }

    public List<int> GetAvailableYears() => _repository.GetAvailableYears();
    public List<int> GetAvailableMonths(int year) => _repository.GetAvailableMonths(year);

    public void AddSession(CodingSession session)
    {
        _repository.Insert(session);
    }

    public bool UpdateSession(int id, DateTime newStart, DateTime newEnd)
    {
        return _repository.Update(id, newStart, newEnd);
    }
    public bool DeleteSession(int id)
    {
        if (!SessionExists(id)) return false;

        return _repository.Delete(id);
        
    }

    public List<CodingSession> GetSessionsByMonth(int year, int month)
    {
        return _repository.GetByMonth(year, month);
    }

    public bool ValidateTimes(DateTime start, DateTime end)
    {
        return end >= start;
    }

    public bool SessionExists(int id)
    {
        return _repository.Exists(id);
    }

}