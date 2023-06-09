﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using NuGet.Versioning;
using verificable.Models;
using verificable.ViewModels;

namespace verificable.Controllers
{
    public class FormulariosController : Controller
    {
        private readonly BbddverificableContext _context;
        const int MIN_YEAR = 2019;
        const int MIN_MONTH = 1;
        const int MIN_DAY = 1;

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


        public async Task<IActionResult> Create([Bind("NumAtencion,Cne,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumInscripcion,Estado")] Formulario formulario)
        {
            List<Adquirente> adquirenteCandidates = new List<Adquirente>();
            List<Enajenante> enajenanteCandidates = new List<Enajenante>();

            
            if (ModelState.IsValid)
            {
                _context.Add(formulario);
                await _context.SaveChangesAsync();

                const string regularizacionDePatrimonio = "Regularizacion De Patrimonio";
                const string compraventa = "Compraventa";

                UpdateFormularioVigencia(formulario.NumInscripcion);

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

                ///meter en funcion
                var dataFormHTML = new Dictionary<string, string>();

                foreach (string key in Request.Form.Keys)
                {
                    
                    string valor = Request.Form[key];
                    dataFormHTML.Add(key, valor);
                    
                }
                
                foreach (string k in dataFormHTML.Keys)
                {
                    Console.WriteLine(k + ": " + dataFormHTML[k]);
                }
                /////meter en funcion

                if (formulario.Cne == regularizacionDePatrimonio)
                {
                    RegularizacionDePatrimonioCase(formulario, numAdquirentes, numEnajenantes);
                }
                else if (formulario.Cne == compraventa) 
                {
                    CompraventaCase(formulario, numAdquirentes, numEnajenantes, adquirenteCandidates, enajenanteCandidates, dataFormHTML);
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
        public async Task<IActionResult> Edit(int id, [Bind("NumAtencion,Cne,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumInscripcion,Estado")] Formulario formulario)
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

            formulario.Estado = "Eliminado"; // Cambiar el estado a "eliminado"

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

                // Cambiar el estado a "Eliminado"
                formulario.Estado = "Eliminado";
            }
            
            await _context.SaveChangesAsync();
            UpdateFormularioVigencia(formulario.NumInscripcion);

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

        private void UpdateFormularioVigencia(int? numInscripcion)
        {
            var formularios = _context.Formularios.Where(e => e.NumInscripcion == numInscripcion && e.Estado != "Eliminado").ToList();

            if (formularios.Count >= 1)
            {
                var formularioMaxNumAtencion = formularios.OrderByDescending(e => e.NumAtencion).First();
                DateTime fechaInscripcionMaxNumAtencion = (DateTime)formularioMaxNumAtencion.FechaInscripcion;
                var formularioMaxNumAtencionYear = fechaInscripcionMaxNumAtencion.Year;

                foreach (var formulario in formularios)
                {
                    var formularioFecha = (DateTime)formulario.FechaInscripcion;
                    var formularioYear = formularioFecha.Year;
                    formulario.Estado = formulario == formularioMaxNumAtencion && formularioYear == formularioMaxNumAtencionYear ? "Vigente" : "No vigente";
                    
                }

                _context.SaveChanges();
            }
        }

        public void RegularizacionDePatrimonioCase(Formulario formulario, int numAdquirentes, int numEnajenantes)
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
                    fechaInscripcion = new DateTime(MIN_YEAR, MIN_MONTH, MIN_DAY);
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

        public void CompraventaCase(Formulario formulario, int numAdquirentes, int numEnajenantes, List<Adquirente> adquirenteCandidates, List<Enajenante> enajenanteCandidates, Dictionary <string, string> dataFormHTML)
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
                    CultureInfo culture = new CultureInfo("en-US");
                    porcentajeDerechoAdq += decimal.Parse(Request.Form["adquirentes[" + i + "].porcentaje_derecho"], culture);
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
                    CultureInfo culture = new CultureInfo("en-US");
                    porcentajeDerechoEna += decimal.Parse(Request.Form["enajenantes[" + i + "].porcentaje_derecho"], culture);
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

            enajenanteCandidates = GetEnajenanteCantidates(numEnajenantes, formulario, percentagePerEna, dataFormHTML);
            foreach (var enajenante in enajenanteCandidates)
            {
                _context.Add(new Enajenante
                {
                    RunRut = enajenante.RunRut,
                    PorcentajeDerecho = (double?)enajenante.PorcentajeDerecho,
                    NoAcreditado = (bool?)enajenante.NoAcreditado,
                    NumAtencion = formulario.NumAtencion
                });

                DateTime fechaInscripcion = (DateTime)formulario.FechaInscripcion;
                if (fechaInscripcion.Year < MIN_YEAR)
                {
                    fechaInscripcion = new DateTime(MIN_YEAR, MIN_MONTH, MIN_DAY);
                }
            }
            adquirenteCandidates = GetAdquirienteCantidates(numAdquirentes, formulario, percentagePerAdq, dataFormHTML);
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
                    fechaInscripcion = new DateTime(MIN_YEAR, MIN_MONTH, MIN_DAY);
                }
            }

