using Entities.Dtos.Report;
using Entities.Models;

namespace Services.Contracts;

public interface IReportService
{
    Task CreateReport(CreateReportDto dto);
    Task deleteAllReport(string userId);
    Task deleteOneReport(string userId);
    IQueryable<Report> GetAllReports(bool trackChanges);
}