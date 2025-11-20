
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

    public void UpdateSession(int id, DateTime newStart, DateTime newEnd)
    {
        _repository.Update(id,newStart,newEnd);
    }

    public void DeleteSession(int id)
    {
       _repository.Delete(id);
    }

    public List<CodingSession> GetSessionsByMonth(int year, int month)
{
    return _repository.GetByMonth(year, month);
}


}