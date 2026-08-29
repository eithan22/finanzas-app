using FluentValidation;
using Finanzas.Domain.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Finanzas.Api.Middleware;


// Único lugar del sistema que atrapa una excepción no manejada y la
// traduce a una respuesta HTTP. Así ningún controller necesita try/catch
// propio: tira la excepción de dominio y esto se encarga.

public class ManejadorExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepciones> _logger;

    public ManejadorExcepciones(ILogger<ManejadorExcepciones> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            var erroresPorCampo = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var detalleValidacion = new ValidationProblemDetails(erroresPorCampo)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Datos inválidos"
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(detalleValidacion, cancellationToken);
            return true;
        }

        // RF-28: un recurso de otro usuario también cae acá como 404 (no
        // 403) a propósito — confirmar con 403 que el recurso existe pero
        // no es tuyo ya sería filtrar información.
        var (statusCode, titulo) = exception switch
        {
            RecursoNoEncontradoException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            CredencialesInvalidasException or
            CuentaNoVerificadaException or
            TokenRecuperacionInvalidoException or
            RefreshTokenInvalidoException => (StatusCodes.Status401Unauthorized, "No autorizado"),
            DomainException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Error no controlado en {Path}", httpContext.Request.Path);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Ocurrió un error inesperado."
                : exception.Message
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
