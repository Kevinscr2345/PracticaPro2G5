using System.Net;

namespace PracticaPro2G5.Middleware
{
    /// <summary>
    /// DTO inmutable (record) para la respuesta del manejador global de excepciones.
    /// </summary>
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
}
