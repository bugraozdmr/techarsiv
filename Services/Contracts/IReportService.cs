using Entities.Dtos.Report;
using Entities.Models;

namespace Services.Contracts;

public interface IReportService
{
    Task CreateReport(CreateReportDto dto);
    Task deleteAllReport(int ReportId);
    Task deleteOneReport(int ReportId);
    IQueryable<Report> GetAllReports(bool trackChanges);
}