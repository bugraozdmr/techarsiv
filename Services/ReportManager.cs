using AutoMapper;
using Entities.Dtos.Report;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services;

public class ReportManager : IReportService
{
    private readonly IRepositoryManager _manager;
    private readonly IMapper _mapper;


    public ReportManager(IRepositoryManager manager,
        IMapper mapper)
    {
        _manager = manager;
        _mapper = mapper;
    }

    public async Task CreateReport(CreateReportDto dto)
    {
        var report = _mapper.Map<Report>(dto);
        
        _manager.ReportRepository.CreateReport(report);
        await _manager.SaveAsync();
    }

    public async Task deleteAllReport(int ReportId)
    {
        var report = _manager
            .ReportRepository
            .getAllReports(false)
            .Where(s => s.ReportId.Equals(ReportId))
            .FirstOrDefault();
        
        _manager.ReportRepository.deleteReport(report);
        await _manager.SaveAsync();
    }

    public async Task deleteOneReport(int ReportId)
    {
        var report = _manager
            .ReportRepository
            .getAllReports(false)
            .Where(s => s.ReportId.Equals(ReportId))
            .FirstOrDefault();
        
        _manager.ReportRepository.deleteReport(report);
        await _manager.SaveAsync();
    }

    public IQueryable<Report> GetAllReports(bool trackChanges)
    {
        return _manager.ReportRepository.getAllReports(false);
    }
}