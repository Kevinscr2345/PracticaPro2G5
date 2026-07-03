namespace PracticaPro2G5.BLL.Dtos
{
    /// <summary>
    /// Envoltorio (Envelope Pattern) que estandariza TODA respuesta de los servicios.
    /// El frontend siempre interpreta el resultado leyendo estas mismas propiedades.
    /// </summary>
    public class Respuesta<T>
    {
        public bool EsCorrecto { get; set; }

        public string Mensaje { get; set; }

        public T? Dato { get; set; }

        public int Codigo { get; set; }

        public Respuesta()
        {
            EsCorrecto = true;
            Mensaje = "Operación realizada correctamente.";
            Codigo = 200;
        }
    }
}
