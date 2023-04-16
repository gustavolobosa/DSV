using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using verificable.Models;

namespace verificable.Controllers
{
    public class MultipropietariosController : Controller
    {
        private readonly BbddverificableContext _context;

        public MultipropietariosController(BbddverificableContext context)
        {
            _context = context;
            
            var multipropietarios = _context.Multipropietarios.ToList();
            foreach (var multipropietario in multipropietarios)
            {
                var comuna = multipropietario.Comuna;
                var manzana = multipropietario.Manzana;
                var predio = multipropietario.Predio;
                var fecha = multipropietario.FechaInscripcion;
                var numInscripcion = multipropietario.NumInscripcion;

                foreach(var comparison in multipropietarios)
                {
                    if (comparison.Comuna == comuna && comparison.Manzana == manzana && comparison.Predio == predio &&
                        comparison.FechaInscripcion < fecha && comparison.VigenciaFinal == null)
                    {
                        comparison.VigenciaFinal = fecha;
                    }
                    else if (comparison.Comuna == comuna && comparison.Manzana == manzana && comparison.Predio == predio &&
                        comparison.FechaInscripcion == fecha && comparison.VigenciaFinal == null)
                    {
                        if(comparison.NumInscripcion > numInscripcion)
                        {
                            multipropietario.VigenciaFinal = comparison.FechaInscripcion;
                        }
                        else if (comparison.NumInscripcion > numInscripcion)
                        {
                            comparison.VigenciaFinal = fecha;
                        }
                        
                    }
                    else if (comparison.Comuna == comuna && comparison.Manzana == manzana && comparison.Predio == predio &&
                        comparison.FechaInscripcion > fecha && comparison.VigenciaFinal != null)
                    {
                        _context.Remove(multipropietario);

                    }
                }
            }
            _context.SaveChanges();

        }

        // GET: Multipropietarios
        public async Task<IActionResult> Index(string comunaInput)
        {
            if (string.IsNullOrWhiteSpace(comunaInput))
            {
                return View(await _context.Multipropietarios.ToListAsync());
            }
            else
            {
                var comuna = Request.Form["comunaInput"];
                var manzana = Request.Form["manzanaInput"];
                var predio = Request.Form["predioInput"];
                var year = int.Parse(Request.Form["añoInput"]);



                var multipropietarios = await _context.Multipropietarios
                    .Where(m => m.Comuna.Contains(comuna)
                    && m.Manzana.Contains(manzana)
                    && m.Predio.Contains(predio)
                    && m.FechaInscripcion.Value.Year == year)
                    .ToListAsync();
                return View(multipropietarios);
            }
        }




        // GET: Multipropietarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Multipropietarios == null)
            {
                return NotFound();
            }

            var multipropietario = await _context.Multipropietarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multipropietario == null)
            {
                return NotFound();
            }

            return View(multipropietario);
        }

        // GET: Multipropietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Multipropietarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comuna,Manzana,Predio,RunRut,PorcentajeDerecho,Fojas,FechaInscripcion,NumInscripcion,VigenciaInicial,VigenciaFinal")] Multipropietario multipropietario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(multipropietario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(multipropietario);
        }

        // GET: Multipropietarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Multipropietarios == null)
            {
                return NotFound();
            }

            var multipropietario = await _context.Multipropietarios.FindAsync(id);
            if (multipropietario == null)
            {
                return NotFound();
            }
            return View(multipropietario);
        }

        // POST: Multipropietarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comuna,Manzana,Predio,RunRut,PorcentajeDerecho,Fojas,FechaInscripcion,NumInscripcion,VigenciaInicial,VigenciaFinal")] Multipropietario multipropietario)
        {
            if (id != multipropietario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(multipropietario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MultipropietarioExists(multipropietario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(multipropietario);
        }

        // GET: Multipropietarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Multipropietarios == null)
            {
                return NotFound();
            }

            var multipropietario = await _context.Multipropietarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multipropietario == null)
            {
                return NotFound();
            }

            return View(multipropietario);
        }

        // POST: Multipropietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Multipropietarios == null)
            {
                return Problem("Entity set 'BbddverificableContext.Multipropietarios'  is null.");
            }
            var multipropietario = await _context.Multipropietarios.FindAsync(id);
            if (multipropietario != null)
            {
                _context.Multipropietarios.Remove(multipropietario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MultipropietarioExists(int id)
        {
          return (_context.Multipropietarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
