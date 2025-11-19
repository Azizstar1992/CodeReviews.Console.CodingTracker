using CodingTracker.Models;

namespace CodingTracker.Services
{
    internal interface ISessionService
    {
        void AddSession(CodingSession session);
        void UpdateSession(int id, DateTime newStart, DateTime newEnd);
        void DeleteSession(int id);
        List<CodingSession> GetSessionsByMonth(int year, int month);
        string GenerateMonthlyReport(int year, int month);
    }
}