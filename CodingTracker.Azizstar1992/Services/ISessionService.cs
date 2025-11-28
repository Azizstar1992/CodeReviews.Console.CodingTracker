using CodingTracker.Models;

namespace CodingTracker.Services
{
    internal interface ISessionService
    {
        void AddSession(CodingSession session);

        bool UpdateSession(int id, DateTime newStart, DateTime newEnd);
        bool DeleteSession(int id);

        List<CodingSession> GetSessionsByMonth(int year, int month);

        public bool SessionExists(int id);

        List<int> GetAvailableMonths(int year);

        List<int> GetAvailableYears();

        public bool ValidateTimes(DateTime start, DateTime end);
    }
}