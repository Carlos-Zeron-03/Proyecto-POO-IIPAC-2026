using Microsoft.AspNetCore.Mvc;
using ProyectoAPI.Models;
using ProyectoAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentosController : ControllerBase
    {
        // Guarda y crea un puente a la base de datos
        private readonly AppDbContext _context;

        // Recibe la conexión con la base de datos lista para usarse
        public MedicamentosController(AppDbContext context)
        {
            _context = context;
        }

        // Consulta y devuelve la lista de los medicamentos registrados a la base de datos
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Medicamentos);
        }

        // Autoriza que solo usuarios autorizados como "Doctor" y "Enfermero" pueden entrar 
        [Authorize(Roles = "Doctor,Enfermero")]

        // Abre la pantalla principal
        public IActionResult Index()
        {
            return View();
        }

        // Muestra el formulario para llenar los datos de un nuevo medicamento
        public IActionResult Register()
        {
            return View();
        }
             
        // Este método sirve de apoyo para evitar errores en el código,
        // pero si se ejecuta avisa que la pantalla web aún no existe

        private IActionResult View(Medicamento med)
        {
            throw new NotImplementedException();
        }

        // Recibe los datos del medicamento y los guarda en la base de datos
        [HttpPost]
        public IActionResult Post(Medicamento nuevo)
        {
            _context.Medicamentos.Add(nuevo);
            _context.SaveChanges(); 
            return Ok("Medicamento agregado");
        }

        // Recibe los datos desde un formulario de pagina web
        public async Task<IActionResult> Create(Medicamento med)
        {
            // Revisa si el usuario ingreso bien todos los campos
            if (ModelState.IsValid)
            {
                // Guarda en la base de datos y si todo esta bien lo envía de vuelta
                return RedirectToAction("Index");
            }

            // Si hubo un error se queda en la misma pantalla
            return View(med);
        }

        // Muestra la pantalla para poder modificar un medicamento, solo los doctores tienen esa autorización
        [Authorize(Roles = "Doctor")]
        public IActionResult Edit(int id)
        {
            return View();
        }

        // Avisa que la pantalla web todavía no ha sido programada
        private IActionResult View()
        {
            throw new NotImplementedException();
        }

        // Pasa el ID a una URL
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Busca si el medicamento con ese Id existe
            var m = _context.Medicamentos.FirstOrDefault(x => x.Id == id);

            // Si no lo encuentra avisa con un error
            if (m == null)
                return NotFound("No encontrado");

            // Si lo encuentra, lo elimina y confirma la acción
            _context.Medicamentos.Remove(m);
            return Ok("Eliminado");
        }

        // Hace lo mismo que el borrado anterior, pero pensado para el botón eliminar
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca el medicamento por su Id
            var med = await _context.Medicamentos.FindAsync(id);

            // Si existe lo borra y guarda los cambios sin congelar la aplicación
            if (med != null)
            {
                _context.Medicamentos.Remove(med);
                await _context.SaveChangesAsync();
            }

            // Manda al usuario de regreso a la pantalla principal
            return RedirectToAction("Index");
        }
    }
}