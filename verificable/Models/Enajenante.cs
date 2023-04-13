using System;
using System.Collections.Generic;

namespace verificable.Models;

public partial class Enajenante
{
    public int Id { get; set; }

    public int? NumAtencion { get; set; }

    public string? RunRut { get; set; }

    public double? PorcentajeDerecho { get; set; }

    public bool? NoAcreditado { get; set; }

    public virtual Formulario? NumAtencionNavigation { get; set; }
}
