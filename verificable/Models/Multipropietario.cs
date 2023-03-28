using System;
using System.Collections.Generic;

namespace verificable.Models;

public partial class Multipropietario
{
    public int Id { get; set; }

    public string Comuna { get; set; } = null!;

    public string Manzana { get; set; } = null!;

    public string Predio { get; set; } = null!;

    public string? RunRut { get; set; }

    public double? PorcentajeDerecho { get; set; }

    public int? Fojas { get; set; }

    public DateTime FechaInscripcion { get; set; }

    public int? NumInscripcion { get; set; }

    public DateTime? VigenciaInicial { get; set; }

    public DateTime? VigenciaFinal { get; set; }
}
