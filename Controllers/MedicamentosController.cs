using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_POO_IIPAC_2026.Data;
using Proyecto_POO_IIPAC_2026.Models;

namespace Proyecto_POO_IIPAC_2026.Controllers
{
    [ApiController]
    [Route("api/medicamentos")]
    [Authorize(Roles = "Doctor,Enfermero")]
    public class MedicamentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicamentosController(
            AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicamentos()
        {
            var medicamentos =
                await _context.Medicamentos.ToListAsync();

            return Ok(medicamentos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicamento(
            int id)
        {
            var medicamento =
                await _context.Medicamentos
                    .FindAsync(id);


            if (medicamento == null)
            {
                return NotFound(new
                {
                    mensaje = "Medicamento no encontrado."
                });
            }


            return Ok(medicamento);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CrearMedicamento(
            [FromBody] Medicamento medicamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            medicamento.Id = 0;


            _context.Medicamentos.Add(medicamento);

            await _context.SaveChangesAsync();


            return CreatedAtAction(
                nameof(GetMedicamento),
                new { id = medicamento.Id },
                medicamento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> EditarMedicamento(
            int id,
            [FromBody] Medicamento medicamento)
        {
            if (id != medicamento.Id)
            {
                return BadRequest(new
                {
                    mensaje = "El ID de la URL no coincide con el ID del medicamento."
                });
            }


            var existente =
                await _context.Medicamentos
                    .FindAsync(id);


            if (existente == null)
            {
                return NotFound(new
                {
                    mensaje = "Medicamento no encontrado."
                });
            }


            existente.Nombre =
                medicamento.Nombre;

            existente.FechaCaducidad =
                medicamento.FechaCaducidad;

            existente.Cantidad =
                medicamento.Cantidad;

            existente.Tipo =
                medicamento.Tipo;


            await _context.SaveChangesAsync();


            return Ok(new
            {
                mensaje = "Medicamento actualizado.",
                medicamento = existente
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> EliminarMedicamento(
            int id)
        {
            var medicamento =
                await _context.Medicamentos
                    .FindAsync(id);


            if (medicamento == null)
            {
                return NotFound(new
                {
                    mensaje = "Medicamento no encontrado."
                });
            }


            _context.Medicamentos.Remove(
                medicamento);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                mensaje = "Medicamento eliminado correctamente."
            });
        }
    }
}