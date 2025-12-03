using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Cursos.CursoCreate;

public class CursoCreateCommand
{
    public record CursoCreateCommandRequest(CursoCreateRequest cursoCreateRequest) : IRequest<Result<Guid>>;

    internal class CursoCreateCommandHandler : IRequestHandler<CursoCreateCommandRequest, Result<Guid>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IPhotoService _photoService;

        public CursoCreateCommandHandler(MasterNetDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        public async Task<Result<Guid>> Handle(CursoCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var cursoId = Guid.NewGuid();
            var curso = new Curso
            {
                Id = cursoId,
                Titulo = request.cursoCreateRequest.Titulo,
                Descripcion = request.cursoCreateRequest.Descripcion,
                FechaPublicacion = request.cursoCreateRequest.FechaPublicacion
            };

            if (request.cursoCreateRequest.Foto is not null)
            {
                var photoUploadResult = await _photoService.AddPhoto(request.cursoCreateRequest.Foto);
                var photo = new Photo
                {
                    Id = Guid.NewGuid(),
                    Url = photoUploadResult.Url,
                    PublicId = photoUploadResult.PublicId,
                    CursoId = cursoId
                };

                curso.Photos = new List<Photo> { photo };
            }

            if (request.cursoCreateRequest.InstructorId is not null)
            {
                var instructor = _context.Instructores!.FirstOrDefault(x => x.Id == request.cursoCreateRequest.InstructorId);

                if (instructor is null)
                {
                    return Result<Guid>.Failure("No se encontró el instructor");
                }

                curso.Instructores = new List<Instructor> { instructor };
            }

            if (request.cursoCreateRequest.PrecioId is not null)
            {
                var precio = await _context.Precios!.FirstOrDefaultAsync(x => x.Id == request.cursoCreateRequest.PrecioId);

                if (precio is null)
                {
                    return Result<Guid>.Failure("No se encontró el precio");
                }

                curso.Precios = new List<Precio> { precio };
            }

            _context.Add(curso);
            var resultado = await _context.SaveChangesAsync(cancellationToken) > 0;

            return resultado ? Result<Guid>.Success(curso.Id) : Result<Guid>.Failure("Error al crear el curso");
        }
    }

    public class CursoCreateCommandValidator : AbstractValidator<CursoCreateCommandRequest>
    {
        public CursoCreateCommandValidator()
        {
            RuleFor(x => x.cursoCreateRequest).SetValidator(new CursoCreateValidator());
        }
    }

}