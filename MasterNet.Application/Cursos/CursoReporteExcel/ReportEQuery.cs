using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Cursos.CursoReporteExcel;

public class ReportEQuery
{
    public record ReportEQueryRequest : IRequest<MemoryStream>;

    internal class ReportEQueryHandler : IRequestHandler<ReportEQueryRequest, MemoryStream>
    {
        private readonly MasterNetDbContext _context;
        private readonly ReportService<Curso> _reportService;

        public ReportEQueryHandler(MasterNetDbContext context, ReportService<Curso> reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<MemoryStream> Handle(ReportEQueryRequest request, CancellationToken cancellationToken)
        {
            var cursos = await _context.Cursos!.Take(10).Skip(0).ToListAsync();

            return await _reportService.GetCsvReport(cursos);
        }
    }
}