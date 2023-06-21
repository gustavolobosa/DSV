using System;
using System.Collections.Generic;

namespace verificable.Models;

public partial class Formulario
{
    public int NumAtencion { get; set; }

    public string? Cne { get; set; }

    public string? Comuna { get; set; }

    public string? Manzana { get; set; }

    public string? Predio { get; set; }

    public string? Estado { get; set; }

    public int? Fojas { get; set; }

    public DateTime? FechaInscripcion { get; set; }

    public int? NumInscripcion { get; set; }

    public virtual ICollection<Adquirente> Adquirentes { get; } = new List<Adquirente>();

    public virtual ICollection<Enajenante> Enajenantes { get; } = new List<Enajenante>();
}
