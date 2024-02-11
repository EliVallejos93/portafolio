using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CHALLENGEIntuit_Back
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {

        private readonly APIDbContext _context;

        public ClientesController(APIDbContext context)
        {
            _context = context;
        }

        // POST api/Clientes/Insert
        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] Clientes clientes)
        {
            try
            {
                string mensaje = ValidarDatos(clientes);
                if (mensaje != "") return BadRequest(new { code = "ERR", message = mensaje });

                _context.Clientes.Add(clientes);
                _context.SaveChanges();

                return Ok(new { code = "OK", message = "Creacion exitosa" });
            }
            catch (Exception e)
            {
                return BadRequest(new { code = "ERR", message = e.Message });
            }
        }

        // GET api/Clientes/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var clientes = await _context.Clientes.ToListAsync();

                return Ok(new { code = "OK", message = clientes });
            }
            catch (Exception e)
            {
                return BadRequest(new { code = "ERR", message = e.Message });
            }
        }

        // GET api/Clientes/GetID
        [HttpGet("GetID")]
        public async Task<IActionResult> GetID(int id)
        {
            try
            {
                var cliente = _context.Clientes.Find(id);
                if (cliente == null)
                    return NotFound(new { code = "ERR", message = "Cliente no encontrado" });

                return Ok(new { code = "OK", message = cliente });
            }
            catch (Exception e)
            {
                return BadRequest(new { code = "ERR", message = e.Message });
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string nombre)
        {
            try
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.Nombre == nombre);
                if (cliente == null)
                    return NotFound(new { code = "ERR", message = "Cliente no encontrado" });

                return Ok(new { code = "OK", message = cliente });
            }
            catch (Exception e)
            {
                return BadRequest(new { code = "ERR", message = e.Message });
            }
        }


        // POST api/Clientes/Update
        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] Clientes clientes)
        {
            try
            {
                var clienteEncontrado = _context.Clientes.Find(clientes.ID);
                if (clienteEncontrado == null)
                    return NotFound(new { code = "ERR", message = "Cliente no encontrado" });

                string mensaje = ValidarDatos(clientes);
                if (mensaje != "") return BadRequest(new { code = "ERR", message = mensaje });

                clienteEncontrado.Nombre = clientes.Nombre;
                clienteEncontrado.Apellido = clientes.Apellido;
                clienteEncontrado.FechaNacimiento = clientes.FechaNacimiento;
                clienteEncontrado.CUIT = clientes.CUIT;
                clienteEncontrado.Domicilio = clientes.Domicilio;
                clienteEncontrado.Telefono = clientes.Telefono;
                clienteEncontrado.Email = clientes.Email;

                _context.SaveChanges();

                return Ok(new { code = "OK", message = "Actualizacion exitosa" });
            }
            catch (Exception e)
            {
                return BadRequest(new { code = "ERR", message = e.Message });
            }
        }

        private string ValidarDatos(Clientes clientes)
        {
            #region Validar Nombre
            if (clientes.Nombre == "")
                return "Nombre es obligatorio";
            #endregion

            #region Validar Apellido
            if (clientes.Apellido == "")
                return "Apellido es obligatorio";
            #endregion

            #region validar CUIL
            // Eliminar caracteres no numéricos del CUIT
            string cleanedCuit = Regex.Replace(clientes.CUIT, "[^0-9]", "");

            // Verificar la longitud del CUIT
            if (cleanedCuit.Length != 11)
                return "El CUIT no es válido";

            // Verificar que el prefijo sea válido
            string prefijo = cleanedCuit.Substring(0, 2);
            if (prefijo != "20" && prefijo != "23" && prefijo != "24" && prefijo != "27")
                return "El CUIT no es válido";

            // Verificar que el número de documento sea válido
            string numeroDocumento = cleanedCuit.Substring(2, 8);
            if (numeroDocumento == "00000000" || numeroDocumento.Distinct().Count() == 1)
                return "El CUIT no es válido";

            // Verificar el dígito verificador
            int[] multipliers = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            int temp;
            int verifierDigit = int.Parse(cleanedCuit.Substring(10, 1));
            for (int i = 0; i < 10; i++)
            {
                temp = int.Parse(cleanedCuit.Substring(i, 1)) * multipliers[i];
                sum += temp;
            }
            int remainder = sum % 11;
            int calculatedDigit = remainder == 0 ? 0 : remainder == 1 ? 9 : 11 - remainder;

            if (verifierDigit != calculatedDigit)
                return "El CUIT no es válido";
            #endregion

            #region Validar Domicilio
            if (clientes.Domicilio == "")
                return "Domicilio es obligatorio";
            #endregion

            #region Validar Telefono
            if (clientes.Telefono == "")
                return "Telefono es obligatorio";
            if (!Regex.IsMatch(clientes.Telefono, @"^\d+$"))
                return "Formato de Telefono incorrecto";
            #endregion

            #region Validar Email
            // Expresión regular para validar el formato de un correo electrónico
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Crear una instancia de Regex con el patrón de correo electrónico
            Regex regex = new Regex(emailPattern);

            // Verificar si el correo electrónico coincide con el patrón
            if (!regex.IsMatch(clientes.Email))
                return "El correo electrónico no es válido";
            #endregion

            return "";
        }
    }
}