            List<Multipropietario> ongoingMultipropietarios = GetOngoingMultipropietarios(formulario.Comuna, formulario.Manzana, formulario.Predio, 0);


            bool oneEnajenanteAndAdquirente = enajenanteCandidates.Count == 1 && adquirenteCandidates.Count == 1;

            List<Multipropietario> multipropietariosToAdd = new List<Multipropietario>();

            multipropietariosToAdd = CasesToTransfer(adquirenteCandidates, enajenanteCandidates, oneEnajenanteAndAdquirente, formulario, ongoingMultipropietarios, multipropietariosToAdd);


            foreach (var multipropietario in multipropietariosToAdd)
            {

                _context.Add(new Multipropietario
                {
                    Cne = multipropietario.Cne,
                    Comuna = multipropietario.Comuna,
                    Manzana = multipropietario.Manzana,
                    Predio = multipropietario.Predio,
                    RunRut = multipropietario.RunRut,
                    PorcentajeDerecho = multipropietario.PorcentajeDerecho,
                    Fojas = multipropietario.Fojas,
                    FechaInscripcion = multipropietario.FechaInscripcion,
                    NumInscripcion = multipropietario.NumInscripcion,
                    VigenciaInicial = multipropietario.VigenciaInicial
                });
            }
        }

        public List<Multipropietario> CasesToTransfer(List<Adquirente>  adquirenteCandidates, List<Enajenante> enajenanteCandidates,bool oneEnajenanteAndAdquirente,Formulario formulario, List<Multipropietario> ongoingMultipropietarios, List<Multipropietario> multipropietariosToAdd)
        {
            if (TotalRightPercentage(adquirenteCandidates) == 100)
            {
                multipropietariosToAdd = TotalTransferCase(formulario, enajenanteCandidates, adquirenteCandidates, ongoingMultipropietarios);
            }

            else if (oneEnajenanteAndAdquirente)
            {
                multipropietariosToAdd = OneAdquirenteAndEnajenanteCase(adquirenteCandidates, enajenanteCandidates, ongoingMultipropietarios, formulario);
            }

            else
            {
                multipropietariosToAdd = DominioCase(formulario, enajenanteCandidates, adquirenteCandidates);
            }

            double? acumulatedPercentage = 0;

            foreach (var multipropietario in multipropietariosToAdd)
            {
                acumulatedPercentage += multipropietario.PorcentajeDerecho;
            }

            foreach (var multipropietario in multipropietariosToAdd)
            {
                multipropietario.PorcentajeDerecho = multipropietario.PorcentajeDerecho * 100 / acumulatedPercentage;
            }

            multipropietariosToAdd = MergeSameMultipropietariosPercentage(multipropietariosToAdd);

            return multipropietariosToAdd;
        }
        
