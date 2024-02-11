using System.ComponentModel.DataAnnotations;

namespace CHALLENGEIntuit_Back
{
    public class Clientes
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string CUIT { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}
   