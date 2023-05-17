﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using verificable.Models;
using verificable.ViewModels;

namespace verificable.Controllers
{
    public class FormulariosController : Controller
    {
        private readonly BbddverificableContext _context;
        const int MIN_YEAR = 2019;
        public FormulariosController(BbddverificableContext context)
        {
            _context = context;
        }

        // GET: Formularios
        public async Task<IActionResult> Index()
        {
              return _context.Formularios != null ? 
                          View(await _context.Formularios.ToListAsync()) :
                          Problem("Entity set 'BbddverificableContext.Formularios'  is null.");
        }

        // GET: Formularios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }
            var formulario = await _context.Formularios
                .FirstOrDefaultAsync(m => m.NumAtencion == id);
            if (formulario == null)
            {
                return NotFound();
            }
            ViewData["Enajenantes"] = await _context.Enajenantes
                .Where(e => e.NumAtencion == id)
                .ToListAsync();
            ViewData["Adquirientes"] = await _context.Adquirentes
                .Where(e => e.NumAtencion == id)
                .ToListAsync();
            return View(formulario);
        }

        // GET: Formularios/Create
        public IActionResult Create()
        {
            var FormularioComunaViewModel = new FormularioComunaViewModel
            {
                FormulariosList = _context.Formularios.ToList(),
                ComunasList = _context.Comunas.ToList()
            };
            return View(FormularioComunaViewModel);
        }

        // POST: Formularios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Create([Bind("NumAtencion,Cne,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumInscripcion")] Formulario formulario)
        {
            List<Adquirente> adquirenteCandidates = new List<Adquirente>();
            List<Enajenante> enajenanteCandidates = new List<Enajenante>();
            if (ModelState.IsValid)
            {
                _context.Add(formulario);
                await _context.SaveChangesAsync();

                int numEnajenantes = 0;
                foreach (string key in Request.Form.Keys)
                {
                    string valor = Request.Form[key];
                    bool validEnajenante = key.StartsWith("enajenantes") && key.EndsWith("run_rut") && valor.Length != 0;

                    if (validEnajenante)
                    {
                        numEnajenantes++;
                    }
                }

                int numAdquirentes = 0;
                foreach (string key in Request.Form.Keys)
                {
                    string valor = Request.Form[key];
                    bool validAdquirente = key.StartsWith("adquirentes") && key.EndsWith("run_rut") && valor.Length != 0;

                    if (validAdquirente)
                    {
                        numAdquirentes++;
                    }
                }
                if (formulario.Cne == "Regularizacion De Patrimonio")
                {
                    //El siguente bloque es para ver a cuales adquirentes les falta un porcentaje y calcular la suma de pocentajes total.
                    decimal? porcentajeDerechoAdq = 0;
                    List<string> adqWithoutPercentage = new List<string>();

                    for (int i = 0; i < numAdquirentes; i++)
                    {
                        string runRutAdqToValidatePercentage = Request.Form["adquirentes[" + i + "].run_rut"];

                        if (!string.IsNullOrEmpty(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]))
                        {
                            porcentajeDerechoAdq += decimal.Parse(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]);
                        }

                        string checkboxValueAdq = Request.Form["adquirentes[" + i + "].no_acreditado"].ToString().ToLower();
                        if (checkboxValueAdq == "on" || checkboxValueAdq == "true")
                        {
                            adqWithoutPercentage.Add(runRutAdqToValidatePercentage);
                        }
                    }

                    //Calcula cual es el porcentaje que se le da a cada adquiriente.
                    float percentagePerAdq = ((float)(100 - porcentajeDerechoAdq)) / adqWithoutPercentage.Count;

                    for (int i = 0; i < numAdquirentes; i++)
                    {
                        string runRut = Request.Form["adquirentes[" + i + "].run_rut"];
                        decimal? porcentajeDerecho = null;
                        if (!string.IsNullOrEmpty(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]))
                        {
                            porcentajeDerecho = decimal.Parse(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]);
                        }
                        bool noAcreditado = false;
                        string checkboxValue = Request.Form["adquirentes[" + i + "].no_acreditado"].ToString().ToLower();
                        if (checkboxValue == "on" || checkboxValue == "true")
                        {
                            noAcreditado = true;
                            porcentajeDerecho = (decimal?)percentagePerAdq;
                        }
                        _context.Add(new Adquirente { RunRut = runRut, PorcentajeDerecho = (double?)porcentajeDerecho, NoAcreditado = (bool?)noAcreditado, NumAtencion = formulario.NumAtencion });

