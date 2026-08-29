using FluentValidation;
using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IMapper;
using Finanzas.Application.Interfaces.IServices;
using Finanzas.Domain.Entidades;
using Finanzas.Domain.Excepciones;
using Finanzas.Domain.Interfaces;

namespace Finanzas.Application.Servicios;

public class ServicioCategoria : IServicioCategoria
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapperCategoria _mapperCategoria;
    private readonly IValidator<CrearCategoriaDto> _crearValidador;
    private readonly IValidator<ActualizarCategoriaDto> _actualizarValidador;

    public ServicioCategoria(
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork,
        IMapperCategoria mapperCategoria,
        IValidator<CrearCategoriaDto> crearValidador,
        IValidator<ActualizarCategoriaDto> actualizarValidador)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
        _mapperCategoria = mapperCategoria;
        _crearValidador = crearValidador;
        _actualizarValidador = actualizarValidador;
    }

    public async Task<CategoriaResponseDto> CrearAsync(Guid usuarioId, CrearCategoriaDto dto, CancellationToken cancellationToken = default)
    {
        await _crearValidador.ValidateAndThrowAsync(dto, cancellationToken);

        var categoria = _mapperCategoria.ACrear(usuarioId, dto);

        if (dto.SubcategoriaDeId is Guid padreId)
        {
            var padre = await _categoriaRepository.ObtenerPorIdAsync(padreId, usuarioId, cancellationToken)
                ?? throw new RecursoNoEncontradoException(nameof(Categoria), padreId);

            categoria.AsignarComoSubcategoriaDe(padre);
        }

        await ValidarNombreUnicoAsync(usuarioId, dto.Nombre, categoria.SubcategoriaDeId, cancellationToken);

        await _categoriaRepository.AgregarAsync(categoria, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return _mapperCategoria.AResponseDto(categoria);
    }

    public async Task<CategoriaResponseDto> ActualizarAsync(Guid usuarioId, Guid id, ActualizarCategoriaDto dto, CancellationToken cancellationToken = default)
    {
        await _actualizarValidador.ValidateAndThrowAsync(dto, cancellationToken);

        var categoria = await _categoriaRepository.ObtenerPorIdAsync(id, usuarioId, cancellationToken)
            ?? throw new RecursoNoEncontradoException(nameof(Categoria), id);

        if (dto.SubcategoriaDeId is Guid padreId)
        {
            if (padreId == id)
            {
                throw new CategoriaInvalidaException("Una categoría no puede ser subcategoría de sí misma.");
            }

            var padre = await _categoriaRepository.ObtenerPorIdAsync(padreId, usuarioId, cancellationToken)
                ?? throw new RecursoNoEncontradoException(nameof(Categoria), padreId);

            categoria.AsignarComoSubcategoriaDe(padre);
        }
        else
        {
            categoria.SubcategoriaDeId = null;
        }

        await ValidarNombreUnicoAsync(usuarioId, dto.Nombre, categoria.SubcategoriaDeId, cancellationToken, id);

        _mapperCategoria.AplicarCambios(categoria, dto);

        _categoriaRepository.Actualizar(categoria);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return _mapperCategoria.AResponseDto(categoria);
    }

    public async Task EliminarAsync(Guid usuarioId, Guid id, CancellationToken cancellationToken = default)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(id, usuarioId, cancellationToken)
            ?? throw new RecursoNoEncontradoException(nameof(Categoria), id);

        var todas = await _categoriaRepository.ListarPorUsuarioAsync(usuarioId, cancellationToken);
        if (todas.Any(c => c.SubcategoriaDeId == id))
        {
            throw new CategoriaInvalidaException("No se puede eliminar una categoría con subcategorías. Eliminá las subcategorías primero.");
        }

        _categoriaRepository.Eliminar(categoria);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);
    }

    public async Task<CategoriaResponseDto> ObtenerPorIdAsync(Guid usuarioId, Guid id, CancellationToken cancellationToken = default)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(id, usuarioId, cancellationToken)
            ?? throw new RecursoNoEncontradoException(nameof(Categoria), id);

        return _mapperCategoria.AResponseDto(categoria);
    }

    public async Task<IReadOnlyList<CategoriaResponseDto>> ObtenerTodosAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var categorias = await _categoriaRepository.ListarPorUsuarioAsync(usuarioId, cancellationToken);
        return categorias.Select(_mapperCategoria.AResponseDto).ToList();
    }

    private async Task ValidarNombreUnicoAsync(Guid usuarioId, string nombre, Guid? subcategoriaDeId, CancellationToken cancellationToken, Guid? idAExcluir = null)
    {
        var todas = await _categoriaRepository.ListarPorUsuarioAsync(usuarioId, cancellationToken);
        var duplicada = todas.Any(c =>
            c.Id != idAExcluir &&
            c.SubcategoriaDeId == subcategoriaDeId &&
            string.Equals(c.Nombre, nombre, StringComparison.OrdinalIgnoreCase));

        if (duplicada)
        {
            throw new CategoriaInvalidaException($"Ya existe una categoría llamada \"{nombre}\" en ese nivel.");
        }
    }
}
