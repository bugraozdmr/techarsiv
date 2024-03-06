using Entities.Models;

namespace Repositories.Contracts;

public interface IReportRepository
{
    IQueryable<Report> getAllReports(bool trackchanges);
    void CreateReport(Report report);
    void deleteReport(Report report);
}