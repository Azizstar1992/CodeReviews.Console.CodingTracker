using CodingTracker.Models;


namespace CodingTracker.Services
{
    internal interface ISessionRepository
    {
        void Insert(CodingSession session);
        void Update(int id, DateTime newStart, DateTime newEnd);
        void Delete(int id);
        List<CodingSession> GetByMonth(int year, int month);

        List<int> GetAvailableYears();
        List<int> GetAvailableMonths(int year);
        
    }
}