        public List<Multipropietario> TotalTransferCase(Formulario formulario, List<Enajenante> enajenanteCandidates, List<Adquirente> adquirenteCandidates, List<Multipropietario> multipropietarios)
        {
            List<Multipropietario> multipropietariosToCompare = multipropietarios;
            List<Multipropietario> multipropietariosToDiscontinue = new List<Multipropietario>();
            List<Multipropietario> potentialMultipropietarios = new List<Multipropietario>();
            double? potentialTotalPercentage = 0;
            double? percentageToTransfer = 0;
            foreach (var enajenante in enajenanteCandidates)
            {
                foreach(var multipropietario in multipropietariosToCompare)
                {
                    if(enajenante.RunRut == multipropietario.RunRut)
                    {
                        multipropietariosToDiscontinue.Add(multipropietario);
                        percentageToTransfer += multipropietario.PorcentajeDerecho;
                    }
                }
            }



            foreach (var adquirente in adquirenteCandidates)
            {
                if(percentageToTransfer > 0)
                {
                    adquirente.PorcentajeDerecho = percentageToTransfer * (adquirente.PorcentajeDerecho/100);
                }
                DateTime fechaInscripcion = (DateTime)formulario.FechaInscripcion;
                if (fechaInscripcion.Year < MIN_YEAR)
                {
                    fechaInscripcion = new DateTime(MIN_YEAR, MIN_MONTH, MIN_DAY);
                }
                potentialMultipropietarios.Add(new Multipropietario
                {
                    Cne = formulario.Cne,
                    Comuna = formulario.Comuna,
                    Manzana = formulario.Manzana,
                    Predio = formulario.Predio,
                    RunRut = adquirente.RunRut,
                    PorcentajeDerecho = adquirente.PorcentajeDerecho,
                    Fojas = formulario.Fojas,
                    FechaInscripcion = formulario.FechaInscripcion,
                    NumInscripcion = formulario.NumInscripcion,
                    VigenciaInicial = fechaInscripcion.Year
                });
                potentialTotalPercentage += adquirente.PorcentajeDerecho;
            }
            foreach(var multipropietario in multipropietariosToCompare)
            {
                if (!multipropietariosToDiscontinue.Contains(multipropietario))
                {
                    DateTime fechaInscripcion = (DateTime)formulario.FechaInscripcion;
                    if (fechaInscripcion.Year < MIN_YEAR)
                    {
                        fechaInscripcion = new DateTime(MIN_YEAR, MIN_MONTH, MIN_DAY);
                    }
                    potentialMultipropietarios.Add(new Multipropietario
                    {
                        Cne = formulario.Cne,
                        Comuna = formulario.Comuna,
                        Manzana = formulario.Manzana,
                        Predio = formulario.Predio,
                        RunRut = multipropietario.RunRut,
                        PorcentajeDerecho = multipropietario.PorcentajeDerecho,
                        Fojas = formulario.Fojas,
                        FechaInscripcion = formulario.FechaInscripcion,
                        NumInscripcion = formulario.NumInscripcion,
                        VigenciaInicial = fechaInscripcion.Year
                    });
                    potentialTotalPercentage += multipropietario.PorcentajeDerecho;
                }
            }

            //Scale percentages
            foreach(var multipropietario in potentialMultipropietarios)
            {
                multipropietario.PorcentajeDerecho = multipropietario.PorcentajeDerecho * 100 / potentialTotalPercentage;
            }

            return potentialMultipropietarios;
        }
        
        public double? TotalRightPercentage(List<Adquirente> compraventaAdquirientes)
        {
            double? percentageCount = 0;
            foreach (var adquiriente in compraventaAdquirientes)
            {
                percentageCount += adquiriente.PorcentajeDerecho;
            }
            return percentageCount;
        }

