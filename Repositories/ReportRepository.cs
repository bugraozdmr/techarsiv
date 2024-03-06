using Entities.Models;
using Repositories.Contracts;
using Repositories.EF;


namespace Repositories;

public class ReportRepository : RepositoryBase<Report>,IReportRepository
{
    public ReportRepository(RepositoryContext context) : base(context)
    {
    }

    public IQueryable<Report> getAllReports(bool trackchanges) => FindAll(trackchanges);

    public void CreateReport(Report report) => Create(report);

    public void deleteReport(Report report) => Remove(report);
}