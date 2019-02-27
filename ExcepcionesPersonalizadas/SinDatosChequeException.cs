namespace CAPCajaLite.ExcepcionesPersonalizadas
{
    using System;
    using System.Runtime.Serialization;

    public class SinDatosChequeException : Exception
    {
        /// <summary>
        /// Crea la excepción
        /// </summary>
        public SinDatosChequeException() : base()
        {
        }

        /// <summary>
        /// Crea la excepción con descripción
        /// </summary>
        /// <param name="message">Descripción de la excepción</param>
        public SinDatosChequeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Crea la excepción con descripción y una causa interna
        /// </summary>
        /// <param name="message">Descripción de la excepción</param>
        /// <param name="innerException">Causa interna de la excepción</param>
        public SinDatosChequeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Crea la excepción de datos serializados.
        /// escenario habitual es cuando se produce una excepción en algún lugar de la estación de trabajo remota
        /// y tenemos que volver a crear / volver a lanzar la excepción en la máquina local
        /// </summary>
        /// <param name="info">Información de serialization</param>
        /// <param name="context">Contexto de serealización</param>
        protected SinDatosChequeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