        public List<Multipropietario> OneAdquirenteAndEnajenanteCase(List<Adquirente> adquirenteCandidates, List<Enajenante> enajenanteCandidates, List<Multipropietario> multipropietarios, Formulario formulario)
        {
            List<Multipropietario> potentialMultipropietarios = new List<Multipropietario>();
            Adquirente adquirente = adquirenteCandidates[0];
            Enajenante enajenante = enajenanteCandidates[0];
            double? originalPercentage = 0;

            DateTime fechaInscripcion = (DateTime)formulario.FechaInscripcion;
            if (fechaInscripcion.Year < MIN_YEAR)
            {
                fechaInscripcion = new DateTime(MIN_YEAR, MIN_MONTH, MIN_DAY);
            }
            if (multipropietarios.Count > 0)
            {
                foreach (var multipropietario in multipropietarios)
                {
                    if(multipropietario.RunRut == enajenante.RunRut)
                    {
                        originalPercentage = multipropietario.PorcentajeDerecho;
                    }
                    else
                    {
                        potentialMultipropietarios.Add(new Multipropietario
                            {
                                Cne = formulario.Cne,
                                Comuna = formulario.Comuna,
                                Manzana = formulario.Manzana,
                                Predio = formulario.Predio,
                                RunRut = multipropietario.RunRut,
                                PorcentajeDerecho = multipropietario.PorcentajeDerecho,
                                Fojas = formulario.Fojas,
                                FechaInscripcion = formulario.FechaInscripcion,
                                NumInscripcion = formulario.NumInscripcion,
                                VigenciaInicial = fechaInscripcion.Year
                            }
                        );
                    }
                }
                double? percetnageToChange = (originalPercentage) * (enajenante.PorcentajeDerecho) / 100;

                adquirente.PorcentajeDerecho = percetnageToChange;
                enajenante.PorcentajeDerecho = originalPercentage - percetnageToChange;
                potentialMultipropietarios.Add(new Multipropietario
                    {
                        Cne = formulario.Cne,
                        Comuna = formulario.Comuna,
                        Manzana = formulario.Manzana,
                        Predio = formulario.Predio,
                        RunRut = enajenante.RunRut,
                        PorcentajeDerecho = enajenante.PorcentajeDerecho,
                        Fojas = formulario.Fojas,
                        FechaInscripcion = formulario.FechaInscripcion,
                        NumInscripcion = formulario.NumInscripcion,
                        VigenciaInicial = fechaInscripcion.Year
                    });
                potentialMultipropietarios.Add(new Multipropietario
                {
                    Cne = formulario.Cne,
                    Comuna = formulario.Comuna,
                    Manzana = formulario.Manzana,
                    Predio = formulario.Predio,
                    RunRut = adquirente.RunRut,
                    PorcentajeDerecho = adquirente.PorcentajeDerecho,
                    Fojas = formulario.Fojas,
                    FechaInscripcion = formulario.FechaInscripcion,
                    NumInscripcion = formulario.NumInscripcion,
                    VigenciaInicial = fechaInscripcion.Year
                });
            }
            else
            {
                potentialMultipropietarios.Add(new Multipropietario
                {
                    Cne = formulario.Cne,
                    Comuna = formulario.Comuna,
                    Manzana = formulario.Manzana,
                    Predio = formulario.Predio,
                    RunRut = enajenante.RunRut,
                    PorcentajeDerecho = 100 - adquirente.PorcentajeDerecho,
                    Fojas = formulario.Fojas,
                    FechaInscripcion = formulario.FechaInscripcion,
                    NumInscripcion = null,
                    VigenciaInicial = fechaInscripcion.Year
                });
                potentialMultipropietarios.Add(new Multipropietario
                {
                    Cne = formulario.Cne,
                    Comuna = formulario.Comuna,
                    Manzana = formulario.Manzana,
                    Predio = formulario.Predio,
                    RunRut = adquirente.RunRut,
                    PorcentajeDerecho = adquirente.PorcentajeDerecho,
                    Fojas = formulario.Fojas,
                    FechaInscripcion = formulario.FechaInscripcion,
                    NumInscripcion = formulario.NumInscripcion,
                    VigenciaInicial = fechaInscripcion.Year
                });
            }

            return potentialMultipropietarios;
        }