                        DateTime fechaInscripcion = (DateTime)formulario.FechaInscripcion;
                        if (fechaInscripcion.Year < MIN_YEAR)
                        {
                            fechaInscripcion = new DateTime(MIN_YEAR, 1, 1);
                        }
                        _context.Add(new Multipropietario
                        {
                            Cne = formulario.Cne,
                            Comuna = formulario.Comuna,
                            Manzana = formulario.Manzana,
                            Predio = formulario.Predio,
                            RunRut = runRut,
                            PorcentajeDerecho = (double?)porcentajeDerecho,
                            Fojas = formulario.Fojas,
                            FechaInscripcion = formulario.FechaInscripcion,
                            NumInscripcion = formulario.NumInscripcion,
                            VigenciaInicial = fechaInscripcion.Year
                        });
                        if (!ComunaExists(formulario.Comuna))
                        {
                            _context.Add(new Comuna { Nombre = formulario.Comuna });
                        }
                    }
                }
                else if (formulario.Cne == "Compraventa") 
                {
                    //El siguente bloque es para ver a cuales adquirentes les falta un porcentaje y calcular la suma de pocentajes total.
                    decimal? porcentajeDerechoAdq = 0;
                    decimal? porcentajeDerechoEna = 0;
                    List<string> adqWithoutPercentage = new List<string>();
                    List<string> enaWithoutPercentage = new List<string>();

                    for (int i = 0; i < numAdquirentes; i++)
                    {
                        string runRutAdqToValidatePercentage = Request.Form["adquirentes[" + i + "].run_rut"];

                        if (!string.IsNullOrEmpty(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]))
                        {
                            porcentajeDerechoAdq += decimal.Parse(Request.Form["adquirentes[" + i + "].porcentaje_derecho"]);
                        }

                        string checkboxValueAdq = Request.Form["adquirentes[" + i + "].no_acreditado"].ToString().ToLower();
                        if (checkboxValueAdq == "on" || checkboxValueAdq == "true")
                        {
                            adqWithoutPercentage.Add(runRutAdqToValidatePercentage);
                        }
                    }

                    for (int i = 0; i < numEnajenantes; i++)
                    {
                        string runRutEnaToValidatePercentage = Request.Form["enajenantes[" + i + "].run_rut"];

                        if (!string.IsNullOrEmpty(Request.Form["enajenantes[" + i + "].porcentaje_derecho"]))
                        {
                            porcentajeDerechoEna += decimal.Parse(Request.Form["enajenantes[" + i + "].porcentaje_derecho"]);
                        }

                        string checkboxValueEna = Request.Form["enajenantes[" + i + "].no_acreditado"].ToString().ToLower();
                        if (checkboxValueEna == "on" || checkboxValueEna == "true")
                        {
                            enaWithoutPercentage.Add(runRutEnaToValidatePercentage);
                        }
                    }
                    //Calcula cual es el porcentaje que se le da a cada adquiriente y enajenante.
                    float percentagePerAdq = ((float)(100 - porcentajeDerechoAdq)) / adqWithoutPercentage.Count;
                    float percentagePerEna = ((float)(100 - porcentajeDerechoEna)) / enaWithoutPercentage.Count;

                    enajenanteCandidates = GetEnajenanteCantidates(numEnajenantes, formulario, percentagePerEna);
                    foreach (var enajenante in enajenanteCandidates)
                    {
                        _context.Add(new Enajenante { 
                            RunRut = enajenante.RunRut, 
                            PorcentajeDerecho = (double?)enajenante.PorcentajeDerecho, 
                            NoAcreditado = (bool?)enajenante.NoAcreditado, 
                            NumAtencion = formulario.NumAtencion 
                        });

                        DateTime fechaInscripcion = (DateTime)formulario.FechaInscripcion;
                        if (fechaInscripcion.Year < MIN_YEAR)
                        {
                            fechaInscripcion = new DateTime(MIN_YEAR, 1, 1);
                        } 
                    }
                    adquirenteCandidates = GetAdquirienteCantidates(numAdquirentes, formulario, percentagePerAdq);
                    foreach (var adquiriente in adquirenteCandidates)
                    {
                        _context.Add(new Adquirente 
                        {
                            RunRut = adquiriente.RunRut, 
                            PorcentajeDerecho = (double?)adquiriente.PorcentajeDerecho, 
                            NoAcreditado = (bool?)adquiriente.NoAcreditado, 
                            NumAtencion = formulario.NumAtencion 
                        });

                        DateTime fechaInscripcion = (DateTime)formulario.FechaInscripcion;
                        if (fechaInscripcion.Year < MIN_YEAR)
                        {
                            fechaInscripcion = new DateTime(MIN_YEAR, 1, 1);
                        }
                    }
                    Console.WriteLine("Enajenantes Candidates: ");
                    foreach (var enajenante in enajenanteCandidates)
                    {
                        Console.WriteLine(enajenante.RunRut, enajenante.PorcentajeDerecho);
                    }
                    Console.WriteLine("Adquirientes Candidates: ");
                    foreach (var adquiriente in adquirenteCandidates)
                    {
                        Console.WriteLine(adquiriente.RunRut, adquiriente.PorcentajeDerecho);
                    }
                    
                    Console.WriteLine("Informacion del formulario: ");
                    Console.WriteLine(formulario.Comuna);
                    Console.WriteLine(formulario.Manzana);
                    Console.WriteLine(formulario.Predio);
                    Console.WriteLine("Last Multipropietarios Related: ");
                    List<Multipropietario> ongoingMultipropietarios = GetOngoingMultipropietarios(formulario.Comuna, formulario.Manzana, formulario.Predio);
                    foreach (var ongoing in ongoingMultipropietarios)
                    {
                        Console.WriteLine(ongoing.RunRut);
                        Console.WriteLine(ongoing.PorcentajeDerecho);
                    }

                    if (TotalRightPercentage(adquirenteCandidates) == 100)
                    {
                        TotalTransferCase(formulario.Comuna, formulario.Manzana, formulario.Predio);
                    } 
                    /*_context.Add(new Multipropietario
                    {
                        Cne = formulario.Cne,
                        Comuna = formulario.Comuna,
                        Manzana = formulario.Manzana,
                        Predio = formulario.Predio,
                        RunRut = runRut,
                        PorcentajeDerecho = (double?)porcentajeDerecho,
                        Fojas = formulario.Fojas,
                        FechaInscripcion = formulario.FechaInscripcion,
                        NumInscripcion = formulario.NumInscripcion,
                        VigenciaInicial = fechaInscripcion.Year
                    });*/
                }
                
                enajenanteCandidates.Clear();
                adquirenteCandidates.Clear();
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(formulario);
        }

        // GET: Formularios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formularios.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }
            return View(formulario);
        }

        // POST: Formularios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumAtencion,Cne,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumInscripcion")] Formulario formulario)
        {
            if (id != formulario.NumAtencion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(formulario.NumAtencion))
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
            return View(formulario);
        }

        // GET: Formularios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formularios
                .FirstOrDefaultAsync(m => m.NumAtencion == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // POST: Formularios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Formularios == null)
            {
                return Problem("Entity set 'BbddverificableContext.Formularios'  is null.");
            }
            var formulario = await _context.Formularios.FindAsync(id);
            if (formulario != null)
            {
                // Delete associated enajenantes first
                var enajenantes = _context.Enajenantes.Where(e => e.NumAtencion == id);
                foreach (var enajenante in enajenantes)
                {
                    _context.Enajenantes.Remove(enajenante);
                }

                // Delete associated adquirentes first
                var adquirentes = _context.Adquirentes.Where(a => a.NumAtencion == id);
                foreach (var adquirente in adquirentes)
                {
                    _context.Adquirentes.Remove(adquirente);
                }
                _context.Formularios.Remove(formulario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioExists(int id)
        {
          return (_context.Formularios?.Any(e => e.NumAtencion == id)).GetValueOrDefault();
        }
        public bool ComunaExists(string nombre)
        {
            return _context.Comunas.Any(e => e.Nombre == nombre);
        }
        private void TotalTransferCase(string comuna, string manzana, string predio)
        {
            List<Multipropietario> multipropietariosToCompare = GetOngoingMultipropietarios(comuna, manzana, predio);
            _context.Multipropietarios.OrderBy(obj => obj.VigenciaInicial).ThenBy(obj => obj.NumInscripcion);
            //Search for the previous information about the multipropietarios
            //Asign values for this cacse, save data in multipropietrios
        }
        private double? TotalRightPercentage(List<Adquirente> compraventaAdquirientes)
        {
            double? percentageCount = 0;
            foreach (var adquiriente in compraventaAdquirientes)
            {
                percentageCount += adquiriente.PorcentajeDerecho;
            }
            return percentageCount;
        }

        private List<Multipropietario> GetOngoingMultipropietarios(string comuna, string manzana, string predio)
        {
            List<Multipropietario> ongoingMultipropietarios = new List<Multipropietario>();
            foreach (var multipropietario in _context.Multipropietarios)
            {
                bool targetIdentifier = multipropietario.Comuna == comuna && multipropietario.Manzana == manzana &&
                                        multipropietario.Predio == predio;
                if (multipropietario.VigenciaFinal == null && targetIdentifier)
                {
                    ongoingMultipropietarios.Add(multipropietario);
                }
                
            }
            return ongoingMultipropietarios;
        }

        private List<Enajenante> GetEnajenanteCantidates(int numEnajenantes, Formulario formulario, float percentagePerEna)
        {
            List<Enajenante> enajenanteCandidates = new List<Enajenante>();
            for (var num = 0; num < numEnajenantes;  num++)
            {
                string runRut = Request.Form["enajenantes[" + num + "].run_rut"];
                decimal? porcentajeDerecho = null;
                if (!string.IsNullOrEmpty(Request.Form["enajenantes[" + num + "].porcentaje_derecho"]))
                {
                    porcentajeDerecho = decimal.Parse(Request.Form["enajenantes[" + num + "].porcentaje_derecho"]);
                }
                bool noAcreditado = false;
                string checkboxValue = Request.Form["enajenantes[" + num + "].no_acreditado"].ToString().ToLower();
                if (checkboxValue == "on" || checkboxValue == "true")
                {
                    noAcreditado = true;
                    porcentajeDerecho = (decimal?)percentagePerEna;
                }
                enajenanteCandidates.Add(new Enajenante
                {
                    RunRut = runRut,
                    PorcentajeDerecho = (double?)porcentajeDerecho,
                    NoAcreditado = (bool?)noAcreditado,
                    NumAtencion = formulario.NumAtencion
                });
            }
            return enajenanteCandidates;
        }

        private List<Adquirente> GetAdquirienteCantidates(int numAdquirientes, Formulario formulario, float percentagePerAdq)
        {
            List<Adquirente> adquirienteCandidates = new List<Adquirente>();
            for (var num = 0; num < numAdquirientes; num++)
            {
                string runRut = Request.Form["adquirentes[" + num + "].run_rut"];
                decimal? porcentajeDerecho = null;
                if (!string.IsNullOrEmpty(Request.Form["adquirentes[" + num + "].porcentaje_derecho"]))
                {
                    porcentajeDerecho = decimal.Parse(Request.Form["adquirentes[" + num + "].porcentaje_derecho"]);
                }
                bool noAcreditado = false;
                string checkboxValue = Request.Form["adquirentes[" + num + "].no_acreditado"].ToString().ToLower();
                if (checkboxValue == "on" || checkboxValue == "true")
                {
                    noAcreditado = true;
                    porcentajeDerecho = (decimal?)percentagePerAdq;
                }
                adquirienteCandidates.Add(new Adquirente
                {
                    RunRut = runRut,
                    PorcentajeDerecho = (double?)porcentajeDerecho,
                    NoAcreditado = (bool?)noAcreditado,
                    NumAtencion = formulario.NumAtencion
                });
            }
            return adquirienteCandidates;
        }
    }
}
