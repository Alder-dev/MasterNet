using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Cursos.CursoUpdate;

public class CursoUpdateCommand
{
    public record CursoUpdateCommandRequest(CursoUpdateRequest cursoUpdateRequest, Guid? CursoId) : IRequest<Result<Guid>>;

    internal class CursoUpdateCommandHandler : IRequestHandler<CursoUpdateCommandRequest, Result<Guid>>
    {
        private readonly MasterNetDbContext _context;

        public CursoUpdateCommandHandler(MasterNetDbContext context)
        {
            _context = context;
        }

        public  async Task<Result<Guid>> Handle(CursoUpdateCommandRequest request, CancellationToken cancellationToken)
        {
            var cursoId = request.CursoId;
            var curso = await _context.Cursos!.FirstOrDefaultAsync(x => x.Id == cursoId);

            if (curso is null)
            {
                return Result<Guid>.Failure("Curso no encontrado");
            }

            curso.Titulo = request.cursoUpdateRequest.Titulo;
            curso.Descripcion = request.cursoUpdateRequest.Descripcion;
            curso.FechaPublicacion = request.cursoUpdateRequest.FechaPublicacion;

            _context.Entry(curso).State = EntityState.Modified;
            var resultado = await _context.SaveChangesAsync(cancellationToken) > 0;

            return resultado ? Result<Guid>.Success(curso.Id) : Result<Guid>.Failure("Error al actualizar el curso");
        }
    }

    public class CursoUpdateCommandRequestValidator : AbstractValidator<CursoUpdateCommandRequest>
    {
        public CursoUpdateCommandRequestValidator()
        {
            RuleFor(x => x.cursoUpdateRequest).SetValidator(new CursoUpdateValidator());
            RuleFor(x => x.CursoId).NotNull();
        }
    }
}