using CaynayBot.Models;
using CaynayBot.Repositories;

namespace CaynayBot.Services
{
    public interface IReportService
    {
        Task UpdateReport(Report Report);
        Task AddReport(Report Report);
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report?> DeleteByTime(DateTime result);
    }
    
    
    public class ReportService : IReportService
    {
        private readonly IRepository<Report> _reportService;

        public ReportService(IRepository<Report> userReport)
        {
            _reportService = userReport;
        }

        public async Task UpdateReport(Report Report)
        {
            await Task.Run(() => _reportService.Update(Report));
            await _reportService.SaveChangesAsync();
        }
        public async Task AddReport(Report Report)
        {
            await Task.Run(() => _reportService.AddAsync(Report));
            await _reportService.SaveChangesAsync();
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            var result = await _reportService.GetAllAsync();
            return [.. result];
        }

        public async Task<Report?> DeleteByTime(DateTime createdAt)
        {
            var reports = await _reportService.GetAllAsync();

            foreach (var rep in reports)
            {
                var t1 = rep.CreatedAt.Ticks;
                var t2 = createdAt.Ticks;

                if (t2 - t1 < 50)
                {
                    if(rep != null)
                        _reportService.Delete(rep);
                    await _reportService.SaveChangesAsync();
                    return rep;
                }
            }

            return null;
        }
    }
}