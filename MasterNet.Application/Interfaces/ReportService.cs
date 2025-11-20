using MasterNet.Domain;

namespace MasterNet.Application.Interfaces;

public interface ReportService<T> where T : BaseEntity
{
    Task<MemoryStream> GetCsvReport(List<T> records);
}