        public List<Multipropietario> DominioCase(Formulario formulario, List<Enajenante> enajenanteCandidates, List<Adquirente> adquirenteCandidates)
        {
            List<Multipropietario> multipropietariosToCompare = GetOngoingMultipropietarios(formulario.Comuna, formulario.Manzana, formulario.Predio, 0);
            List<Multipropietario> potentialMultipropietarios = new List<Multipropietario>();
            bool foundEnajenante = false;
            foreach (var multipropietario in multipropietariosToCompare)
            {
                foundEnajenante = false;
                foreach(var enajenante in enajenanteCandidates)
                {
                    if(multipropietario.RunRut == enajenante.RunRut)
                    {
                        foundEnajenante = true;
                        potentialMultipropietarios.Add(new Multipropietario
                        {
                            Cne = formulario.Cne,
                            Comuna = formulario.Comuna,
                            Manzana = formulario.Manzana,
                            Predio = formulario.Predio,
                            RunRut = multipropietario.RunRut,
                            PorcentajeDerecho = multipropietario.PorcentajeDerecho - enajenante.PorcentajeDerecho,
                            Fojas = formulario.Fojas,
                            FechaInscripcion = formulario.FechaInscripcion,
                            NumInscripcion = formulario.NumInscripcion,
                            VigenciaInicial = formulario.FechaInscripcion.Value.Year
                        });
                        break;
                    }
                }
                if (!foundEnajenante)
                {
                    potentialMultipropietarios.Add(new Multipropietario
                    {
                        Cne = formulario.Cne,
                        Comuna = formulario.Comuna,
                        Manzana = formulario.Manzana,
                        Predio = formulario.Predio,
                        RunRut = multipropietario.RunRut,
                        PorcentajeDerecho = multipropietario.PorcentajeDerecho,
                        Fojas = formulario.Fojas,
                        FechaInscripcion = formulario.FechaInscripcion,
                        NumInscripcion = formulario.NumInscripcion,
                        VigenciaInicial = formulario.FechaInscripcion.Value.Year
                    });
                }
            }
            foreach(var multipropietario in potentialMultipropietarios)
            {
                if(multipropietario.PorcentajeDerecho <= 0)
                {
                    potentialMultipropietarios.Remove(multipropietario);
                }
            }
            foreach(var adquirente in adquirenteCandidates)
            {
                potentialMultipropietarios.Add(new Multipropietario
                {
                    Cne = formulario.Cne,
                    Comuna = formulario.Comuna,
                    Manzana = formulario.Manzana,
                    Predio = formulario.Predio,
                    RunRut = adquirente.RunRut,
                    PorcentajeDerecho = adquirente.PorcentajeDerecho,
                    Fojas = formulario.Fojas,
                    FechaInscripcion = formulario.FechaInscripcion,
                    NumInscripcion = formulario.NumInscripcion,
                    VigenciaInicial = formulario.FechaInscripcion.Value.Year
                });
            }
            double? ghostEnajenantes = 0;
            foreach (var enajenante in enajenanteCandidates)
            {
                bool foundMultipropietario = false;
                foreach (var multipropietario in multipropietariosToCompare)
                {
                    if (enajenante.RunRut == multipropietario.RunRut)
                    {
                        foundMultipropietario = true;
                        break;
                    }
                }
                if (!foundMultipropietario)
                {
                    potentialMultipropietarios.Add(new Multipropietario
                    {
                        Cne = formulario.Cne,
                        Comuna = formulario.Comuna,
                        Manzana = formulario.Manzana,
                        Predio = formulario.Predio,
                        RunRut = enajenante.RunRut,
                        PorcentajeDerecho = 0,
                        Fojas = formulario.Fojas,
                        FechaInscripcion = formulario.FechaInscripcion,
                        NumInscripcion = formulario.NumInscripcion,
                        VigenciaInicial = formulario.FechaInscripcion.Value.Year
                    });
                    ghostEnajenantes += 1;
                }
            }
            double? sumaPorcentajes = 0;

            foreach(var multipropietario in potentialMultipropietarios)
            {
                sumaPorcentajes += multipropietario.PorcentajeDerecho;
            }
            if(sumaPorcentajes < 100)
            {
                foreach(var multipropietario in potentialMultipropietarios)
                {
                    if(multipropietario.PorcentajeDerecho == 0)
                    {
                        multipropietario.PorcentajeDerecho = (100 - sumaPorcentajes) / ghostEnajenantes;
                    }
                }
            }
            else
            {
                foreach (var multipropietario in potentialMultipropietarios)
                {
                    if(multipropietario.PorcentajeDerecho == 0)
                    {
                        potentialMultipropietarios.Remove(multipropietario);
                    }
                }
            }

            foreach (var multipropietario in potentialMultipropietarios)
            {
                Console.WriteLine("rut: {0}"+ multipropietario.RunRut + " porcentaje: {0}" + multipropietario.PorcentajeDerecho);
            }

            return potentialMultipropietarios;
        }

