using CodingTracker.Models;


namespace CodingTracker.Services
{
    internal interface ISessionRepository
    {
        void Insert(CodingSession session);
        bool Update(int id, DateTime newStart, DateTime newEnd);
        bool Delete(int id);
        List<CodingSession> GetByMonth(int year, int month);

        List<int> GetAvailableYears();
        List<int> GetAvailableMonths(int year);
        public bool Exists(int id); 
    }
}