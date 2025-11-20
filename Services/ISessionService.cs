using CodingTracker.Models;

namespace CodingTracker.Services
{
    internal interface ISessionService
    {
        void AddSession(CodingSession session);
        void UpdateSession(int id, DateTime newStart, DateTime newEnd);
        void DeleteSession(int id);
        List<CodingSession> GetSessionsByMonth(int year, int month);
        
        
        List<int> GetAvailableMonths(int year);
    List<int> GetAvailableYears();
    }
}