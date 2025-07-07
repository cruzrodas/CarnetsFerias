using System;
using System.Collections.Generic;

namespace CarnetsdeFeria.Models;

public partial class Ferium
{
    public int Id { get; set; }

    public string? DescripcionFeria { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool? Estatus { get; set; }

    public bool? Activa { get; set; }

    public virtual ICollection<FeriaArea> FeriaAreas { get; set; } = new List<FeriaArea>();
}
