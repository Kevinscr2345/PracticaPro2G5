using System.Net;

namespace PracticaPro2G5.Middleware
{
    /// <summary>
    /// Manejador global de excepciones (requisito del enunciado). Envuelve toda la tubería
    /// HTTP en un try/catch, registra el error y traduce el tipo de excepción a un código
    /// HTTP y un mensaje amigable, respondiendo siempre en formato JSON.
    /// </summary>
    public class MiddlewareGlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareGlobalExceptionHandler> _logger;

        public MiddlewareGlobalExceptionHandler(RequestDelegate next, ILogger<MiddlewareGlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo una excepción no controlada: {Mensaje}", ex.Message);
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private static async Task ManejarExcepcionAsync(HttpContext context, Exception excepcion)
        {
            var respuesta = excepcion switch
            {
                ArgumentNullException => new ExceptionResponse(HttpStatusCode.BadRequest, "Petición inválida: faltan datos requeridos."),
                ArgumentException => new ExceptionResponse(HttpStatusCode.BadRequest, "Petición inválida."),
                KeyNotFoundException => new ExceptionResponse(HttpStatusCode.NotFound, "El recurso solicitado no existe."),
                UnauthorizedAccessException => new ExceptionResponse(HttpStatusCode.Unauthorized, "No tiene autorización para realizar esta acción."),
                NotImplementedException => new ExceptionResponse(HttpStatusCode.NotImplemented, "Funcionalidad no implementada."),
                _ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)respuesta.StatusCode;

            await context.Response.WriteAsJsonAsync(respuesta);
        }
    }
}
