using System;
using System.Collections.Generic;

namespace CarnetsdeFeria.Models;

public partial class Participacion
{
    public int Id { get; set; }

    public string? DescripcionParticipacion { get; set; }

    public bool? Estatus { get; set; }

    public virtual ICollection<FeriaCarnet> FeriaCarnets { get; set; } = new List<FeriaCarnet>();
}
