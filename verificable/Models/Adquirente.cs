﻿using System;
using System.Collections.Generic;

namespace verificable.Models;

public partial class Adquirente
{
    public int Id { get; set; }

    public int? NumAtencion { get; set; }

    public string? RunRut { get; set; }

    public double? PorcentajeDerecho { get; set; }

    public double? NoAcreditado { get; set; }

    public virtual Formulario? NumAtencionNavigation { get; set; }
}