        public List<Multipropietario> MergeSameMultipropietariosPercentage(List<Multipropietario> multipropietariosToAdd)
        {
            Dictionary<string, double> mergedPercentages = new Dictionary<string, double>();
            Dictionary<string, int> repetedRuts = new Dictionary<string, int>();  
            List<Multipropietario> multipropietariosToRemove = new List<Multipropietario>();

            foreach (var multi in multipropietariosToAdd)
            {
                if (repetedRuts.ContainsKey(multi.RunRut))
                {
                    repetedRuts[multi.RunRut] += 1;
                }
                else
                {
                    repetedRuts.Add(multi.RunRut, 1);
                }

                if (mergedPercentages.ContainsKey(multi.RunRut))
                {
                    mergedPercentages[multi.RunRut] += multi.PorcentajeDerecho.Value;
                }
                else
                {
                    mergedPercentages.Add(multi.RunRut, multi.PorcentajeDerecho.Value);
                }
            }

            foreach (var multi in multipropietariosToAdd)
            {
                multi.PorcentajeDerecho = mergedPercentages[multi.RunRut];
            }

            foreach (var multi in multipropietariosToAdd)
            {
                if (repetedRuts[multi.RunRut] > 1)
                {
                    multipropietariosToRemove.Add(multi);
                    repetedRuts[multi.RunRut] -= 1;
                }
            }
            foreach (var multiToRemove in multipropietariosToRemove)
            {
                multipropietariosToAdd.Remove(multiToRemove);
            }

            return multipropietariosToAdd;
        }

        public List<Multipropietario> GetOngoingMultipropietarios(string comuna, string manzana, string predio, int get)
        {
            List<Multipropietario> ongoingMultipropietarios = new List<Multipropietario>();

            if (get == 1)
            {
                foreach (var multipropietario in _context.Multipropietarios)
                {
                    bool targetIdentifier = multipropietario.Comuna == comuna && multipropietario.Manzana == manzana &&
                                            multipropietario.Predio == predio;
                    if (multipropietario.VigenciaFinal == null && targetIdentifier)
                    {
                        ongoingMultipropietarios.Add(multipropietario);
                    }

                }
            }
            return ongoingMultipropietarios;
        }

        public List<Enajenante> GetEnajenanteCantidates(int numEnajenantes, Formulario formulario, float percentagePerEna, Dictionary <string, string> dataFormHTML)
        {
            List<Enajenante> enajenanteCandidates = new List<Enajenante>();
            for (var num = 0; num < numEnajenantes;  num++)
            {
                string runRut = dataFormHTML["enajenantes[" + num + "].run_rut"];
                decimal? porcentajeDerecho = null;
                if (!string.IsNullOrEmpty(dataFormHTML["enajenantes[" + num + "].porcentaje_derecho"]))
                {
                    porcentajeDerecho = decimal.Parse(dataFormHTML["enajenantes[" + num + "].porcentaje_derecho"]);
                }
                bool noAcreditado = false;

                string checkboxKey = "enajenantes[" + num + "].no_acreditado";
                string checkboxValue;

                if (dataFormHTML.ContainsKey(checkboxKey))
                {
                    checkboxValue = dataFormHTML[checkboxKey].ToString().ToLower();
                }
                else
                {
                    checkboxValue = "off";
                }

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

        public List<Adquirente> GetAdquirienteCantidates(int numAdquirientes, Formulario formulario, float percentagePerAdq, Dictionary<string, string> dataFormHTML)
        {
            List<Adquirente> adquirienteCandidates = new List<Adquirente>();
            for (var num = 0; num < numAdquirientes; num++)
            {
                string runRut = dataFormHTML["adquirentes[" + num + "].run_rut"];
                decimal? porcentajeDerecho = null;
                if (!string.IsNullOrEmpty(dataFormHTML["adquirentes[" + num + "].porcentaje_derecho"]))
                {
                    porcentajeDerecho = decimal.Parse(dataFormHTML["adquirentes[" + num + "].porcentaje_derecho"]);
                }
                bool noAcreditado = false;

                string checkboxKey = "adquirentes[" + num + "].no_acreditado";
                string checkboxValue;

                if (dataFormHTML.ContainsKey(checkboxKey))
                {
                    checkboxValue = dataFormHTML[checkboxKey].ToString().ToLower();
                }
                else
                {
                    checkboxValue = "off";
                }

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
